using Predict.Models;
using Predict.RapidAPIFixtures;
using RestSharp;
using RestSharp.Serialization.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Data.Entity;
using League = Predict.Models.League;

namespace Predict.RapidApi
{

    public static class RapidApiHelper
    {
        private static string RapidApiFixtureUrl = "https://api-football-v1.p.rapidapi.com/v2/fixtures/league/";
        private static string Timezone = "timezone=Europe%2FLondon";
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private static void DailyRapidApiGenerateEvents(ApplicationDbContext context)
        {
            // Check if we need to generate events. 
            // AutoGenerateEvents --> 1 = Weekly, 2 = twice per month, 3 = Monthly
            // Weeks start on same weekday as first fixture for the league
            // Months are whole calendar months


            var leagueEventGenerations = context.LeagueEventGenerations
                .Include(a => a.League)
                .Where(a => a.Enabled == true).ToList();

            foreach (var leagueEventGeneration in leagueEventGenerations)
            {
                // work out first fixture date
                // iterate out to find next suitable event to create. Only create event if not already created.
                // month after current month

                var league = leagueEventGeneration.League;
                var weeklyWeeks = 1;

                var firstFixtureForLeague = context.Fixtures.Where(a => a.LeagueId == league.Id)
                    .OrderBy(a => a.FixtureDateTime)
                    .FirstOrDefault();

                var lastFixtureForLeague = context.Fixtures.Where(a => a.LeagueId == league.Id)
                    .OrderByDescending(a => a.FixtureDateTime)
                    .FirstOrDefault();

                if (firstFixtureForLeague != null && lastFixtureForLeague!= null)
                {

                    var dateToCheck = firstFixtureForLeague.FixtureDateTime.Date;
                    var endDate = firstFixtureForLeague.FixtureDateTime.Date;

                    // For weekly events I want
                    // 2 separate weekly events generated where the start date is ahead of today
                    // , find the first date of the day of the week of the first fixture that is after todays date

                    if (leagueEventGeneration.GenerationFrequencyId == 1)
                    {

                        while (dateToCheck < DateTime.UtcNow.Date)
                        {
                            dateToCheck = dateToCheck.AddDays(7);
                            weeklyWeeks += 1;
                        }

                        endDate = dateToCheck.AddDays(7);
                    }
                    else if (leagueEventGeneration.GenerationFrequencyId == 2)
                    {
                        if (dateToCheck > DateTime.UtcNow)
                        {
                            // If the first fixture is in the future, then set up the first event to be to 15 or end of month
                            if (dateToCheck.Day <= 15)
                            {
                                endDate = dateToCheck.AddDays(15 - dateToCheck.Day);
                            }
                            else
                            {
                                endDate = new DateTime(dateToCheck.AddMonths(1).Year, dateToCheck.AddMonths(1).Month, 1).AddDays(-1);
                            }
                        }
                        else
                        {
                            // Get next 1st of month, or 16th of month
                            if (DateTime.UtcNow.Day <= 15)
                            {
                                // 16th of current month
                                dateToCheck = DateTime.UtcNow.AddDays(15 - DateTime.UtcNow.Day);
                                endDate = new DateTime(dateToCheck.AddMonths(1).Year, dateToCheck.AddMonths(1).Month, 1).AddDays(-1);
                            }
                            else
                            {
                                // first of next month
                                dateToCheck = new DateTime(DateTime.UtcNow.AddMonths(1).Year, DateTime.UtcNow.AddMonths(1).Month, 1);
                                endDate = dateToCheck.AddDays(14);
                            }
                        }

                    }
                    else if (leagueEventGeneration.GenerationFrequencyId == 3)
                    {
                        if (dateToCheck > DateTime.UtcNow)
                        {
                            endDate = new DateTime(DateTime.UtcNow.AddMonths(1).Year, DateTime.UtcNow.AddMonths(2).Month, 1).AddDays(-1);
                        }
                        else
                        {
                            dateToCheck = new DateTime(DateTime.UtcNow.AddMonths(1).Year, DateTime.UtcNow.AddMonths(1).Month, 1);
                            endDate = new DateTime(DateTime.UtcNow.AddMonths(1).Year, DateTime.UtcNow.AddMonths(2).Month, 1).AddDays(-1);
                        }
                        
                    }

                    if (dateToCheck > lastFixtureForLeague.FixtureDateTime.Date)
                    {
                        // There are no fixtures left. Disable any further generations 
                        leagueEventGeneration.Enabled = false;
                        leagueEventGeneration.ModifiedDateTime = DateTime.UtcNow;
                    }
                    else
                    {
                        // Date found that is ahead of today and is before the last fixture
                        // I want to create a weekly event starting at this date
                        var eventExists = context.EventGenerations.Any(a => a.LeagueEventGenerationId == leagueEventGeneration.Id
                                                                            && a.BaseStartDate == dateToCheck
                                                                            && a.BaseEndDate == endDate);
                        if (!eventExists)
                        {
                            var eventName = league.ShortLeagueName;

                            if (leagueEventGeneration.GenerationFrequencyId == 1)
                            {
                                eventName = eventName + " Week " + weeklyWeeks.ToString();
                            }
                            else if (leagueEventGeneration.GenerationFrequencyId == 2)
                            {
                                eventName = eventName + " " + dateToCheck.ToString("MMM") + " " + dateToCheck.Day +
                                            " - " + endDate.Day;
                            }
                            else if (leagueEventGeneration.GenerationFrequencyId == 3)
                            {
                                eventName = eventName + " " + dateToCheck.ToString("MMM");
                            }

                            CreateEvent(leagueEventGeneration, dateToCheck, endDate, eventName, context);
                        }

                    }

                }

            }


        }

