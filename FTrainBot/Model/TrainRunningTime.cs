using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTrainBot.Model {
    public class TrainRunningTime {
        public string DepartureStation { get; set; }
        public string DestinationStation { get; set; }
        public DateTime DepartureTime { get; set; }
        public string TrafficCondition { get; set; }
    }
}
