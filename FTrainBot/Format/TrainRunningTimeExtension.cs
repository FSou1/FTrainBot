using FTrainBot.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTrainBot.Format {
    public static class TrainRunningTimeExtension {
        public static string Format(this TrainRunningTime self, string format) {
            switch (format) {
                case "$plain":
                    {
                        var result = new StringBuilder();
                        result.Append($"{self.DepartureTime.ToShortTimeString()} | ");
                        result.Append($"{self.TrafficCondition} | ");
                        result.Append($"{self.DepartureStation} → ");
                        result.Append($"{self.DestinationStation} ");
                        return $"{result}";
                    }
                default:
                    throw new ArgumentOutOfRangeException($"Unexpected {self.GetType()} format");
            }
        }
    }
}
