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
        private static Dictionary<StationEnum, TrainStationDirection> dict = new Dictionary<StationEnum, TrainStationDirection> {
            {
                StationEnum.Lobnya,
                new TrainStationDirection {
                    StationId = 29804,
                    DirectionId = "d1"
                }
            },
            {
                StationEnum.Timiryazevskaya,
                new TrainStationDirection {
                    StationId = 28704,
                    DirectionId = "d9"
                }
            }
        };

        public async Task<IList<TrainRunningTime>> Fetch(StationEnum departureStation, DateTime departureTimeFrom, int count) {
            if (!dict.ContainsKey(departureStation))
                throw new ArgumentException($"Unknown departureStation: {departureStation}");

            var html = await Grab(dict[departureStation]);
            var parsed = await Parse(html);

            return parsed.Where(d=>d.DepartureTime > departureTimeFrom).Take(count).ToList();
        }

        private async Task<string> Grab(TrainStationDirection stationDirection) {
            var url = $"http://www.tutu.ru/station.php?nnst={stationDirection.StationId}&list={stationDirection.DirectionId}&print=yes";
            return await DownloadString(url);
        }

        private async Task<IEnumerable<TrainRunningTime>> Parse(string html) {
            var document = new HtmlDocument();
            document.LoadHtml(html);
            var scheduleTable = document.GetElementbyId("schedule_table");

            return scheduleTable.SelectNodes("tbody/tr").Select(ParseLine);
        }

        private TrainRunningTime ParseLine(HtmlNode node) {
            var routeInfo = node.SelectSingleNode("td[1]/a[1]").InnerText;
            var timeInfo = node.SelectSingleNode("td[3]/div[1]").InnerText;
            var condition = node.SelectSingleNode("td[4]").InnerText;
            var parts = routeInfo.Split('→');

            return new TrainRunningTime {
                DepartureStation = parts[0],
                DestinationStation = parts[1],
                DepartureTime = DateTime.ParseExact(ExtractTimeCharacters(timeInfo), "HH:mm", CultureInfo.InvariantCulture),
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

        internal class TrainStationDirection {
            internal int StationId { get; set; }
            internal string DirectionId { get; set; }
        }
    }
}
