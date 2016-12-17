using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTrainBot.Model {
    public class TrainRoute {
        public StationEnum DepartureStation { get; set; }
        public StationEnum DestinationStation { get; set; }

        public TrainRoute(StationEnum departure, StationEnum destination) {
            DepartureStation = departure;
            DestinationStation = destination;
        }
    }
}
