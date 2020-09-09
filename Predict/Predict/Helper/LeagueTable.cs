using System.Collections.Generic;
using Predict.Models;

namespace Predict.Helper
{
    public class LeagueTable
    {
        public short LeagueId { get; set; }
        public string LeagueName { get; set; }
        public List<LeagueTableTeam> LeagueTableTeams { get; set; }
    }
}