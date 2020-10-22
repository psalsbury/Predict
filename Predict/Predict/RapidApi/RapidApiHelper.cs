using Predict.Models;
using Predict.RapidAPIFixtures;
using RestSharp;
using RestSharp.Serialization.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using League = Predict.Models.League;

namespace Predict.RapidApi
{

    public static class RapidApiHelper
    {
        public static async Task UpdatePremierLeagueAsync(int rapidApiLeagueId)
        {
            await Task.Run(() =>
            {
                FixturesByLeague(rapidApiLeagueId);
            });
        }

        public static void UpdatePremierLeague()
        {
           // this is for the 20/21 league
            var rapidApiLeagueId = 2790;
            FixturesByLeague(rapidApiLeagueId);
        }

        public static void UpdateChampionshipLeague()
        {
            // this is for the 20/21 league
            var rapidApiLeagueId = 2794;
            FixturesByLeague(rapidApiLeagueId);
        }

        public static void UpdateRapidApiLeagueByDate(int rapidApiLeagueId, DateTime dateToUpdate)
        {
            FixturesByLeagueByDate(rapidApiLeagueId, dateToUpdate);
        }
        public static void UpdateRapidApiLeague(int rapidApiLeagueId)
        {
            FixturesByLeague(rapidApiLeagueId);
        }

        private static void FixturesByLeagueByDate(int rapidApiLeagueId, DateTime dateToUpdate)
        {
            var context = new ApplicationDbContext();

            var nbrTimesApiCalled = SettingCheck(context);
            if (nbrTimesApiCalled >= 100)
                return;

            var resultDate = dateToUpdate.Year + "-" + dateToUpdate.Month.ToString("D2") + "-" + dateToUpdate.Day.ToString("D2");
            var client = new RestClient("https://api-football-v1.p.rapidapi.com/v2/fixtures/league/" + rapidApiLeagueId + "/" + resultDate + "?timezone=Europe%2FLondon");
            var request = new RestRequest(Method.GET);
            request.AddHeader("x-rapidapi-host", "api-football-v1.p.rapidapi.com");
            request.AddHeader("x-rapidapi-key", "dd93656aa1msh15481f122393c01p11fd67jsnf7e74925fed3");
            IRestResponse response = client.Execute(request);

            // Set setting value
            IncrementSetting(context, nbrTimesApiCalled);
            UpdateFixtures(context, response);
        }

