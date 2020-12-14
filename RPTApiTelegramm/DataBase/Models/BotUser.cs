using System.Collections.Generic;

namespace RPTApi.DataBase.Models
{
    public class BotUser
    {
        public int Id { get; set; } // Inherits from TelegramUser.
        public virtual List<Order> Orders { get; set; }
        public BotUser()
        {
            Orders = new List<Order>();
        }
    }
}
