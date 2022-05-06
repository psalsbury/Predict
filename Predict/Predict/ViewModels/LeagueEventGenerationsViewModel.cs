using Predict.Models;
using System.Collections.Generic;

namespace Predict.ViewModels
{
    public class LeagueEventGenerationsViewModel
    {
        public League League { get; set; } // Present for when there is no league event generations

        public List<LeagueEventGeneration> LeagueEventGenerations { get; set; }

        public List<RapidApiV3LeagueSeason> RapidApiV3LeagueSeasons { get; set; }

        public List<Team> Teams { get; set; }

    }
}