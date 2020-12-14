using System.Threading.Tasks;

namespace RPTApi.Telegram.Commands
{
    public class Help : ICommand
    {
        public static string Name => "/help";
        public async Task Execute(string messsageText, Worker worker, int userId)
        {
            await worker.SendUserMessage(userId, "Список доступных комманд:\n\\track - Отследить посылку\n\\rename - переименовать посылку в сохраненных\n\\remove - удалить посылку из сохраненных");
        }
    }
}
