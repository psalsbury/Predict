using System.Collections.Generic;

namespace Predict.ViewModels
{
    public class StatsKoViewModel
    {
        public List<StatsKoRoundOf> StatsKoRoundOfs { get; set; }
    }

    public class StatsKoRoundOf
    {
        public int RoundOf { get; set; }
        public int TeamId { get; set; }
        public string TeamName { get; set; }
        public int NumberOfPredictions { get; set; }
    }
}