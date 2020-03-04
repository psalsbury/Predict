using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Predict.Models;

namespace Predict.ViewModels
{
    public class KoFixturePredictionViewModel
    {

        public KoFixturePredictionViewModel()
        {
            Predictions = true;
        }

        public List<KoFixturePrediction> KoFixturePredictions { get; set; }
        public KoWinningTeamPrediction KoWinningTeam { get; set; }
        public List<Team> Teams { get; set; }
        public int MaxRows { get; set; }
        public int MaxCols { get; set; }
        public bool ReadOnly { get; set; }
        public bool Predictions { get; set; } // if false then used for results

        public Dictionary<int, int> FirstStageAutoFill;
        public Dictionary<int, int> RankedTeamsForAutoFill;

        public int Power2(int exponent)
        {
            var result = 1;
            for (var power = 2; power <= exponent; power++)
            {
                result = result * 2;
            }
            return result;
        }
    }
}