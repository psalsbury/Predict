using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Predict.Models;

namespace Predict.Helper
{
    public static class LeagueTableHelper
    {

        public static Dictionary<int, int> FetchFirstRoundTeamsForAutoFill(int eventId, string userId)
        {

            var context = new ApplicationDbContext();
            var resultingDictionary = new Dictionary<int, int>();

            // Work out, based on the group predictions, the teams that would route through to the first KO round
            var leagueTables = FetchLeagueTablesByUserId(eventId, userId);

            var koFixtures = context.KoFixtures.Where(k => k.EventId == eventId);
            var maxRoundOf = koFixtures.Max(p => p.RoundOf);

            foreach (var koFixture in koFixtures.Where(k => k.RoundOf == maxRoundOf).OrderBy(p => p.Position))
            {
                var team1FromLeague = koFixture.Team1FromLeague;
                var team1FromLeaguePosition = koFixture.Team1FromLeaguePosition ?? default(int);
                var team2FromLeague = koFixture.Team2FromLeague;
                var team2FromLeaguePosition = koFixture.Team2FromLeaguePosition ?? default(int);

                var team1 = team1FromLeague.Length == 1
                    ? GetTeamFromPosition(leagueTables, team1FromLeague, team1FromLeaguePosition)
                    : GetBestPlacedThirdPosition(leagueTables, team1FromLeague, team1FromLeaguePosition);
                var team2 = team2FromLeague.Length == 1
                    ? GetTeamFromPosition(leagueTables, team2FromLeague, team2FromLeaguePosition)
                    : GetBestPlacedThirdPosition(leagueTables, team2FromLeague, team2FromLeaguePosition);

                resultingDictionary.Add(((koFixture.Position-1)*2)+1, team1.TeamId);
                resultingDictionary.Add(((koFixture.Position-1)*2)+2, team2.TeamId);
            }

            return resultingDictionary;

        }

        public static LeagueTableTeam GetTeamFromPosition(List<LeagueTable> leagueTables, string teamFromLeague, int teamFromLeaguePosition)
        {
            var teamLeagueTable = leagueTables.Find(l => l.League == teamFromLeague);
            return teamLeagueTable.LeagueTableTeams.Find(t => t.Position == teamFromLeaguePosition);
        }

        public static LeagueTableTeam GetBestPlacedThirdPosition(List<LeagueTable> leagueTables, string teamFromLeague, int teamFromLeaguePosition)
        {

            //TODO This will all need updating once the qualifing is understood
            var nbrLeagues = teamFromLeague.Length;
            var firstLeague = teamFromLeague[nbrLeagues-1];

            var teamLeagueTable = leagueTables.Find(l => l.League == firstLeague.ToString());
            return teamLeagueTable.LeagueTableTeams.Find(t => t.Position == teamFromLeaguePosition);

        }

        public static Dictionary<int,int> FetchAllTeamsInOrder(int eventId, string userId)
        {

            var allLeagueTeams = new List<LeagueTableTeam>();
            var leagueTables = FetchLeagueTablesByUserId(eventId, userId);
            var result = new Dictionary<int, int>();

            foreach (var leagueTable in leagueTables)
            {
                foreach (var leagueTableTeam in leagueTable.LeagueTableTeams)
                {
                    allLeagueTeams.Add(leagueTableTeam);
                }
            }

            allLeagueTeams = allLeagueTeams.OrderByDescending(c => c.Points)
                            .ThenByDescending(e => e.GoalDifference)
                            .ThenByDescending(e => e.GoalsFor).ToList();

            Int16 position = 0;
            foreach (var leagueTeam in allLeagueTeams)
            {
                position += 1;
                result.Add(position,leagueTeam.TeamId);
            }

            return result;

        }

        public static List<LeagueTable> FetchLeagueTablesFromResults(int eventId)
        {
            var context = new ApplicationDbContext();
            var eventTeams = GetEventTeams(context, eventId);
            var leagueTables = GetLeagueTables(context, eventTeams, eventId);

            var fixtures = context.Fixtures
                .Where(p => p.EventId == eventId && p.AwayResult != null);

            foreach (var fixture in fixtures)
            {
                var homeTeamId = fixture.HomeTeamId;
                var awayTeamId = fixture.AwayTeamId;
                var eventTeamHome = eventTeams.First(t => t.TeamId == homeTeamId);
                var eventTeamAway = eventTeams.First(t => t.TeamId == awayTeamId);

                if (eventTeamHome.League == eventTeamAway.League) // This should always be true
                {
                    var league = eventTeamHome.League;
                    var leagueTable = leagueTables.First(l => l.League == league);

                    var leagueTableTeamHome = leagueTable.LeagueTableTeams.First(t => t.TeamId == homeTeamId);
                    var leagueTableTeamAway = leagueTable.LeagueTableTeams.First(t => t.TeamId == awayTeamId);

                    leagueTableTeamHome =
                        UpdateLeagueTableTeam(leagueTableTeamHome, fixture.HomeResult ?? default(short), fixture.AwayResult ?? default(short));

                    leagueTableTeamAway =
                        UpdateLeagueTableTeam(leagueTableTeamAway, fixture.HomeResult ?? default(short), fixture.AwayResult ?? default(short));

                }

            }

            return SortTables(leagueTables);
        }

