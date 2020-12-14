using System.Threading.Tasks;
using Telegram.Bot;

namespace RPTApi.Telegram.Commands
{
    public interface ICommand
    {
        public static string Name { get; }
        public Task Execute(string messsageText, Worker worker,int userId);
    }
}
