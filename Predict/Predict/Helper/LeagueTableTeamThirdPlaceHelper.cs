using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Predict.Helper
{
    public class LeagueTableTeamThirdPlaceHelper : LeagueTableTeam
    {
        public LeagueTableTeamThirdPlaceHelper(LeagueTableTeam leagueTableTeam)
        {
            TeamId = leagueTableTeam.TeamId;
            Team = leagueTableTeam.Team;
            Played = leagueTableTeam.Played;
            Won = leagueTableTeam.Won;
            Draws = leagueTableTeam.Draws;
            Lost = leagueTableTeam.Lost;
            GoalsFor = leagueTableTeam.GoalsFor;
            GoalsAgainst = leagueTableTeam.GoalsAgainst;
            TeamFlag = leagueTableTeam.TeamFlag;
            Points =  leagueTableTeam.Points;
            Position  = leagueTableTeam.Position;
        }
        public short LeagueId { get; set; }
        public string LeagueName { get; set; }
    }
}