        public static List<LeagueTable> FetchLeagueTablesByUserId(int eventId, string userId)
        {
            var context = new ApplicationDbContext();
            var eventTeams = GetEventTeams(context, eventId);
            var leagueTables = GetLeagueTables(context, eventTeams, eventId);

            var fixturePredictions = context.FixturePredictions
                .Include(p => p.Fixture)
                .Where(p => p.PlayerId == userId && p.Fixture.EventId == eventId);

            foreach (var fixturePrediction in fixturePredictions)
            {
                var homeTeamId = fixturePrediction.Fixture.HomeTeamId;
                var awayTeamId = fixturePrediction.Fixture.AwayTeamId;

                var eventTeamHome = eventTeams.First(t => t.TeamId == homeTeamId);
                var eventTeamAway = eventTeams.First(t => t.TeamId == awayTeamId);

                if (eventTeamHome.League == eventTeamAway.League) // This should always be true
                {
                    var league = eventTeamHome.League;
                    var leagueTable = leagueTables.First(l => l.League == league);

                    var leagueTableTeamHome = leagueTable.LeagueTableTeams.First(t => t.TeamId == homeTeamId);
                    var leagueTableTeamAway = leagueTable.LeagueTableTeams.First(t => t.TeamId == awayTeamId);

                    leagueTableTeamHome =
                        UpdateLeagueTableTeam(leagueTableTeamHome, fixturePrediction.HomePrediction ?? default(short), fixturePrediction.AwayPrediction ?? default(short));

                    leagueTableTeamAway =
                        UpdateLeagueTableTeam(leagueTableTeamAway, fixturePrediction.AwayPrediction ?? default(short), fixturePrediction.HomePrediction ?? default(short));
                }
            }

            return SortTables(leagueTables);

        }


        private static LeagueTableTeam UpdateLeagueTableTeam(LeagueTableTeam leagueTableTeam, short goalsFor, short goalsAgainst)
        {
            leagueTableTeam.Played += 1;
            leagueTableTeam.GoalsFor += goalsFor;
            leagueTableTeam.GoalsAgainst += goalsAgainst;

            if (goalsFor > goalsAgainst)
            {
                // Team Won
                leagueTableTeam.Won += 1;
                leagueTableTeam.Points += 3;
            }
            else if (goalsFor < goalsAgainst)
            {
                // Team Lost
                leagueTableTeam.Lost += 1;

            }
            else if (goalsFor == goalsAgainst)
            {
                // Draw
                leagueTableTeam.Draws += 1;
                leagueTableTeam.Points += 1;
            }

            return leagueTableTeam;

        }

        private static List<LeagueTable> SortTables(List<LeagueTable> leagueTables)
        {
            leagueTables = leagueTables.OrderBy(l => l.League).ToList();

            foreach (var leagueTable in leagueTables)
            {
                leagueTable.LeagueTableTeams = leagueTable.LeagueTableTeams.OrderByDescending(c => c.Points)
                    .ThenBy(e => e.GoalDifference)
                    .ThenByDescending(e => e.GoalDifference)
                    .ThenByDescending(e => e.GoalsFor).ToList();

                short position = 1;

                foreach (var leagueTableTeam in leagueTable.LeagueTableTeams)
                {
                    leagueTableTeam.Position = position;
                    position += 1;
                }
            }

            return leagueTables;
        }

        private static List<EventTeam> GetEventTeams(ApplicationDbContext context, int eventId)
        {
            var eventTeams = context.EventTeams
                .Include(t => t.Team)
                .Where(p => p.EventId == eventId).ToList();

            return eventTeams;
        }

        private static List<LeagueTable> GetLeagueTables(ApplicationDbContext context, List<EventTeam> eventTeams, int eventId)
        {

            var leagueTables = new List<LeagueTable>();

            foreach (var eventTeam in eventTeams)
            {
                var league = eventTeam.League;
                var currentLeagueTable = leagueTables.FirstOrDefault(e => e.League == league);
                if (currentLeagueTable == null)
                {
                    var leagueTable = new LeagueTable
                    {
                        League = league
                        ,LeagueTableTeams = new List<LeagueTableTeam>()
                    };
                    leagueTable.LeagueTableTeams.Add(NewLeagueTableTeam(eventTeam.Team.TeamName, eventTeam.TeamId, eventTeam.Team.FlagFileLocation));

                    leagueTables.Add(leagueTable);
                }
                else
                {
                    currentLeagueTable.LeagueTableTeams.Add(NewLeagueTableTeam(eventTeam.Team.TeamName, eventTeam.TeamId, eventTeam.Team.FlagFileLocation));
                }
            }

            return leagueTables;
        }


        private static LeagueTableTeam NewLeagueTableTeam(string teamName, int teamId, string teamFlag)
        {
            var leagueTableTeam = new LeagueTableTeam
            {
                Team = teamName,
                TeamId = teamId,
                TeamFlag = teamFlag,
                Draws = 0,
                GoalsAgainst = 0,
                GoalsFor = 0,
                Lost = 0,
                Played = 0,
                Position = 1,
                Won = 0
            };
            return leagueTableTeam;
        }


    }
}