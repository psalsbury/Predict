using System.Collections.Generic;
using Predict.Models;

namespace Predict.Helper
{
    public class LeagueTable
    {
        public LeagueSubLeague LeagueSubLeague { get; set; }
        public List<LeagueSubLeagueTeam> LeagueSubLeagueTeams { get; set; }
        public List<LeagueTableTeam> LeagueTableTeams { get; set; }

    }
}