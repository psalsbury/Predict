using System;
using System.Collections.Generic;
using Predict.Models;
using System.Web;

namespace Predict.ViewModels
{
    public class LeagueEventGenerationsEditViewModel
    {

        public LeagueEventGeneration LeagueEventGeneration { get; set; }

        public List<RapidApiV3LeagueSeason> RapidApiV3LeagueSeasons { get; set; }

        public List<Team> Teams { get; set; }

        public List<GenerationType> GenerationTypes { get; set; }

    }

    public class GenerationType
    {
        public int Id { get; set; }
        public string GenerationTypeName { get; set; }
    }
}
