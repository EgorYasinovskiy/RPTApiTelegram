using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot;

namespace RPTApi.Telegram.Commands
{
    public class Track : ICommand
    {
        public string Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Execute(string messsageText,TelegramBotClient client)
        {
            throw new NotImplementedException();
        }

        public bool IsCommand(string messageText)
        {
            throw new NotImplementedException();
        }
    }
}
