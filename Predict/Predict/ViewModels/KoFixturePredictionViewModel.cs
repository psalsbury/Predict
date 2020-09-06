using System.Collections.Generic;
using Predict.Helper;
using Predict.Models;

namespace Predict.ViewModels
{
    public class KoFixturePredictionViewModel
    {
        public Dictionary<int, int> FirstStageAutoFill;
        public Dictionary<int, int> RankedTeamsForAutoFill;

        public KoFixturePredictionViewModel()
        {
            Predictions = true;
        }

        public short EventId { get; set; }
        public List<KoFixturePrediction> KoFixturePredictions { get; set; }
        public KoWinningTeamPrediction KoWinningTeam { get; set; }
        public List<EventTeam> EventTeams { get; set; }
        public int MaxRows { get; set; }
        public int MaxCols { get; set; }
        public bool ReadOnly { get; set; }
        public bool Predictions { get; set; } // if false then used for results
        public bool IsPremiumPlayer { get; set; }

        public int Power2(int exponent)
        {
            var result = 1;
            for (var power = 2; power <= exponent; power++) result = result * 2;
            return result;
        }
    }
}