        private static void CreateEvent(LeagueEventGeneration leagueEventGeneration, DateTime startDate, DateTime endDate, string eventName, ApplicationDbContext context)
        {

            var endDateToUse = endDate.AddDays(1); // Linq doesnt like .AddDays. Add 1 day to include fixtures with that date date
            var fixtures = context.Fixtures.Where(a => a.ResultProcessed == false
                                                       && a.LeagueId == leagueEventGeneration.League.Id
                                                       && a.FixtureDateTime >= startDate
                                                       && a.FixtureDateTime < endDateToUse)
                                                        .OrderBy(a => a.FixtureDateTime).ToList();

            // If no fixtures available, then exit
            if (!fixtures.Any())
                return;

            // Find the default pool
            var defaultPool = context.Pools.First(a => a.DefaultPoolForEvent == true);

            var alreadyExists = context.EventGenerations.Count(a => a.LeagueEventGenerationId == leagueEventGeneration.Id);

            // Event has not been created, so create it.
            var myEvent = new Event
            {
                EventName = eventName,
                EventDescription = eventName,
                CreatedDateTime = DateTime.UtcNow,
                ModifiedDateTime = DateTime.UtcNow,
                CreatedByPlayerId = defaultPool.AdminPlayerId,
                StartDateTime = fixtures.OrderBy(a => a.FixtureDateTime).First().FixtureDateTime,
                EndDateTime = fixtures.OrderByDescending(a => a.FixtureDateTime).First().FixtureDateTime,
                DefaultPoolId = defaultPool.Id
            };
            context.Events.Add(myEvent);
            context.SaveChanges();

            Helper.Cache.SetEventCache();

            var myEventPool = new EventPool
            {
                PoolId = defaultPool.Id, EventId = myEvent.Id, CreatedDateTime = DateTime.UtcNow,
                Enabled =true,
                ModifiedDateTime = DateTime.UtcNow
            };

            context.EventPools.Add(myEventPool);

            var myEventGeneration = new EventGeneration
            {
                EventId = myEvent.Id,
                CreatedDateTime = DateTime.UtcNow,
                ModifiedDateTime = DateTime.UtcNow,
                BaseStartDate = startDate,
                BaseEndDate = endDate,
                LeagueEventGenerationId = leagueEventGeneration.Id
            };

            context.EventGenerations.Add(myEventGeneration);

            foreach (var fixture in fixtures)
            {
                var eventFixture = new EventFixture
                {
                    EventId = myEvent.Id
                    , FixtureId = fixture.Id
                    , CreatedDateTime = DateTime.UtcNow
                    , ModifiedDateTime = DateTime.UtcNow
                    
                };
                context.EventFixtures.Add(eventFixture);
            }

            context.SaveChanges();

        }

