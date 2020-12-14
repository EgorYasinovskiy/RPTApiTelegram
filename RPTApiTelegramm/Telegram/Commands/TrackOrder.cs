using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;

namespace RPTApi.Telegram.Commands
{
    public class TrackOrder : ICommand
    {
        public static string Name => "/track";
        public async Task Execute(string messsageText, Worker worker, int userId)
        {
            var args = messsageText.Replace(Name, string.Empty).Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (args.Length == 1)
            {
                ////if (await worker.GetOrderAsync(args[0],userId))
                ////    await worker.SendUserMessage(userId, "Исполнено!");
                ////else
                ////    await worker.SendUserMessage(userId, $"Я не могу найти трек-номер {args[0]} в списке твоих посылок. Ты уверен что не ошибся?");
            }
            else
            {
                await worker.SendUserMessage(userId, "Укажи какой именно трек-номер ты хочешь переименовать");
            }
        }
    }
}
