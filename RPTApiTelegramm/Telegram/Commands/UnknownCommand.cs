using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;

namespace RPTApi.Telegram.Commands
{
    public class Unknown : ICommand
    {
        public static string Name => "Неизвестная команда";
        public async Task Execute(string messsageText, Worker worker,int userId)
        {
            await worker.SendUserMessage(userId, "Я не совсем тебя понимаю, список доступных команд доступен через /help.");
        }
    }
}
