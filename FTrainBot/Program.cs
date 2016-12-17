using FTrainBot.TrainApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace FTrainBot {
    class Program {
        private static readonly TelegramBotClient Bot = 
            new TelegramBotClient("<your bot api key>");

        private static MessageHandler msgHandler = new MessageHandler(new TutuApiClient());

        static void Main(string[] args) {
            Bot.OnMessage += BotOnMessageReceived;
            Bot.OnMessageEdited += BotOnMessageReceived;

            var me = Bot.GetMeAsync().Result;

            Console.Title = me.Username;

            Bot.StartReceiving();
            Console.ReadLine();
            Bot.StopReceiving();
        }

        private static async void BotOnMessageReceived(object sender, MessageEventArgs e) {
            var message = e.Message;

            if (message == null || message.Type != MessageType.TextMessage)
                return;

            await msgHandler.HandleMessage(Bot, message);
        } 
    }
}
