using Telegram.Bot;

namespace RPTApi.Telegram.Commands
{
    public interface ICommand
    {
        public string Name { get; set; }
        public bool IsCommand(string messageText);
        public void Execute(string messsageText,TelegramBotClient client);
    }
}
