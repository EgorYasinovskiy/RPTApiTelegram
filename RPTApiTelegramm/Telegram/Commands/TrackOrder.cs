using RPTApi.Telegram.Helpers;
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
                var order = await worker.GetOrderAsync(args[0], userId);
                if (order == null)
                {
                    await worker.SendUserMessage(userId, "Я не смог отследить твою посылку по этому трек-номеру, видимо его нет в базе Почты России.");
                    return;
                }
                await worker.SendUserMessage(userId, order.ToMessage(worker.DataBase));
            }
            else
            {
                await worker.SendUserMessage(userId, 
                    "Укажи какую именно посылку ты хочешь отследить. /track {трек-номер} или /track {имя}. Либо нажми на кнопки ниже, тут показаны все твои посылки.",
                    Keyboards.TrackReplyKeyboard.Get(userId,worker.DataBase));
            }
        }
    }
}
