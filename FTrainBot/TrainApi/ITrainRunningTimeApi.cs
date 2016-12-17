using FTrainBot.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTrainBot.TrainApi {
    public interface ITrainRunningTimeApi {
        Task<IList<TrainRunningTime>> Fetch(StationEnum departureStation, DateTime departureTimeFrom, int count);
    }
}
