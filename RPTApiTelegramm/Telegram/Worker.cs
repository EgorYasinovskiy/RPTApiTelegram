using System;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
namespace RPTApi.Telegram
{
    public class Worker : Interfaces.IBotWorker
    {
        TelegramBotClient client;
        DataBase.BotDataContext dataBase;
        Helpers.Config config;
        RuPostApi postApi;
        public async Task DeleteOrder(string barcode)
        {
            var orderToDelete = dataBase.Orders.FirstOrDefault(o => o.Barcode == barcode);
            if (orderToDelete != null)
            {
                dataBase.Orders.Remove(orderToDelete);
                await dataBase.SaveChangesAsync();
            }
        }

        public async Task<DataBase.Models.Order> GetOrder(string barcode, int userId)
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
            config = Helpers.ConfigLoader.GetConfig(cfgFilePath);
            client = new TelegramBotClient(config.BotKey);
            dataBase = new DataBase.BotDataContext(config);
            postApi = new RuPostApi(config.RuPostApiLogin, config.RuPostApiPassword);
        }
        #region Methods-Helpers
        private async Task<DataBase.Models.Order> GetNewOrderAsync(string barcode,int userId)
        {
            var orderHistory = await postApi.GetOperationsHistoryAsync(barcode);
            var newOrder = new DataBase.Models.Order() 
            { 
                LastQuerry = DateTime.Now,
                StartTracking = DateTime.Now,
                UserId = userId 
            };

            dataBase.Orders.Add(newOrder);
            await dataBase.SaveChangesAsync();

            newOrder = dataBase.Orders.FirstOrDefault(o => o.Barcode == barcode);
            foreach (var record in orderHistory.OperationHistoryData)
            {
                var newRecord = new DataBase.Models.Record()
                {
                    DateTime = record.OperationParameters.OperDate,
                    Location = record.AddressParameters.OperationAddress.Description,
                    OrderBarcode = barcode
                };

                dataBase.Records.Add(newRecord);
            }
            await dataBase.SaveChangesAsync();

            return dataBase.Orders.FirstOrDefault(o => o.Barcode == barcode);
        }
        private async Task<DataBase.Models.Order> RefreshOrderInfoAsync(string barcode)
        {
            //var order = dataBase.Orders.FirstOrDefault(x => x.Barcode == barcode);
            
            var history = await postApi.GetOperationsHistoryAsync(barcode);
            foreach (var rec in history.OperationHistoryData)
            {
                var date = rec.OperationParameters.OperDate;
                var location = rec.AddressParameters.OperationAddress.Description;

                if (!dataBase.Records.Any(r => r.OrderBarcode==barcode && r.DateTime == date))
                {
                    var newRecord = new DataBase.Models.Record() { DateTime = date, Location = location, OrderBarcode = barcode};
                    dataBase.Records.Add(newRecord);
                }
            }
            await dataBase.SaveChangesAsync();
            return dataBase.Orders.FirstOrDefault(o => o.Barcode == barcode);
        }
        #endregion
    }
}