using RuPostWSDL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace RPTApi.Telegram
{
    public class Worker : Interfaces.IBotWorker
    {
        global::Telegram.Bot.TelegramBotClient client;
        RPTApi.Helpers.Config config;
        RuPostApi postApi;
        public DataBase.BotDataContext DataBase { get; set; }
        public async Task<bool> DeleteOrder(string barcodeOrName,int userID)
        {
            var orderToDelete = DataBase.Users.FirstOrDefault(u=>u.Id==userID).Orders.FirstOrDefault(o => o.Barcode == barcodeOrName || o.Name==barcodeOrName);
            if (orderToDelete != null)
            {
                DataBase.Users.FirstOrDefault(u=>u.Id==userID).Orders.Remove(orderToDelete);
                await DataBase.SaveChangesAsync();
                return true;
            }
            return false;
        }
        public async Task<bool> RenameOrder(string barcode,string newName,int userId)
        {
            var orderToRename = DataBase.Users.FirstOrDefault(u => u.Id == userId).Orders.FirstOrDefault(o => o.Barcode == barcode);
            if (orderToRename != null)
            {
                DataBase.Update(orderToRename);
                orderToRename.Name = newName;
                await DataBase.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<DataBase.Models.Order> GetOrderAsync(string barcodeOrName, int userId)
        {
            var toTrack = DataBase.Users.FirstOrDefault(u => u.Id == userId).Orders.FirstOrDefault(o => o.Barcode == barcodeOrName || o.Name == barcodeOrName);
            
            if (toTrack!=null)
            {
                var order = DataBase.Orders.FirstOrDefault(x => x.Barcode == toTrack.Barcode);
                if (DateTime.Now - order.LastQuerry < TimeSpan.FromHours(12))
                {
                    order.LastQuerry = DateTime.Now;
                    return order;
                }
                else
                {
                    return await RefreshOrderInfoAsync(toTrack.Barcode);
                }
            }
            else
            {
                return await GetNewOrderAsync(barcodeOrName, userId);
            }
        }

        public async Task RegisterNewUser(int id)
        {
            DataBase.Users.Add(new DataBase.Models.BotUser(){ Id = id });
            await DataBase.SaveChangesAsync();
        }

        public Worker(string cfgFilePath)
        {
            config = RPTApi.Helpers.ConfigLoader.GetConfig(cfgFilePath);
            client = new TelegramBotClient(config.BotKey);
            DataBase = new DataBase.BotDataContext(config);
            postApi = new RuPostApi();
            postApi.AuthorizeAsync(config.RuPostApiLogin, config.RuPostApiPassword).Wait();
        }

        public Worker(RPTApi.Helpers.Config config)
        {
            client = new TelegramBotClient(config.BotKey);
            DataBase = new DataBase.BotDataContext(config);
            postApi = new RuPostApi();
            postApi.AuthorizeAsync(config.RuPostApiLogin, config.RuPostApiPassword).Wait();
        }
        public void Start()
        {
            client.OnMessage += Client_OnMessage;
            client.OnCallbackQuery += Client_OnCallbackQuery;
            client.StartReceiving();

        }

        private async void Client_OnCallbackQuery(object sender, global::Telegram.Bot.Args.CallbackQueryEventArgs e)
        {
            try
            {
                await Helpers.MessageToCommand.GetCommand(e.CallbackQuery.Data).Execute(e.CallbackQuery.Data, this, e.CallbackQuery.From.Id);
            }
            catch
            {
                await SendUserMessage(e.CallbackQuery.From.Id, "Мне не удалось выполнить это:(\nПопробуй позже");
            }
        }

        public void Stop()
        {
            client.StopReceiving();
        }

        public async Task SendUserMessage(int UserId, string message, IReplyMarkup replyMarkup = null)
        {
            await client.SendTextMessageAsync(UserId, message, replyMarkup:replyMarkup);
        }
        private async void Client_OnMessage(object sender, global::Telegram.Bot.Args.MessageEventArgs e)
        {
            if(e.Message.Type == global::Telegram.Bot.Types.Enums.MessageType.Text)
            {
                if (!DataBase.Users.Any(u => u.Id == e.Message.From.Id))
                {
                    await RegisterNewUser(e.Message.From.Id);
                    await client.SendTextMessageAsync(e.Message.From.Id, "Приветствую тебя!", replyMarkup: Keyboards.CommandsKeyboard.Get());
                }
                else
                {
                    try
                    {
                        await Helpers.MessageToCommand.GetCommand(e.Message.Text).Execute(e.Message.Text, this, e.Message.From.Id);
                    }
                    catch
                    {
                        await SendUserMessage(e.Message.From.Id, "Мне не удалось выполнить это:(\nПопробуй позже");
                    }
                }
            }
        }
        #region Methods-Helpers
        private async Task<DataBase.Models.Order> GetNewOrderAsync(string barcode,int userId)
        {
            OperationHistoryResponse orderHistory;
          
            orderHistory = await postApi.GetOperationsHistoryAsync(barcode);

            if (orderHistory.OperationHistoryData == null)
                return null;
            var newOrder = new DataBase.Models.Order()
            {
                Barcode = barcode,
                LastQuerry = DateTime.Now,
                StartTracking = DateTime.Now,
                UserId = userId 
            };

            DataBase.Orders.Add(newOrder);
            await DataBase.SaveChangesAsync();
          
            newOrder = DataBase.Orders.FirstOrDefault(o => o.Barcode == barcode);
            var recs = new List<DataBase.Models.Record>();
            foreach (var record in orderHistory.OperationHistoryData)
            {
                var newRecord = new DataBase.Models.Record()
                {
                    DateTime = record.OperationParameters.OperDate,
                    Location = record.AddressParameters.OperationAddress.Description ?? record.AddressParameters.CountryOper.NameRU ,
                    OperationType = record.OperationParameters.OperType.Name,
                    OperationAttribute = record.OperationParameters.OperAttr?.Name,
                    OrderBarcode = barcode
                };
                // Avoiding of copying
                if(!recs.Any(r=>r.DateTime==newRecord.DateTime && r.Location == newRecord.Location))
                    recs.Add(newRecord);
            }
            DataBase.AddRange(recs);
            await DataBase.SaveChangesAsync();

            return DataBase.Orders.FirstOrDefault(o => o.Barcode == barcode);
        }

        private async Task<DataBase.Models.Order> RefreshOrderInfoAsync(string barcode)
        {
            var history = await postApi.GetOperationsHistoryAsync(barcode);
            var recs = new List<DataBase.Models.Record>();
            recs.AddRange(DataBase.Orders.FirstOrDefault(o => o.Barcode == barcode).Records);
            var order = DataBase.Orders.FirstOrDefault(o => o.Barcode == barcode);
            foreach (var rec in history.OperationHistoryData)
            {
                var date = rec.OperationParameters.OperDate;
                var location = rec.AddressParameters.OperationAddress.Description;
                var newRecord = new DataBase.Models.Record() 
                { 
                    DateTime = date,
                    Location = location,
                    OrderBarcode = barcode 
                };
                //Avoiding of copying info
                if (!recs.Any(r => r.OrderBarcode==barcode && r.DateTime == date))
                    recs.Add(newRecord);
            }
            DataBase.AddRange(recs);
            DataBase.Update(order);
            order.LastQuerry = DateTime.Now;
            DataBase.Update(order);
            await DataBase.SaveChangesAsync();
            return DataBase.Orders.FirstOrDefault(o => o.Barcode == barcode);
        }
        #endregion
    }
}