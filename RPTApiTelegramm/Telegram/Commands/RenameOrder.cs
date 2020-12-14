using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;

namespace RPTApi.Telegram.Commands
{
    public class RenameOrder : ICommand
    {
        public static string Name => "/rename";
        public async Task Execute(string messsageText, Worker worker,int userId)
        {
            var args = messsageText.Replace(Name, string.Empty).Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (args.Length >= 2)
            {

                if (worker.DataBase.Users.FirstOrDefault(u => u.Id == userId).Orders.Any(o => o.Barcode == args[0] || o.Name == args[0]))
                {
                    var barcode = worker.DataBase.Users.FirstOrDefault(u => u.Id == userId).Orders.FirstOrDefault(o => o.Barcode == args[0] || o.Name == args[0]);
                    await worker.RenameOrder(args[0], args[1], userId);
                    await worker.SendUserMessage(userId, "Исполнено!");
                }
                else
                {
                    await worker.SendUserMessage(userId, $"Я не могу найти трек-номер или имя {args[0]} в списке твоих посылок. Ты уверен что не ошибся?");
                }
            }
            else if (args.Length == 1)
            {
                await worker.SendUserMessage(userId, $"Укажи новоя имя для этой посылки. Напиши /rename {args[0]} {{Новое_имя}}");
            }
            else
            {
                await worker.SendUserMessage(userId, "Укажи предыдущее имя посылки или ее трек-номер и новоя имя для этой посылки. Напиши /rename {Старое_имя_или_трек-номер} {Новое_имя}");
            }
        }
    }
}
