using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;

namespace RPTApi.Telegram.Commands
{
    public class RemoveOrder : ICommand
    {
        public static string Name => "/remove";

        public async Task Execute(string messsageText, Worker worker,int userId)
        {
            messsageText = messsageText.Replace(Name, string.Empty).Trim();
            if (messsageText.Split(' ',StringSplitOptions.RemoveEmptyEntries).Length == 1)
            {
                if (await worker.DeleteOrder(messsageText, userId))
                    await worker.SendUserMessage(userId, "Исполнено!");
                else
                    await worker.SendUserMessage(userId, "Я не нашел среди твоих трек-номеров такого номера");
            }
            else
            {
                await worker.SendUserMessage(userId, "Укажи какой именно трек-номер ты хочешь удалить");
            }
        }
    }
}