        public static void DailyRapidApiLeagueCheck()
        {

            // Check all leagues that are linked to rapid api. Update all leagues on a daily basis

            var cacheId = "DailyRapidApiLeagueCheck";
            var performUpdate = false;
            var checkObject = Helper.Cache.GetCachedItem(cacheId);
            if (checkObject == null)
            {
                // Time to do the daily check
                var context = new ApplicationDbContext();
                var siteSetting = context.SiteSettings.FirstOrDefault(a => a.SettingName == cacheId);
                if (siteSetting == null)
                {

                    Logger.Info("DailyRapidApiLeagueCheck --> Site Setting not found");

                    siteSetting = new SiteSetting
                    {
                        CreatedDateTime = DateTime.UtcNow,
                        ModifiedDateTime = DateTime.UtcNow,
                        SettingName = cacheId,
                        SettingValue = DateTime.Now.Date.ToLongDateString()
                    };
                    context.SiteSettings.Add(siteSetting);
                    context.SaveChanges();
                    performUpdate = true;
                }
                else
                {
                    var lastDate = System.Convert.ToDateTime(siteSetting.SettingValue);
                    if (lastDate < DateTime.Now.Date)
                    {

                        Logger.Info("DailyRapidApiLeagueCheck --> lastDate = {0}, Now = {1}", lastDate, DateTime.Now.Date);

                        siteSetting.SettingValue = DateTime.Now.Date.ToLongDateString();
                        siteSetting.ModifiedDateTime = DateTime.UtcNow;
                        context.SiteSettings.AddOrUpdate(siteSetting);
                        context.SaveChanges();
                        performUpdate = true;
                    }
                }

                Helper.Cache.SetCachedItem(cacheId, "ReRunWhenExpired", DateTime.Today.AddDays(1));
                if (performUpdate)
                {
                    Logger.Info("DailyRapidApiLeagueCheck --> Performing daily league update");

                    var leagues = context.Leagues.Where(a => a.RapidApiLeagueId != null && a.DailyRapidApiCheck==true);
                    foreach (var league in leagues)
                    {
                        // Update the whole league for this league
                        RapidApiHelper.FixturesByLeague(league.RapidApiLeagueId ?? 0);

                        var count = context.Fixtures.Count(a => a.LeagueId == league.Id && a.ResultProcessed == false);

                        if (count == 0)
                        {
                            // If there are no further fixtures left, then set the league to stop checking every day
                            league.DailyRapidApiCheck = false;
                            league.ModifiedDateTime = DateTime.UtcNow;
                            context.Leagues.AddOrUpdate(league);
                        }
                    }
                    context.SaveChanges();

                    // Check if events need to be created
                    DailyRapidApiGenerateEvents(context);
                }

                context.Dispose();
            }
        }


        public static void GetRapidApiResults()
        {
            string cacheKey = "NextFixtureCheckDateTime";
            bool checkPerformed = false;
            var rapidApiResultChecks = (List<RapidApiResultCheck>)Helper.Cache.GetCachedItem(cacheKey);
            if (rapidApiResultChecks == null)
            {
                SetNextResultCheckDateTime(false);
                rapidApiResultChecks = (List<RapidApiResultCheck>)Helper.Cache.GetCachedItem(cacheKey);
            }

            foreach (var rapidApiResultCheck in rapidApiResultChecks)
            {

                if (rapidApiResultCheck.FixtureDateTime != DateTime.MinValue && rapidApiResultCheck.FixtureDateTime <= DateTime.UtcNow)
                {
                    var rapidApiLeagueId = rapidApiResultCheck.RapidApiLeagueId;

                    Logger.Info("GetRapidApiResults = Getting results from RapidApi {0}. FixtureDateTime = {1}, Now = {2}", rapidApiLeagueId, rapidApiResultCheck.FixtureDateTime.Date, DateTime.UtcNow);

                    if (rapidApiResultCheck.FixtureDateTime.Date < DateTime.UtcNow.Date)
                    {
                        // If the date of the fixture is less than today then get all results
                        RapidApiHelper.FixturesByLeague(rapidApiLeagueId);
                    }
                    else
                    {
                        // If the date of the fixture is today, then get the results for today only
                        RapidApiHelper.FixturesByLeagueByDate(rapidApiLeagueId, DateTime.UtcNow);
                    }

                    checkPerformed = true;
                }
            }

            if (checkPerformed)
                SetNextResultCheckDateTime(true);
        }

