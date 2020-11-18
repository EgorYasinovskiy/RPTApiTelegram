using System;
using System.Collections.Generic;
using System.Text;

namespace RPTApiTelegram.DataBase.Models
{
    public class BotUser
    {
        public int Id { get; set; } // Inherits from TelegramUser.
        public virtual List<Order> Orders { get; set; }
    }
}
