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
        private readonly Dictionary<string, TrainRoute> dict = new Dictionary<string, TrainRoute>() {
            { "От «Лобня» до «Савёловская»",  new TrainRoute(StationEnum.Lobnya, StationEnum.Savelovskaya) },
            { "От «Лобня» до «Белорусская»",  new TrainRoute(StationEnum.Lobnya, StationEnum.Belorusskaya) },
            { "От «Тимирязевская» до «Лобня»", new TrainRoute(StationEnum.Timiryazevskaya, StationEnum.Lobnya) },            
            { "От «Савёловская» до «Лобня»", new TrainRoute(StationEnum.Savelovskaya, StationEnum.Lobnya) },
            { "От «Белорусская» до «Лобня»", new TrainRoute(StationEnum.Belorusskaya, StationEnum.Lobnya) },
        };

        public MessageHandler(ITrainRunningTimeApi client) {
            _client = client;
        }

        public async Task HandleMessage(TelegramBotClient bot, Message message) {
            if (message == null || message.Type != MessageType.TextMessage)
                return;

            if(dict.ContainsKey(message.Text)) {
                var output = await FetchNextFiveTrainRunningTimes(dict[message.Text]);

                await bot.SendTextMessageAsync(message.Chat.Id, output, replyMarkup: keyboard);
            }
            else {
                await bot.SendTextMessageAsync(message.Chat.Id, "Какое направление интересует?", replyMarkup: keyboard);
            }
        }

        private async Task<string> FetchNextFiveTrainRunningTimes(TrainRoute route) {
            try {
                var moscowNow = DateTimeUtility.GetMoscowDateTime();
                var trains = await _client.Fetch(route.DepartureStation, route.DestinationStation, moscowNow, 10);
                if(trains.Count == 0) {
                    return "На сегодня Все электрички закончились";
                }

                return trains.Format("$plain");
            }
            catch (Exception ex) {
                return $"Не удалось обработать запрос. Причина: {ex.Message}";
            }            
        }

        private static ReplyKeyboardMarkup keyboard = new ReplyKeyboardMarkup(new[] {
            new [] {
                new KeyboardButton("От «Лобня» до «Савёловская»"),
                new KeyboardButton("От «Лобня» до «Белорусская»"),
            },
            new [] {
                new KeyboardButton("От «Тимирязевская» до «Лобня»"),
                new KeyboardButton("От «Савёловская» до «Лобня»"),
                new KeyboardButton("От «Белорусская» до «Лобня»"),
            }
        });
    }
}
