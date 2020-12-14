using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telegram.Bot.Types.ReplyMarkups;

namespace RPTApi.Telegram.Keyboards
{
    class RemoveReplyKeyboard
    {
        public static IReplyMarkup Get(int userId, DataBase.BotDataContext dataBase)
        {
            List<InlineKeyboardButton> tracks = new List<InlineKeyboardButton>();
            foreach (var order in dataBase.Users.FirstOrDefault(u => u.Id == userId).Orders)
            {
                tracks.Add(new InlineKeyboardButton() { Text = $"{order.Name ?? order.Barcode}", CallbackData = $"/remove {order.Name ?? order.Barcode}" });
            }
            return new InlineKeyboardMarkup(tracks);
        }
    }
}
