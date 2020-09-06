using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Predict.Models;
using System.Data.SqlClient;

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
                var team1FromLeagueId = koFixture.Team1FromLeagueId; // comma delimeterd list
                List<short> team1FromLeagueIdArray = team1FromLeagueId.Split(',').Select(short.Parse).ToList();
                var firstTeam1FromLeagueId = team1FromLeagueIdArray.First();
                var team1FromLeaguePosition = koFixture.Team1FromLeaguePosition ?? default(int);

                var team2FromLeagueId = koFixture.Team2FromLeagueId; // comma delimeterd list
                List<short> team2FromLeagueIdArray = team2FromLeagueId.Split(',').Select(short.Parse).ToList();
                var firstTeam2FromLeagueId = team2FromLeagueIdArray.First();
                var team2FromLeaguePosition = koFixture.Team2FromLeaguePosition ?? default(int);

                var team1 = team1FromLeagueIdArray.Count() == 1
                    ? GetTeamFromPosition(leagueTables, firstTeam1FromLeagueId, team1FromLeaguePosition)
                    : GetBestPlacedThirdPosition(leagueTables, team1FromLeagueIdArray, team1FromLeaguePosition);
                var team2 = team2FromLeagueId.Split(',').Count() == 1
                    ? GetTeamFromPosition(leagueTables, firstTeam2FromLeagueId, team2FromLeaguePosition)
                    : GetBestPlacedThirdPosition(leagueTables, team2FromLeagueIdArray, team2FromLeaguePosition);

                resultingDictionary.Add((koFixture.Position - 1) * 2 + 1, team1.TeamId);
                resultingDictionary.Add((koFixture.Position - 1) * 2 + 2, team2.TeamId);
            }

            return resultingDictionary;
        }

        public static LeagueTableTeam GetTeamFromPosition(List<LeagueTable> leagueTables, short leagueId,
            int position)
        {
            var teamLeagueTable = leagueTables.Find(l => l.LeagueId == leagueId);
            return teamLeagueTable.LeagueTableTeams.Find(t => t.Position == position);
        }

        public static LeagueTableTeam GetBestPlacedThirdPosition(List<LeagueTable> leagueTables, List<short> leagueIds,
            int position)
        {
            //TODO This will all need updating once the qualifing is understood
            var nbrLeagues = leagueIds.Count;
            var firstLeague = leagueIds.First();

            var teamLeagueTable = leagueTables.Find(l => l.LeagueId == firstLeague);
            return teamLeagueTable.LeagueTableTeams.Find(t => t.Position == position);
        }

        public static Dictionary<int, int> FetchAllTeamsInOrder(int eventId, string userId)
        {
            var allLeagueTeams = new List<LeagueTableTeam>();
            var leagueTables = FetchLeagueTablesByUserId(eventId, userId);
            var result = new Dictionary<int, int>();

            foreach (var leagueTable in leagueTables)
            foreach (var leagueTableTeam in leagueTable.LeagueTableTeams)
                allLeagueTeams.Add(leagueTableTeam);

            allLeagueTeams = allLeagueTeams.OrderByDescending(c => c.Points)
                .ThenByDescending(e => e.GoalDifference)
                .ThenByDescending(e => e.GoalsFor).ToList();

            short position = 0;
            foreach (var leagueTeam in allLeagueTeams)
            {
                position += 1;
                result.Add(position, leagueTeam.TeamId);
            }

            return result;
        }

        public static List<LeagueTable> FetchLeagueTablesFromResults(int eventId)
        {
            var context = new ApplicationDbContext();
            var eventTeams = GetEventTeams(context, eventId);
            var leagueTables = GetLeagueTables(context, eventTeams, eventId);

            var eventFixtures = context.EventFixtures
                .Where(p => p.EventId == eventId && p.Fixture.AwayResult != null);

            foreach (var eventFixture in eventFixtures)
            {
                var homeTeamId = eventFixture.Fixture.HomeTeamId;
                var awayTeamId = eventFixture.Fixture.AwayTeamId;
                var eventTeamHome = eventTeams.First(t => t.TeamId == homeTeamId);
                var eventTeamAway = eventTeams.First(t => t.TeamId == awayTeamId);

                if (eventTeamHome.LeagueId == eventTeamAway.LeagueId) // This should always be true
                {
                    var leagueId = eventTeamHome.LeagueId;
                    var leagueTable = leagueTables.First(l => l.LeagueId == leagueId);

                    var leagueTableTeamHome = leagueTable.LeagueTableTeams.First(t => t.TeamId == homeTeamId);
                    var leagueTableTeamAway = leagueTable.LeagueTableTeams.First(t => t.TeamId == awayTeamId);

                    leagueTableTeamHome =
                        UpdateLeagueTableTeam(leagueTableTeamHome, eventFixture.Fixture.HomeResult ?? default(short),
                            eventFixture.Fixture.AwayResult ?? default(short));

                    leagueTableTeamAway =
                        UpdateLeagueTableTeam(leagueTableTeamAway, eventFixture.Fixture.HomeResult ?? default(short),
                            eventFixture.Fixture.AwayResult ?? default(short));
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
                .Include(p => p.EventFixture)
                .Include(p => p.EventFixture.Fixture)
                .Where(p => p.PlayerId == userId && p.EventFixture.EventId == eventId);

            foreach (var fixturePrediction in fixturePredictions)
            {
                var homeTeamId = fixturePrediction.EventFixture.Fixture.HomeTeamId;
                var awayTeamId = fixturePrediction.EventFixture.Fixture.AwayTeamId;

                var eventTeamHome = eventTeams.First(t => t.TeamId == homeTeamId);
                var eventTeamAway = eventTeams.First(t => t.TeamId == awayTeamId);

                if (eventTeamHome.LeagueId == eventTeamAway.LeagueId) // This should always be true
                {
                    var leagueId = eventTeamHome.LeagueId;
                    var leagueTable = leagueTables.First(l => l.LeagueId == leagueId);

                    var leagueTableTeamHome = leagueTable.LeagueTableTeams.First(t => t.TeamId == homeTeamId);
                    var leagueTableTeamAway = leagueTable.LeagueTableTeams.First(t => t.TeamId == awayTeamId);

                    leagueTableTeamHome =
                        UpdateLeagueTableTeam(leagueTableTeamHome, fixturePrediction.HomePrediction ?? default(short),
                            fixturePrediction.AwayPrediction ?? default(short));

                    leagueTableTeamAway =
                        UpdateLeagueTableTeam(leagueTableTeamAway, fixturePrediction.AwayPrediction ?? default(short),
                            fixturePrediction.HomePrediction ?? default(short));
                }
            }

            return SortTables(leagueTables);
        }


        private static LeagueTableTeam UpdateLeagueTableTeam(LeagueTableTeam leagueTableTeam, short goalsFor,
            short goalsAgainst)
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
            leagueTables = leagueTables.OrderBy(l => l.LeagueId).ToList();

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

        public static List<EventTeam> GetEventTeams(ApplicationDbContext context, int eventId)
        {
            var eventTeams = context.Database.SqlQuery<EventTeam>(
                "spGetTeamsByEvent @intEventId"
                , new SqlParameter("@intEventId", eventId)).ToList();

            return eventTeams;
        }

        private static List<LeagueTable> GetLeagueTables(ApplicationDbContext context, List<EventTeam> eventTeams,int eventId)
        {
            var leagueTables = new List<LeagueTable>();

            foreach (var eventTeam in eventTeams)
            {
                var leagueId = eventTeam.LeagueId;
                var currentLeagueTable = leagueTables.FirstOrDefault(e => e.LeagueId == leagueId);
                if (currentLeagueTable == null)
                {
                    var leagueTable = new LeagueTable
                    {
                        LeagueId = leagueId
                        , LeagueTableTeams = new List<LeagueTableTeam>()
                    };
                    leagueTable.LeagueTableTeams.Add(NewLeagueTableTeam(eventTeam.TeamName, eventTeam.TeamId,
                        eventTeam.FlagFileLocation));

                    leagueTables.Add(leagueTable);
                }
                else
                {
                    currentLeagueTable.LeagueTableTeams.Add(NewLeagueTableTeam(eventTeam.TeamName,
                        eventTeam.TeamId, eventTeam.FlagFileLocation));
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