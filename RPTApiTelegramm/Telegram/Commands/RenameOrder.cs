using System;
using System.Collections.Generic;
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
            if (args.Length == 2)
            {
                if (await worker.RenameOrder(args[0], args[1], userId))
                    await worker.SendUserMessage(userId, "Исполнено!");
                else
                    await worker.SendUserMessage(userId, $"Я не могу найти трек-номер {args[0]} в списке твоих посылок. Ты уверен что не ошибся?");
            }
            else
            {
                await worker.SendUserMessage(userId, "Укажи какой именно трек-номер ты хочешь переименовать");
            }
        }
    }
}
