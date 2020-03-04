using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Predict.Models;

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