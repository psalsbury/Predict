using Predict.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

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

            var thirdPlacedTeams = new List<LeagueTableTeamThirdPlaceHelper>();
            var thirdPlaceDictionary = new Dictionary<string, string>(); // koId, TeamId
            foreach (LeagueTable leagueTable in leagueTables)
            {
                var leagueTableTeam = new LeagueTableTeamThirdPlaceHelper(leagueTable.LeagueTableTeams.Find(t => t.Position == 3));
                leagueTableTeam.LeagueId = leagueTable.LeagueSubLeague.Id;
                leagueTableTeam.LeagueName = leagueTable.LeagueSubLeague.SubLeagueName;
                thirdPlacedTeams.Add(leagueTableTeam);
            };

            if (eventId == 1)
            {
                var thirdPlacedTeamsOrdered = thirdPlacedTeams.OrderByDescending(p => p.Points)
                    .ThenByDescending(p => p.GoalDifference)
                    .ThenByDescending(p => p.GoalsFor);

                var strFirstFour = (thirdPlacedTeamsOrdered.ElementAt(0).LeagueName
                                    + thirdPlacedTeamsOrdered.ElementAt(1).LeagueName
                                    + thirdPlacedTeamsOrdered.ElementAt(2).LeagueName
                                    + thirdPlacedTeamsOrdered.ElementAt(3).LeagueName);

                strFirstFour = String.Concat(strFirstFour.OrderBy(c => c));

                // First string is the group where the first placed team where play the 3rd places team from the second string
                if (strFirstFour == "ABCD")
                {
                    thirdPlaceDictionary.Add("B", "A");
                    thirdPlaceDictionary.Add("C", "D");
                    thirdPlaceDictionary.Add("E", "B");
                    thirdPlaceDictionary.Add("F", "C");
                }
                else if (strFirstFour == "ABCE")
                {
                    thirdPlaceDictionary.Add("B", "A");
                    thirdPlaceDictionary.Add("C", "E");
                    thirdPlaceDictionary.Add("E", "B");
                    thirdPlaceDictionary.Add("F", "C");
                }
                else if (strFirstFour == "ABCF")
                {
                    thirdPlaceDictionary.Add("B", "A");
                    thirdPlaceDictionary.Add("C", "F");
                    thirdPlaceDictionary.Add("E", "B");
                    thirdPlaceDictionary.Add("F", "C");
                }
                else if (strFirstFour == "ABDE")
                {
                    thirdPlaceDictionary.Add("B", "D");
                    thirdPlaceDictionary.Add("C", "E");
                    thirdPlaceDictionary.Add("E", "A");
                    thirdPlaceDictionary.Add("F", "B");
                }
                else if (strFirstFour == "ABDF")
                {
                    thirdPlaceDictionary.Add("B", "D");
                    thirdPlaceDictionary.Add("C", "F");
                    thirdPlaceDictionary.Add("E", "A");
                    thirdPlaceDictionary.Add("F", "B");
                }
                else if (strFirstFour == "ABEF")
                {
                    thirdPlaceDictionary.Add("B", "E");
                    thirdPlaceDictionary.Add("C", "F");
                    thirdPlaceDictionary.Add("E", "B");
                    thirdPlaceDictionary.Add("F", "A");
                }
                else if (strFirstFour == "ACDE")
                {
                    thirdPlaceDictionary.Add("B", "E");
                    thirdPlaceDictionary.Add("C", "D");
                    thirdPlaceDictionary.Add("E", "C");
                    thirdPlaceDictionary.Add("F", "A");
                }
                else if (strFirstFour == "ACDF")
                {
                    thirdPlaceDictionary.Add("B", "F");
                    thirdPlaceDictionary.Add("C", "D");
                    thirdPlaceDictionary.Add("E", "C");
                    thirdPlaceDictionary.Add("F", "A");
                }
                else if (strFirstFour == "ACEF")
                {
                    thirdPlaceDictionary.Add("B", "E");
                    thirdPlaceDictionary.Add("C", "F");
                    thirdPlaceDictionary.Add("E", "C");
                    thirdPlaceDictionary.Add("F", "A");
                }
                else if (strFirstFour == "ADEF")
                {
                    thirdPlaceDictionary.Add("B", "E");
                    thirdPlaceDictionary.Add("C", "F");
                    thirdPlaceDictionary.Add("E", "D");
                    thirdPlaceDictionary.Add("F", "A");
                }
                else if (strFirstFour == "BCDE")
                {
                    thirdPlaceDictionary.Add("B", "E");
                    thirdPlaceDictionary.Add("C", "D");
                    thirdPlaceDictionary.Add("E", "B");
                    thirdPlaceDictionary.Add("F", "C");
                }
                else if (strFirstFour == "BCDF")
                {
                    thirdPlaceDictionary.Add("B", "F");
                    thirdPlaceDictionary.Add("C", "D");
                    thirdPlaceDictionary.Add("E", "C");
                    thirdPlaceDictionary.Add("F", "B");
                }
                else if (strFirstFour == "BCEF")
                {
                    thirdPlaceDictionary.Add("B", "F");
                    thirdPlaceDictionary.Add("C", "E");
                    thirdPlaceDictionary.Add("E", "C");
                    thirdPlaceDictionary.Add("F", "B");
                }
                else if (strFirstFour == "BDEF")
                {
                    thirdPlaceDictionary.Add("B", "F");
                    thirdPlaceDictionary.Add("C", "E");
                    thirdPlaceDictionary.Add("E", "D");
                    thirdPlaceDictionary.Add("F", "B");
                }

                else if (strFirstFour == "CDEF")
                {
                    thirdPlaceDictionary.Add("B", "F");
                    thirdPlaceDictionary.Add("C", "E");
                    thirdPlaceDictionary.Add("E", "D");
                    thirdPlaceDictionary.Add("F", "C");
                }
            }

            var koFixtures = context.KoFixtures.Where(k => k.EventId == eventId);
            var maxRoundOf = koFixtures.Max(p => p.RoundOf);

            foreach (var koFixture in koFixtures.Where(k => k.RoundOf == maxRoundOf).OrderBy(p => p.Position))
            {
                var team1FromLeagueId = koFixture.Team1FromLeagueId ?? 0;
                var team1FromLeaguePosition = koFixture.Team1FromLeaguePosition ?? default(int);

                var team2FromLeagueId = koFixture.Team2FromLeagueId ?? 0;
                var team2FromLeaguePosition = koFixture.Team2FromLeaguePosition ?? default(int);

                var team1 = GetTeamFromPosition(leagueTables, team1FromLeagueId, team1FromLeaguePosition);
                var team2 = team2FromLeagueId > 0
                    ? GetTeamFromPosition(leagueTables, team2FromLeagueId, team2FromLeaguePosition)
                    : GetBestPlacedThirdPosition(leagueTables, thirdPlaceDictionary, team1FromLeagueId);

                resultingDictionary.Add((koFixture.Position - 1) * 2 + 1, team1.TeamId);
                resultingDictionary.Add((koFixture.Position - 1) * 2 + 2, team2.TeamId);
            }

            return resultingDictionary;
        }

        public static LeagueTableTeam GetTeamFromPosition(List<LeagueTable> leagueTables, short leagueId,
            int position)
        {
            var teamLeagueTable = leagueTables.Find(l => l.LeagueSubLeague.Id == leagueId);
            return teamLeagueTable.LeagueTableTeams.Find(t => t.Position == position);
        }

        public static LeagueTableTeam GetBestPlacedThirdPosition(List<LeagueTable> leagueTables, Dictionary<string, string> thirdPlaceDictionary, int team1FromLeagueId)
        {

            var firstPlaceTeamLeagueShortName = leagueTables.First(a => a.LeagueSubLeague.Id == team1FromLeagueId).LeagueSubLeague.SubLeagueName;
            var leagueShortNameToGet3rdPlaceTeamFrom = thirdPlaceDictionary[firstPlaceTeamLeagueShortName];
            var leagueIdToGet3rdPlaceTeamFrom = leagueTables.First(a => a.LeagueSubLeague.SubLeagueName == leagueShortNameToGet3rdPlaceTeamFrom).LeagueSubLeague.Id;

            var teamLeagueTable = leagueTables.Find(l => l.LeagueSubLeague.Id == leagueIdToGet3rdPlaceTeamFrom);
            return teamLeagueTable.LeagueTableTeams.Find(t => t.Position == 3);
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
            var eventsKo = context.EventKos.Where(a => a.EventId == eventId).FirstOrDefault();
            var leagueSubLeagueTeams = context.LeagueSubLeagueTeams
                                        .Include(p => p.Team)
                                        .Include(p => p.LeagueSubLeague)
                                        .Where(a => a.LeagueSubLeague.LeagueId == eventsKo.LinkedLeagueId).ToList();

            var leagueTables = GetLeagueTables(context, leagueSubLeagueTeams, eventId);

            var eventFixtures = context.EventFixtures
                .Where(p => p.EventId == eventId && p.Fixture.AwayResult != null);

            foreach (var eventFixture in eventFixtures)
            {
                var homeTeamId = eventFixture.Fixture.HomeTeamId;
                var awayTeamId = eventFixture.Fixture.AwayTeamId;
                var eventTeamHome = leagueSubLeagueTeams.First(t => t.TeamId == homeTeamId);
                var eventTeamAway = leagueSubLeagueTeams.First(t => t.TeamId == awayTeamId);

                if (eventTeamHome.LeagueSubLeagueId == eventTeamAway.LeagueSubLeagueId) // This should always be true
                {
                    var leagueId = eventTeamHome.LeagueSubLeagueId;
                    var leagueTable = leagueTables.First(l => l.LeagueSubLeague.Id == leagueId);

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
            var eventsKo = context.EventKos.Where(a => a.EventId == eventId).FirstOrDefault();
            var leagueSubLeagueTeams = context.LeagueSubLeagueTeams
                                            .Include(p => p.Team)
                                            .Include(p => p.LeagueSubLeague)
                                            .Where(a => a.LeagueSubLeague.LeagueId == eventsKo.LinkedLeagueId).ToList();

            var leagueTables = GetLeagueTables(context, leagueSubLeagueTeams, eventId);

            var fixturePredictions = context.FixturePredictions
                .Include(p => p.EventFixture)
                .Include(p => p.EventFixture.Fixture)
                .Where(p => p.PlayerId == userId && p.EventFixture.EventId == eventId);

            foreach (var fixturePrediction in fixturePredictions)
            {
                var homeTeamId = fixturePrediction.EventFixture.Fixture.HomeTeamId;
                var awayTeamId = fixturePrediction.EventFixture.Fixture.AwayTeamId;

                var eventTeamHome = leagueSubLeagueTeams.FirstOrDefault(t => t.TeamId == homeTeamId);
                var eventTeamAway = leagueSubLeagueTeams.FirstOrDefault(t => t.TeamId == awayTeamId);

                if (eventTeamHome!=null && eventTeamAway!=null && eventTeamHome.LeagueSubLeagueId == eventTeamAway.LeagueSubLeagueId)
                {
                    var leagueSubLeagueId = eventTeamHome.LeagueSubLeagueId;
                    var leagueTable = leagueTables.First(l => l.LeagueSubLeague.Id == leagueSubLeagueId);

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
            leagueTables = leagueTables.OrderBy(l => l.LeagueSubLeague.Id).ToList();

            foreach (var leagueTable in leagueTables)
            {
                leagueTable.LeagueTableTeams = leagueTable.LeagueTableTeams.OrderByDescending(c => c.Points)
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

        //public static List<EventTeam> GetEventTeams(ApplicationDbContext context, int eventId)
        //{
        //    var eventTeams = context.Database.SqlQuery<EventTeam>(
        //        "spGetTeamsByEvent @intEventId"
        //        , new SqlParameter("@intEventId", eventId)).ToList();

        //    return eventTeams;
        //}

        private static List<LeagueTable> GetLeagueTables(ApplicationDbContext context, List<LeagueSubLeagueTeam> leagueSubLeagueTeams, int eventId)
        {
            var leagueTables = new List<LeagueTable>();

            foreach (var leagueSubLeagueTeam in leagueSubLeagueTeams)
            {
                var leagueSubLeagueId = leagueSubLeagueTeam.LeagueSubLeagueId;
                var currentLeagueTable = leagueTables.FirstOrDefault(e => e.LeagueSubLeague.Id == leagueSubLeagueId);
                if (currentLeagueTable == null)
                {
                    var leagueTable = new LeagueTable
                    {
                        LeagueSubLeague = leagueSubLeagueTeam.LeagueSubLeague,
                        LeagueTableTeams = new List<LeagueTableTeam>()
                    };

                    leagueTable.LeagueTableTeams.Add(NewLeagueTableTeam(leagueSubLeagueTeam.Team.TeamName, leagueSubLeagueTeam.Team.Id,
                        leagueSubLeagueTeam.Team.FlagFileLocation));

                    leagueTables.Add(leagueTable);
                }
                else
                {
                    currentLeagueTable.LeagueTableTeams.Add(NewLeagueTableTeam(leagueSubLeagueTeam.Team.TeamName,
                        leagueSubLeagueTeam.Team.Id, leagueSubLeagueTeam.Team.FlagFileLocation));
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