        private static DateTime RoundUp(DateTime dt, TimeSpan d)
        {
            return new DateTime((dt.Ticks + d.Ticks - 1) / d.Ticks * d.Ticks, dt.Kind);
        }

        public static void SetNextResultCheckDateTime(bool roundUp)
        {
            // Get all the fixtures that are associated to events, that do not have a result
            Logger.Info("SetNextResultCheckDateTime - Start - roundUp = {0}", roundUp);

            string cacheKey = "NextFixtureCheckDateTime";
            bool updateNeeded = false;

            var rapidApiResultChecks = (List<RapidApiResultCheck>)Helper.Cache.GetCachedItem(cacheKey);
            if (rapidApiResultChecks == null)
            {
                Logger.Info("SetNextResultCheckDateTime - rapidApiResultChecks == null");
                updateNeeded = true;
            }
            else
            {
                foreach (var rapidApiResultCheck in rapidApiResultChecks)
                {
                    // FixtureDateTime includes an addition of 2 hours to ensure we are checking after the game has finished
                    if (rapidApiResultCheck.FixtureDateTime < DateTime.UtcNow)
                    {
                        updateNeeded = true;
                    }
                }
            }

            if (updateNeeded)
            {
                var context = new ApplicationDbContext();

                rapidApiResultChecks = context.Database.SqlQuery<RapidApiResultCheck>(
                    "spGetNextFixtureToCheckResult").ToList();

                foreach (var rapidApiResultCheck in rapidApiResultChecks)
                {
                    var fixtureDateTime = rapidApiResultCheck.FixtureDateTime.AddMinutes(115); // Add 1 hour 55 to the end time 

                    if (roundUp && fixtureDateTime < DateTime.UtcNow)
                        fixtureDateTime = DateTime.UtcNow;

                    if (roundUp)
                        fixtureDateTime = RoundUp(fixtureDateTime, TimeSpan.FromMinutes(5));

                    rapidApiResultCheck.FixtureDateTime = fixtureDateTime;

                    Logger.Info("SetNextResultCheckDateTime - Set League {0} next check date to {1}", rapidApiResultCheck.RapidApiLeagueId, rapidApiResultCheck.FixtureDateTime);
                }
                Helper.Cache.SetCachedItem(cacheKey, rapidApiResultChecks);
                context.Dispose();
            }
        }

        public static void FixturesByLeagueByDate(int rapidApiLeagueId, DateTime dateToUpdate)
        {

            var resultDate = dateToUpdate.Year + "-" + dateToUpdate.Month.ToString("D2") + "-" + dateToUpdate.Day.ToString("D2");

            Logger.Info("FixturesByLeagueByDate - League = {0}, Date = {2}", rapidApiLeagueId, resultDate);

            var baseUrl = RapidApiFixtureUrl + rapidApiLeagueId + "/" + resultDate + "?" + Timezone;
            MakeRapidApiCall(baseUrl);
        }

        public static void FixturesByLeague(int rapidApiLeagueId)
        {
            Logger.Info("FixturesByLeague - League = {0}", rapidApiLeagueId);

            var baseUrl = RapidApiFixtureUrl + rapidApiLeagueId + "?"+ Timezone;
            MakeRapidApiCall(baseUrl);
        }

        private static void MakeRapidApiCall(string baseUrl)
        {
            var context = new ApplicationDbContext();
            var nbrTimesApiCalled = SettingCheck(context);

            if (nbrTimesApiCalled >= 100)
            {
                Logger.Info("MakeRapidApiCall CALLS OVER 100");
                return;
            }

            var client = new RestClient(baseUrl);
            var request = new RestRequest(Method.GET);
            request.AddHeader("x-rapidapi-host", "api-football-v1.p.rapidapi.com");
            request.AddHeader("x-rapidapi-key", "dd93656aa1msh15481f122393c01p11fd67jsnf7e74925fed3");
            IRestResponse response = client.Execute(request);

            // Set setting value
            IncrementSetting(context, nbrTimesApiCalled);
            UpdateFixtures(context, response);
            context.Dispose();
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