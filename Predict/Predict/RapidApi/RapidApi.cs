using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using Microsoft.Ajax.Utilities;
using Predict.RapidAPIFixturesByLeague;
using Predict.Models;
using RestSharp;
using RestSharp.Serialization.Json;
using Fixture = Predict.RapidAPIFixturesByLeague.Fixture;
using System.Web.UI.WebControls;

namespace Predict.Helper
{
    public static class RapidApi
    {
        
        public static void UpdatePremierLeague()
        {
            var rapidApiLeagueId = 2790;
            var leagueName = "Premier League 2020/21";
            var shortLeagueName = "Premier League";
            FixturesByLeague(rapidApiLeagueId, leagueName, shortLeagueName);
        }

        public static void FixturesByLeague(int rapidApiLeagueId, string leagueName, string shortLeagueName)
        {
            var context = new ApplicationDbContext();

            var siteSettingName = "RapidApi_Fixtures_By_League_" + rapidApiLeagueId + "_" +
                                  DateTime.Now.Year.ToString() + "_" + DateTime.Now.Month.ToString();
            var siteSettingValue = GetSiteSetting(context, siteSettingName);
            if (siteSettingValue.IsNullOrWhiteSpace())
            {
                siteSettingValue = "0";
            }

            var nbrTimesApiCalled = System.Convert.ToInt32(siteSettingValue);
            if (nbrTimesApiCalled >= 100)
                return;

            var client = new RestClient("https://api-football-v1.p.rapidapi.com/v2/fixtures/league/"+ rapidApiLeagueId + "?timezone=Europe%2FLondon");
            var request = new RestRequest(Method.GET);
            request.AddHeader("x-rapidapi-host", "api-football-v1.p.rapidapi.com");
            request.AddHeader("x-rapidapi-key", "d2f34fda45msh6212d84073d1f56p1b7dd9jsn619a512aa3fb");
            IRestResponse response = client.Execute(request);

            // Get setting value
            nbrTimesApiCalled = nbrTimesApiCalled + 1;
            siteSettingValue = nbrTimesApiCalled.ToString();
            SaveSiteSetting(context, siteSettingName,siteSettingValue);                

            var jsonSerializer = new JsonSerializer();
            var rapidApiFixtures = jsonSerializer.Deserialize<Root>(response);

            var fixtures = context.Fixtures.ToList();
            var teams = context.Teams.ToList();
            var newResultFound = false;

            var league = context.Leagues.FirstOrDefault(f => f.RapidApiLeagueId == rapidApiLeagueId);
            if (league == null)
            {
                league = new Models.League
                {
                    LeagueName = leagueName,
                    ShortLeagueName = shortLeagueName,
                    CreatedDateTime = DateTime.Now,
                    ModifiedDateTime = DateTime.Now,
                    RapidApiLeagueId = rapidApiLeagueId
                };
                context.Leagues.Add(league);
                context.SaveChanges();
            }

            foreach (var rapidApiFixture in rapidApiFixtures.api.fixtures)
            {
                var rapidApiFixtureId = rapidApiFixture.fixture_id;
                var updateDb = false;
                var homeTeam = CheckTeamExists(context, teams, rapidApiFixture.homeTeam.team_id,
                    rapidApiFixture.homeTeam.team_name, rapidApiFixture.homeTeam.logo);

                if (homeTeam.Id == 0)
                {
                    context.Teams.AddOrUpdate(homeTeam);
                    updateDb = true;
                }

                var awayTeam = CheckTeamExists(context, teams, rapidApiFixture.awayTeam.team_id,
                    rapidApiFixture.awayTeam.team_name, rapidApiFixture.awayTeam.logo);

                if (awayTeam.Id == 0)
                {
                    context.Teams.AddOrUpdate(awayTeam);
                    updateDb = true;
                }

                if(updateDb)
                    context.SaveChanges();

                // Check if the fixture needs updating
                updateDb = false;

                var fixture = fixtures.FirstOrDefault(f => f.RapidApiFixtureId == rapidApiFixtureId);
                if(rapidApiFixture.status == "Match Postponed")
                {
                    if (fixture != null)
                    {
                        if(fixture.HomeResult!=null)
                            newResultFound = true;

                        context.Fixtures.Remove(fixture);
                        updateDb = true;
                    }
                }
                else
                {
                    if (fixture == null)
                    {
                        fixture = new Models.Fixture
                        {
                            RapidApiFixtureId = rapidApiFixtureId,
                            HomeTeamId = homeTeam.Id,
                            AwayTeamId = awayTeam.Id,
                            LeagueId = league.Id,
                            CreatedDateTime = DateTime.Now,
                            ModifiedDateTime = DateTime.Now
                        };
                        updateDb = true;
                    }

                    if (fixture.HomeResult == null && rapidApiFixture.score.fulltime != null)
                    {
                        newResultFound = true;
                        updateDb = true;
                        fixture.HomeResult = (short) rapidApiFixture.goalsHomeTeam;
                        fixture.AwayResult = (short) rapidApiFixture.goalsAwayTeam;
                        fixture.ResultProcessed = false;
                    }

                    if (rapidApiFixture.event_date.IsDaylightSavingTime())
                    {
                        // Change time to UTC
                        rapidApiFixture.event_date = rapidApiFixture.event_date.AddHours(-1);
                    }

                    if (fixture.FixtureDateTime != rapidApiFixture.event_date)
                    {
                        fixture.FixtureDateTime = rapidApiFixture.event_date;
                        updateDb = true;
                    }

                    if (updateDb)
                        context.Fixtures.AddOrUpdate(fixture);
                    
                }

            }
            context.SaveChanges();

            if(newResultFound)
                Helper.Cache.UpdateScoring(context);

        }

        private static Models.Team CheckTeamExists(ApplicationDbContext context, List<Models.Team> teams, int rapidApiTeamId, string teamName, string logo)
        {
            var team = context.Teams.FirstOrDefault(t => t.RapidApiTeamId == rapidApiTeamId);
            if (team == null)
            {
                team = new Models.Team
                {
                    RapidApiTeamId = rapidApiTeamId,
                    TeamName = teamName,
                    TeamFlag = logo,
                    CreatedDateTime = DateTime.Now,
                    ModifiedDateTime = DateTime.Now
                };
            }

            return team;
        }

        public static void SaveSiteSetting(ApplicationDbContext context, string settingName, string settingValue)
        {
            var siteSetting = context.SiteSettings.FirstOrDefault(f => f.SettingName == settingName);
            if (siteSetting == null)
            {
                siteSetting = new SiteSetting
                {
                    SettingName = settingName,
                    SettingValue = settingValue,
                    CreatedDateTime = DateTime.Now,
                    ModifiedDateTime = DateTime.Now
                };
                context.SiteSettings.Add(siteSetting);
            }
        }

        public static string GetSiteSetting(ApplicationDbContext context, string settingName)
        {
            var siteSetting = context.SiteSettings.FirstOrDefault(f => f.SettingName == settingName);
            if (siteSetting == null)
                return "";

            return siteSetting.SettingValue;

        }
    }
}