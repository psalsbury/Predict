using Predict.Helper;
using Predict.Models;
using System.Collections.Generic;

namespace Predict.ViewModels
{
    public class ActualTeam
    {
        public int RoundOf { get; set; }
        public int TeamId { get; set; }
    }
    public class KoFixturePredictionViewModel
    {
        public struct ActualTeam
        {
            public int RoundOf;
            public int TeamId;
        }
        public Dictionary<int, int> FirstStageAutoFill;
        public Dictionary<int, int> RankedTeamsForAutoFill;
        public List<Predict.ViewModels.ActualTeam> ActualTeams;

        public KoFixturePredictionViewModel()
        {
            Predictions = true;
        }

        public short KoStageFirstRoundQty { get; set; }
        public short EventId { get; set; }
        public List<KoFixturePrediction> KoFixturePredictions { get; set; }
        public KoWinningTeamPrediction KoWinningTeam { get; set; }
        public List<LeagueSubLeagueTeam> LeagueSubLeagueTeams { get; set; }
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