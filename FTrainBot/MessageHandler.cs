using FTrainBot.Format;
using FTrainBot.Model;
using FTrainBot.TrainApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace FTrainBot {
    public class MessageHandler {
        private readonly ITrainRunningTimeApi _client;

        public MessageHandler(ITrainRunningTimeApi client) {
            _client = client;
        }

        public async Task HandleMessage(TelegramBotClient bot, Message message) {
            if (message == null || message.Type != MessageType.TextMessage)
                return;

            if (message.Text.Contains("От «Тимирязевская» до «Лобня»")) // send custom keyboard
            {
                var output = await FetchNextFiveTrainRunningTimes(StationEnum.Timiryazevskaya);

                await bot.SendTextMessageAsync(message.Chat.Id, output, replyMarkup: keyboard);
            }
            else if (message.Text.Contains("От «Лобня» до «Тимирязевская»")) // send custom keyboard
            {                
                var output = await FetchNextFiveTrainRunningTimes(StationEnum.Lobnya);

                await bot.SendTextMessageAsync(message.Chat.Id, output, replyMarkup: keyboard);
            }
            else {
                await bot.SendTextMessageAsync(message.Chat.Id, "Какое направление интересует?", replyMarkup: keyboard);
            }
        }

        private async Task<string> FetchNextFiveTrainRunningTimes(StationEnum departureStation) {
            try {
                var trains = await _client.Fetch(departureStation, DateTime.Now, 10);

                return trains.Format("$plain");
            }
            catch (Exception ex) {
                return $"Не удалось обработать запрос. Причина: {ex.Message}";
            }            
        }

        private static ReplyKeyboardMarkup keyboard = new ReplyKeyboardMarkup(new[] {
            new [] {
                new KeyboardButton("От «Тимирязевская» до «Лобня»"),
            },
            new [] {
                new KeyboardButton("От «Лобня» до «Тимирязевская»"),
            }
        });
    }
}
