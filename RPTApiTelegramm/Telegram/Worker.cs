using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPTApi.Telegram
{
    public class Worker : Interfaces.IBotWorker
    {
        global::Telegram.Bot.TelegramBotClient client;
        DataBase.BotDataContext dataBase;
        RPTApi.Helpers.Config config;
        RuPostApi postApi;
        public async Task<bool> DeleteOrder(string barcode,int userID)
        {
            var orderToDelete = dataBase.Users.FirstOrDefault(u=>u.Id==userID).Orders.FirstOrDefault(o => o.Barcode == barcode);
            if (orderToDelete != null)
            {
                dataBase.Orders.Remove(orderToDelete);
                await dataBase.SaveChangesAsync();
                return true;
            }
            return false;
        }
        public async Task<bool> RenameOrder(string barcode,string newName,int userId)
        {
            var orderToRename = dataBase.Users.FirstOrDefault(u => u.Id == userId).Orders.FirstOrDefault(o => o.Barcode == barcode);
            if (orderToRename != null)
            {
                dataBase.Update(orderToRename);
                orderToRename.Name = newName;
                await dataBase.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<DataBase.Models.Order> GetOrderAsync(string barcode, int userId)
        {
            var contains = dataBase.Orders.Any(x => x.Barcode == barcode);
            if (contains)
            {
                var order = dataBase.Orders.FirstOrDefault(x => x.Barcode == barcode);
                if (DateTime.Now - order.LastQuerry < TimeSpan.FromHours(12))
                {
                    order.LastQuerry = DateTime.Now;
                    return order;
                }
                else
                {
                    return await RefreshOrderInfoAsync(barcode);
                }
            }
            else
            {
                return await GetNewOrderAsync(barcode, userId);
            }
        }

        public async Task RegisterNewUser(int id)
        {
            dataBase.Users.Add(new DataBase.Models.BotUser(){ Id = id });
            await dataBase.SaveChangesAsync();
        }

        public Worker(string cfgFilePath)
        {
            config = RPTApi.Helpers.ConfigLoader.GetConfig(cfgFilePath);
            client = new global::Telegram.Bot.TelegramBotClient(config.BotKey);
            dataBase = new DataBase.BotDataContext(config);
            postApi = new RuPostApi();
            postApi.AuthorizeAsync(config.RuPostApiLogin, config.RuPostApiPassword).Wait();
        }

        public Worker(RPTApi.Helpers.Config config)
        {
            //client = new TelegramBotClient(config.BotKey);
            dataBase = new DataBase.BotDataContext(config);
            postApi = new RuPostApi();
            postApi.AuthorizeAsync(config.RuPostApiLogin, config.RuPostApiPassword).Wait();
        }
        public void Start()
        {
            //TODO:IMPLIMENT
            client.OnMessage += Client_OnMessage;
        }

        public async Task SendUserMessage(int UserId, string message)
        {
            await client.SendTextMessageAsync(UserId, message);
        }
        private void Client_OnMessage(object sender, global::Telegram.Bot.Args.MessageEventArgs e)
        {
            if(e.Message.Type == global::Telegram.Bot.Types.Enums.MessageType.Text)
            {

            }
        }
        #region Methods-Helpers
        private async Task<DataBase.Models.Order> GetNewOrderAsync(string barcode,int userId)
        {
            var orderHistory = await postApi.GetOperationsHistoryAsync(barcode);
            var newOrder = new DataBase.Models.Order()
            {
                Barcode = barcode,
                LastQuerry = DateTime.Now,
                StartTracking = DateTime.Now,
                UserId = userId 
            };

            dataBase.Orders.Add(newOrder);
            await dataBase.SaveChangesAsync();
          
            newOrder = dataBase.Orders.FirstOrDefault(o => o.Barcode == barcode);
            var recs = new List<DataBase.Models.Record>();
            foreach (var record in orderHistory.OperationHistoryData)
            {
                var newRecord = new DataBase.Models.Record()
                {
                    DateTime = record.OperationParameters.OperDate,
                    Location = record.AddressParameters.OperationAddress.Description,
                    OperationType = record.OperationParameters.OperType.Name,
                    OperationAttribute = record.OperationParameters.OperAttr?.Name,
                    OrderBarcode = barcode
                };
                // Avoiding of copying
                if(!recs.Any(r=>r.DateTime==newRecord.DateTime && r.Location == newRecord.Location))
                    recs.Add(newRecord);
            }
            dataBase.AddRange(recs);
            await dataBase.SaveChangesAsync();

            return dataBase.Orders.FirstOrDefault(o => o.Barcode == barcode);
        }

        private async Task<DataBase.Models.Order> RefreshOrderInfoAsync(string barcode)
        {
            var history = await postApi.GetOperationsHistoryAsync(barcode);
            var recs = new List<DataBase.Models.Record>();
            recs.AddRange(dataBase.Orders.FirstOrDefault(o => o.Barcode == barcode).Records);
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
            dataBase.AddRange(recs);
            await dataBase.SaveChangesAsync();
            return dataBase.Orders.FirstOrDefault(o => o.Barcode == barcode);
        }
        #endregion
    }
}