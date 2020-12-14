using System.Collections.Generic;

namespace RPTApi.Telegram.Keyboards
{
    public static class CommandsKeyboard
    {
        private static global::Telegram.Bot.Types.ReplyMarkups.ReplyKeyboardMarkup ReplyKeyboardMarkup = new global::Telegram.Bot.Types.ReplyMarkups.ReplyKeyboardMarkup()
        {
            Keyboard = new List<List<global::Telegram.Bot.Types.ReplyMarkups.KeyboardButton>>()
            {
                new List<global::Telegram.Bot.Types.ReplyMarkups.KeyboardButton>()
                {
                    new global::Telegram.Bot.Types.ReplyMarkups.KeyboardButton()
                    {
                        Text = "/track"
                    },
                    new global::Telegram.Bot.Types.ReplyMarkups.KeyboardButton()
                    {
                        Text = "/rename"
                    },
                    new global::Telegram.Bot.Types.ReplyMarkups.KeyboardButton()
                    {
                        Text = "/remove"
                    }

                }
            },
            ResizeKeyboard = true
        };
        public static global::Telegram.Bot.Types.ReplyMarkups.IReplyMarkup Get()
        {
            return ReplyKeyboardMarkup;
        }
      
    }
}
