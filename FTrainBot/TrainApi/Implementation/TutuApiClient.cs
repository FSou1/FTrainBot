using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FTrainBot.Model;
using RestSharp;
using HtmlAgilityPack;
using System.Globalization;
using System.Text.RegularExpressions;

namespace FTrainBot.TrainApi {
    public class TutuApiClient : ITrainRunningTimeApi {
        private static Dictionary<StationEnum, int> dict = new Dictionary<StationEnum, int> {
            { StationEnum.Lobnya, 29804 },
            { StationEnum.Timiryazevskaya, 28704 },
            { StationEnum.Savelovskaya, 28604 },
            { StationEnum.Belorusskaya,101 }
        };

        public async Task<IList<TrainRunningTime>> Fetch(
            StationEnum departure, 
            StationEnum destination, 
            DateTime departureTimeFrom, 
            int count
        ) {
            if (!dict.ContainsKey(departure))
                throw new ArgumentException($"Unknown departureStation: {departure}");
            if(!dict.ContainsKey(destination))
                throw new ArgumentException($"Unknown departureStation: {destination}");

            var html = await Grab(dict[departure], dict[destination], departureTimeFrom.Date);
            var parsed = await Parse(html);

            return parsed.Where(d=>d.DepartureTime > departureTimeFrom).Take(count).ToList();
        }

        private async Task<string> Grab(int departureStationId, int destinationStationId, DateTime date) {
            var url = $"http://www.tutu.ru/rasp.php?st1={departureStationId}&st2={destinationStationId}&date={date.ToShortDateString()}&print=yes";
            return await DownloadString(url);
        }

        private async Task<IEnumerable<TrainRunningTime>> Parse(string html) {
            var document = new HtmlDocument();
            document.LoadHtml(html);
            var scheduleTable = document.GetElementbyId("schedule_table");

            return scheduleTable.SelectNodes("tbody/tr").Select(ParseLine);
        }

        private TrainRunningTime ParseLine(HtmlNode node) {
            var departure = node.SelectSingleNode("td[10]/a[1]").InnerText;
            var destination = node.SelectSingleNode("td[12]/a[1]").InnerText;
            var departureTime = node.SelectSingleNode("td[2]/div/a[1]").InnerText;
            var condition = node.SelectSingleNode("td[8]").InnerText;
            var time = ExtractTimeCharacters(departureTime).Substring(0, 5);

            return new TrainRunningTime {
                DepartureStation = departure,
                DestinationStation = destination,
                DepartureTime = DateTime.ParseExact(time, "HH:mm", CultureInfo.GetCultureInfo("ru-RU")),
                TrafficCondition = ExtractCyrillicCharacters(condition)
            };
        }

        private string ExtractTimeCharacters(string str) {
            return Regex.Replace(str, "[^0-9.:]+", "", RegexOptions.Compiled);
        }

        private string ExtractCyrillicCharacters(string str) {
            return Regex.Replace(str, "[^а-яА-Я]+", "", RegexOptions.Compiled);
        }

        private async Task<string> DownloadString(string url) {
            var client = new RestClient(url);
            var request = new RestRequest();
            var response = await client.ExecuteTaskAsync(request);
            return response.Content;
        }
    }
}