        private static void FixturesByLeague(int rapidApiLeagueId)
        {
            var context = new ApplicationDbContext();
            var nbrTimesApiCalled = SettingCheck(context);

            if (nbrTimesApiCalled >= 100)
                return;

            var client = new RestClient("https://api-football-v1.p.rapidapi.com/v2/fixtures/league/" + rapidApiLeagueId + "?timezone=Europe%2FLondon");
            var request = new RestRequest(Method.GET);
            request.AddHeader("x-rapidapi-host", "api-football-v1.p.rapidapi.com");
            request.AddHeader("x-rapidapi-key", "dd93656aa1msh15481f122393c01p11fd67jsnf7e74925fed3");
            IRestResponse response = client.Execute(request);

            // Set setting value
            IncrementSetting(context, nbrTimesApiCalled);
            UpdateFixtures(context, response);
        }
        private static void UpdateFixtures(ApplicationDbContext context, IRestResponse response)
        {
            var jsonSerializer = new JsonSerializer();
            var rapidApiFixtures = jsonSerializer.Deserialize<Root>(response);
            var fixtures = context.Fixtures.ToList();
            var teams = context.Teams.ToList();
            var newResultFound = false;
            var eventsWithChangedFixtureDateTime = new List<short>();

            foreach (var rapidApiFixture in rapidApiFixtures.api.fixtures)
            {

                var rapidApiLeague = rapidApiFixture.league;
                var league = AddOrUpdateLeague(context, rapidApiLeague, rapidApiFixture.league_id);
                var homeTeam = AddOrUpdateTeam(context, teams, rapidApiFixture.homeTeam.team_id, rapidApiFixture.homeTeam.team_name, rapidApiFixture.homeTeam.logo);
                var awayTeam = AddOrUpdateTeam(context, teams, rapidApiFixture.awayTeam.team_id, rapidApiFixture.awayTeam.team_name, rapidApiFixture.awayTeam.logo);
                var rapidApiFixtureId = rapidApiFixture.fixture_id;

                // Check if the fixture needs updating
                var updateDb = false;

                var fixture = fixtures.FirstOrDefault(f => f.RapidApiFixtureId == rapidApiFixtureId);
                if (rapidApiFixture.status == "Match Postponed")
                {
                    if (fixture != null)
                    {
                        if (fixture.HomeResult != null)
                            newResultFound = true;

                        context.Fixtures.Remove(fixture);
                        updateDb = true;
                    }
                }
                else
                {
                    if (rapidApiFixture.event_date.IsDaylightSavingTime())
                    {
                        // Change time to UTC
                        rapidApiFixture.event_date = rapidApiFixture.event_date.AddHours(-1);
                    }

                    if (fixture == null)
                    {
                        fixture = new Models.Fixture
                        {
                            RapidApiFixtureId = rapidApiFixtureId,
                            HomeTeamId = homeTeam.Id,
                            AwayTeamId = awayTeam.Id,
                            LeagueId = league.Id,
                            FixtureDateTime = rapidApiFixture.event_date,
                            CreatedDateTime = DateTime.UtcNow,
                            ModifiedDateTime = DateTime.UtcNow
                        };
                        updateDb = true;
                    }
                    else
                    {
                        // Existing fixture
                        if (fixture.FixtureDateTime != rapidApiFixture.event_date)
                        {
                            // Date has changed
                            fixture.FixtureDateTime = rapidApiFixture.event_date;
                            fixture.ModifiedDateTime = DateTime.UtcNow;
                            updateDb = true;

                            // Get all the events that have this fixture
                            var eventFixtures = context.EventFixtures.Where(a => a.FixtureId == fixture.Id).ToList();
                            foreach (var eventFixture in eventFixtures)
                            {
                                var eventId = eventFixture.EventId;
                                if (!eventsWithChangedFixtureDateTime.Contains(eventId))
                                    eventsWithChangedFixtureDateTime.Add(eventId);
                            }
                        }
                    }

                    if (fixture.HomeResult == null && rapidApiFixture.score.fulltime != null)
                    {
                        newResultFound = true;
                        updateDb = true;
                        fixture.HomeResult = (short)rapidApiFixture.goalsHomeTeam;
                        fixture.AwayResult = (short)rapidApiFixture.goalsAwayTeam;
                        fixture.ResultProcessed = false;
                    }

                    if (updateDb)
                        context.Fixtures.AddOrUpdate(fixture);
                }

            }
            context.SaveChanges();

            if (newResultFound)
                Helper.Cache.UpdateScoring(context);

            foreach (var eventId in eventsWithChangedFixtureDateTime)
            {
                Helper.Cache.UpdateEventStartEnd(context, eventId);
            }

        }
        private static League AddOrUpdateLeague(ApplicationDbContext context, RapidAPIFixtures.League rapidApiLeague, int rapidApiLeagueId)
        {
            var league = context.Leagues.FirstOrDefault(f => f.RapidApiLeagueId == rapidApiLeagueId);
            if (league == null)
            {
                league = new Models.League
                {
                    LeagueName = rapidApiLeague.name,
                    ShortLeagueName = rapidApiLeague.name,
                    CreatedDateTime = DateTime.UtcNow,
                    ModifiedDateTime = DateTime.UtcNow,
                    RapidApiLeagueId = rapidApiLeagueId
                };
                context.Leagues.Add(league);
                context.SaveChanges();
            }

            return league;
        }
        private static Models.Team AddOrUpdateTeam(ApplicationDbContext context, List<Models.Team> teams, int rapidApiTeamId, string teamName, string logo)
        {
            var team = context.Teams.FirstOrDefault(t => t.RapidApiTeamId == rapidApiTeamId);
            teamName = teamName.Replace(" United", " Utd");
            var changeMade = false;
            if (team == null)
            {
                team = new Models.Team
                {
                    RapidApiTeamId = rapidApiTeamId,
                    TeamName = teamName,
                    TeamFlag = logo,
                    CreatedDateTime = DateTime.UtcNow,
                    ModifiedDateTime = DateTime.UtcNow
                };
                context.Teams.AddOrUpdate(team);
                changeMade = true;
            }
            else
            {
                if (team.TeamName != teamName || team.TeamFlag != logo)
                {
                    team.TeamName = teamName;
                    team.TeamFlag = logo;
                    changeMade = true;
                }
            }
            if(changeMade== true)
                context.SaveChanges();

            return team;
        }
        private static void SaveSiteSetting(ApplicationDbContext context, string settingName, int settingValue)
        {
            var siteSetting = context.SiteSettings.FirstOrDefault(f => f.SettingName == settingName);
            if (siteSetting == null)
            {
                siteSetting = new SiteSetting
                {
                    SettingName = settingName,
                    SettingValue = settingValue.ToString(),
                    CreatedDateTime = DateTime.UtcNow,
                    ModifiedDateTime = DateTime.UtcNow
                };
                context.SiteSettings.Add(siteSetting);
            }
            else
            {
                siteSetting.SettingValue = settingValue.ToString();
                siteSetting.ModifiedDateTime = DateTime.UtcNow;
                context.SiteSettings.AddOrUpdate(siteSetting);
            }
        }

        private static int GetSiteSetting(ApplicationDbContext context, string settingName)
        {
            var siteSetting = context.SiteSettings.FirstOrDefault(f => f.SettingName == settingName);
            if (siteSetting == null)
                return 0;

            var numericValue = System.Convert.ToInt32(siteSetting.SettingValue);

            return numericValue;

        }

        private static int SettingCheck(ApplicationDbContext context)
        {
            var siteSettingName = "RapidApi_Fixtures" + DateTime.UtcNow.Year.ToString() + "_" + DateTime.UtcNow.Month.ToString() + "_" + DateTime.UtcNow.Day.ToString();
            return GetSiteSetting(context, siteSettingName);

        }
        private static void IncrementSetting(ApplicationDbContext context, int nbrTimesApiCalled)
        {
            var siteSettingName = "RapidApi_Fixtures" + DateTime.UtcNow.Year.ToString() + "_" + DateTime.UtcNow.Month.ToString() + "_" + DateTime.UtcNow.Day.ToString();
            SaveSiteSetting(context, siteSettingName, nbrTimesApiCalled + 1);
        }
    }
}