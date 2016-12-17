using FTrainBot.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTrainBot.Format {
    public static class ListTrainRunningTimeExtension {
        public static string Format(this IList<TrainRunningTime> self, string format) {
            switch (format) {
                case "$plain":
                    {
                        var result = new StringBuilder();
                        foreach(var train in self) {
                            result.AppendLine($"{train.Format(format)}");
                        }
                        return $"{result}";
                    }
                default:
                    throw new ArgumentOutOfRangeException($"Unexpected {self.GetType()} format");
            }
        }
    }
}
