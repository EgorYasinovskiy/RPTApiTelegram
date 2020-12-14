using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPTApi.Telegram.Helpers
{
    public static class OrderToMessageExtension
    {
        public static string ToMessage(this DataBase.Models.Order order,DataBase.BotDataContext context)
        {
            var records = context.Records.Where(r => r.OrderBarcode == order.Barcode);
            string message = "";
            foreach(var rec in records.OrderBy(r => r.DateTime))
            {
                var edge = "####################\n";
                message += $"{edge}{rec.DateTime: dd/MM/yyyy HH:mm}\n{rec.Location}\n{rec.OperationAttribute ?? rec.OperationType}\n{edge}\n";
            }
            return message;
        }
    }
}
