using System.Collections.Generic;

namespace Predict.Helper
{
    public class LeagueTable
    {
        public short LeagueId { get; set; }
        public string LeagueName { get; set; }
        public List<LeagueTableTeam> LeagueTableTeams { get; set; }
    }
}