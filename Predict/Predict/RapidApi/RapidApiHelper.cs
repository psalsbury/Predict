using Predict.Models;
using RestSharp;
using RestSharp.Serialization.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Data.Entity;
using System.Net.PeerToPeer.Collaboration;
using Antlr.Runtime;
using League = Predict.Models.League;

namespace Predict.RapidApi
{
    public static class RapidApiHelper
    {
        private static readonly string RapidApiV3OddsUrl = "https://api-football-v1.p.rapidapi.com/v3/odds";
        private static readonly string RapidApiV3LeaguesUrl = "https://api-football-v1.p.rapidapi.com/v3/leagues";
        private static readonly string RapidApiV3FixturesUrl = "https://api-football-v1.p.rapidapi.com/v3/fixtures";
        private static readonly string RapidApiV3CountriesUrl = "https://api-football-v1.p.rapidapi.com/v3/countries";
        private static readonly string Timezone = "timezone=Europe%2FLondon";
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static void V3Countries()
        {

            // Called from the Admin screen to get a list of all the countries that Rapid API can provide fixtures for
            var client = new RestClient(RapidApiV3CountriesUrl);
            var request = new RestRequest(Method.GET);
            request.AddHeader("X-RapidAPI-Host", "api-football-v1.p.rapidapi.com");
            request.AddHeader("X-RapidAPI-Key", "dd93656aa1msh15481f122393c01p11fd67jsnf7e74925fed3");
            IRestResponse response = client.Execute(request);

            var context = new ApplicationDbContext();
            var jsonSerializer = new JsonSerializer();
            var responseCountries = jsonSerializer.Deserialize<RapidApiV3CountriesClassHelper.Root>(response);

            foreach(var responseCountry in responseCountries.response)
            {
                var countryName = responseCountry.name;
                var countryCode = responseCountry.code;
                var countryFlag = responseCountry.flag;

                var rapidApiV3Country = context.RapidApiV3Countries.Where(a => a.CountryName == countryName).FirstOrDefault();
                if (rapidApiV3Country == null)
                {
                    rapidApiV3Country = new RapidApiV3Country
                    {
                        CountryCode = countryCode,
                        Flag = countryFlag,
                        CountryName = countryName,
                        CreatedDateTime = DateTime.UtcNow,
                        ModifiedDateTime = DateTime.UtcNow
                    };
                    context.RapidApiV3Countries.Add(rapidApiV3Country);
                }
            }
            context.SaveChanges();
        }

        public static void V3Leagues(string countryToFetch, int seasonToFetch)
        {
            // Called from the admin screen to get a list of all leagues for a given country and year
            var client = new RestClient(RapidApiV3LeaguesUrl + "?country=" + countryToFetch + "&season=" + seasonToFetch.ToString());
            var request = new RestRequest(Method.GET);
            request.AddHeader("X-RapidAPI-Host", "api-football-v1.p.rapidapi.com");
            request.AddHeader("X-RapidAPI-Key", "dd93656aa1msh15481f122393c01p11fd67jsnf7e74925fed3");
            IRestResponse response = client.Execute(request);

            var context = new ApplicationDbContext();
            var jsonSerializer = new JsonSerializer();
            var responseLeagues = jsonSerializer.Deserialize<RapidApiV3LeagueClassHelper.Root>(response);

            foreach (var responseLeague in responseLeagues.response)
            {

                var country = responseLeague.country;
                var league = responseLeague.league;
                var seasons = responseLeague.seasons;

                var rapidApiV3Country = context.RapidApiV3Countries.Where(a => a.CountryName == country.name).FirstOrDefault();
                if (rapidApiV3Country == null)
                {
                    rapidApiV3Country = new RapidApiV3Country
                    {
                        CountryCode = country.code,
                        Flag = country.flag,
                        CountryName = country.name,
                        CreatedDateTime = DateTime.UtcNow,
                        ModifiedDateTime = DateTime.UtcNow
                    };
                    context.RapidApiV3Countries.Add(rapidApiV3Country);
                }

                var rapidApiV3League = context.RapidApiV3Leagues.Where(a => a.Id == league.id).FirstOrDefault();
                if (rapidApiV3League == null)
                {
                    rapidApiV3League = new RapidApiV3League
                    {
                        Id = league.id,
                        Name = league.name,
                        Logo = league.logo,
                        Type = league.type,
                        CountryName = country.name,
                        CreatedDateTime = DateTime.UtcNow,
                        ModifiedDateTime = DateTime.UtcNow
                    };
                    context.RapidApiV3Leagues.Add(rapidApiV3League);
                }

                foreach (var season in seasons)
                {
                    var rapidApiV3LeagueSeason = context.RapidApiV3LeagueSeasons.Where(a => a.Year == season.year && a.RapidApiV3LeagueId == rapidApiV3League.Id).FirstOrDefault();
                    if (rapidApiV3LeagueSeason == null)
                    {
                        rapidApiV3LeagueSeason = new RapidApiV3LeagueSeason
                        {
                            RapidApiV3LeagueId = rapidApiV3League.Id,
                            Year = season.year,
                            StartDate = Convert.ToDateTime(season.start),
                            EndDate = Convert.ToDateTime(season.end),
                            Current = season.current,
                            CreatedDateTime = DateTime.UtcNow,
                            ModifiedDateTime = DateTime.UtcNow
                        };
                        context.RapidApiV3LeagueSeasons.Add(rapidApiV3LeagueSeason);
                    }
                    else
                    {
                        if (rapidApiV3LeagueSeason.Current != season.current)
                        {
                            rapidApiV3LeagueSeason.Current = season.current;
                            rapidApiV3LeagueSeason.ModifiedDateTime = DateTime.UtcNow;
                            context.RapidApiV3LeagueSeasons.AddOrUpdate(rapidApiV3LeagueSeason);
                        }
                    }
                }
            }
            context.SaveChanges();
        }

        public static void ForceDailyRapidApiLeagueCheckWithoutBetting()
        {
            // Called from the admin screen to force a faily update. This is usually automated to run once per day.
            try
            {
                var context = new ApplicationDbContext();
                PerformDailyUpdate(context, false);
                context.Dispose();
            }
            catch (Exception e)
            {
                Logger.Error("DailyRapidApiLeagueCheck ERRORED with message --> " + e.Message);
                throw;
            }
        }

        public static void ForceDailyRapidApiLeagueCheckWithBetting()
        {
            // Called from the admin screen to force a faily update. This is usually automated to run once per day.
            try
            {
                var context = new ApplicationDbContext();
                PerformDailyUpdate(context, true);
                context.Dispose();
            }
            catch (Exception e)
            {
                Logger.Error("DailyRapidApiLeagueCheck ERRORED with message --> " + e.Message);
                throw;
            }
        }

        public static void DailyRapidApiLeagueCheck()
        {
            try
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
                        // Setting not found, create it
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
                        // setting found in the db, ue this one
                        var lastDate = System.Convert.ToDateTime(siteSetting.SettingValue);

                        Logger.Info("DailyRapidApiLeagueCheck --> lastDate = {0}, Now = {1}", lastDate, DateTime.Now.Date);

                        if (lastDate < DateTime.Now.Date)
                        {

                            siteSetting.SettingValue = DateTime.Now.Date.ToLongDateString();
                            siteSetting.ModifiedDateTime = DateTime.UtcNow;
                            context.SiteSettings.AddOrUpdate(siteSetting);
                            context.SaveChanges();
                            performUpdate = true;
                        }
                    }

                    Helper.Cache.SetCachedItem(cacheId, "ReRunWhenExpired", DateTime.Now.Date.AddDays(1));

                    Logger.Info("DailyRapidApiLeagueCheck --> performUpdate = {0}", performUpdate);

                    if (performUpdate)
                    {
                        PerformDailyUpdate(context, true);
                    }
                    context.Dispose();
                }
            }
            catch (Exception e)
            {
                Logger.Error("DailyRapidApiLeagueCheck ERRORED with message --> " + e.Message);
                throw;
            }
        }

        private static void PerformDailyUpdate(ApplicationDbContext context, bool UpdateOdds)
        {
            // Run through all the leagues assocoated to rapid api and update them

            Logger.Info("DailyRapidApiLeagueCheck --> Performing daily league update");

            var v3Leagues = context.Leagues.Where(a => a.RapidApiV3LeagueSeasonId != null && a.DailyRapidApiCheck == true).ToList();
            foreach (var v3League in v3Leagues)
            {

                // Find the earliest date that has a results that has not been processed
                DateTime earliestDate = DateTime.UtcNow.Date;

                if (context.Fixtures.Any(a => a.ResultProcessed == false && a.LeagueId == v3League.Id))
                    earliestDate = context.Fixtures.Where(a => a.ResultProcessed == false && a.LeagueId == v3League.Id).Min(f => f.FixtureDateTime);

                var rapidApiV3LeagueSeason = context.RapidApiV3LeagueSeasons.Where(a => a.Id == v3League.RapidApiV3LeagueSeasonId).FirstOrDefault(); ;

                // Update the whole league for this league
                V3FixturesByLeague(rapidApiV3LeagueSeason, earliestDate.Date, v3League.Id);

                // Get the odds for this league
                if (UpdateOdds)
                    V3OddsByLeagueAndBookmaker(rapidApiV3LeagueSeason, earliestDate);

                var todayMinusTen = DateTime.UtcNow.AddDays(-10);
                var count = context.Fixtures.Count(a => a.LeagueId == v3League.Id && a.ResultProcessed == false && a.FixtureDateTime >= todayMinusTen);
                if (count == 0)
                {
                    // Wait 10 days after the last fixture. If no more fixtures, then stop checking every day.
                    v3League.DailyRapidApiCheck = false;
                    v3League.ModifiedDateTime = DateTime.UtcNow;
                    context.Leagues.AddOrUpdate(v3League);
                    context.SaveChanges();
                }

            }
            // Check if events need to be created.
            DailyRapidApiGenerateEvents(context);
            DailyRapidApiUpdateEvents(context);

        }

        public static void GenerateAndUpdateEvents()
        {
            // Called from the admin screen to force the creation and the update of events (comps)
            var context = new ApplicationDbContext();
            DailyRapidApiGenerateEvents(context);
            DailyRapidApiUpdateEvents(context);
            context.Dispose();
        }

        public static void GetRapidApiResults()
        {
            // Check to see if we have reached the time where we expect a game to be completed.
            string cacheKey = "NextFixtureCheckDateTime";
            bool checkPerformed = false;
            var rapidApiResultChecks = (List<RapidApiResultCheck>)Helper.Cache.GetCachedItem(cacheKey);
            if (rapidApiResultChecks == null)
            {
                SetNextResultCheckDateTime(false, false);
                rapidApiResultChecks = (List<RapidApiResultCheck>)Helper.Cache.GetCachedItem(cacheKey);
            }

            // V3
            foreach (var rapidApiResultCheck in rapidApiResultChecks.Where(a => a.RapidApiV3LeagueSeasonId != null))
            {

                if (rapidApiResultCheck.FixtureDateTime != DateTime.MinValue && rapidApiResultCheck.ResultCheckDateTime <= DateTime.UtcNow)
                {
                    var rapidApiV3LeagueSeason = new RapidApiV3LeagueSeason { Id = (int)rapidApiResultCheck.RapidApiV3LeagueSeasonId, Year = (int)rapidApiResultCheck.Year, RapidApiV3LeagueId = (int)rapidApiResultCheck.RapidApiV3LeagueId };

                    Logger.Info("GetRapidApiResults V3 = Getting results from rapidApiV3LeagueSeasonId = {0}. FixtureDateTime = {1}, ResultCheckDateTime = {2}, Now = {3}", rapidApiV3LeagueSeason.Id, rapidApiResultCheck.FixtureDateTime.Date, rapidApiResultCheck.ResultCheckDateTime, DateTime.UtcNow);

                    if (rapidApiResultCheck.FixtureDateTime.Date < DateTime.UtcNow.Date)
                    {
                        // If the date of the fixture is less than today then get all results.
                        RapidApiHelper.V3FixturesByLeague(rapidApiV3LeagueSeason, rapidApiResultCheck.FixtureDateTime.Date, rapidApiResultCheck.LeagueId);
                    }
                    else
                    {
                        // If the date of the fixture is today, then get the results for today only
                        RapidApiHelper.V3FixturesByLeagueByDate(rapidApiV3LeagueSeason, DateTime.UtcNow.Date, rapidApiResultCheck.LeagueId);
                    }

                    checkPerformed = true;
                }
            }

            if (checkPerformed)
                SetNextResultCheckDateTime(true, false);
        }

        public static void SetNextResultCheckDateTime(bool roundUp, bool forceUpdate)
        {
            // Get all the fixtures that are associated to events, that do not have a result
            Logger.Info("SetNextResultCheckDateTime - Start - roundUp = {0}", roundUp);

            string cacheKey = "NextFixtureCheckDateTime";
            bool updateNeeded = false;

            var rapidApiResultChecks = (List<RapidApiResultCheck>)Helper.Cache.GetCachedItem(cacheKey);
            if (forceUpdate)
            {
                updateNeeded = true;
            }
            else if (rapidApiResultChecks == null)
            {
                Logger.Info("SetNextResultCheckDateTime - rapidApiResultChecks == null");
                updateNeeded = true;
            }
            else
            {
                foreach (var rapidApiResultCheck in rapidApiResultChecks)
                {
                    // FixtureDateTime includes an addition of 1 hour 55 mins to ensure we are checking after the game has finished
                    if (rapidApiResultCheck.ResultCheckDateTime < DateTime.UtcNow)
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
                    if (roundUp && rapidApiResultCheck.ResultCheckDateTime < DateTime.UtcNow)
                        rapidApiResultCheck.ResultCheckDateTime = DateTime.UtcNow;

                    if (roundUp)
                        rapidApiResultCheck.ResultCheckDateTime = RoundUp(rapidApiResultCheck.ResultCheckDateTime, TimeSpan.FromMinutes(5));

                    Logger.Info("SetNextResultCheckDateTime - Set League {0}, fixture date {1}, next check date to {2}", rapidApiResultCheck.RapidApiLeagueId, rapidApiResultCheck.FixtureDateTime, rapidApiResultCheck.ResultCheckDateTime);
                }
                Helper.Cache.SetCachedItem(cacheKey, rapidApiResultChecks);
                context.Dispose();
            }
        }

        public static void V3FixturesByLeagueByDate(RapidApiV3LeagueSeason rapidApiV3LeagueSeason, DateTime dateToUpdate, short leagueId)
        {

            try
            {
                var resultDate = dateToUpdate.Year 
                                + "-" + dateToUpdate.Month.ToString("D2") 
                                + "-" + dateToUpdate.Day.ToString("D2");

                Logger.Info("V3FixturesByLeagueByDate - rapidApiV3LeagueSeasonId = {0}, Date = {1}, LeagueId = {2}", rapidApiV3LeagueSeason.Id, resultDate, leagueId);

                var baseUrl = RapidApiV3FixturesUrl
                    + "?date=" + resultDate
                    + "&league=" + rapidApiV3LeagueSeason.RapidApiV3LeagueId 
                    + "&season=" + rapidApiV3LeagueSeason.Year.ToString() 
                    + "&" + Timezone;

                var response = MakeRapidApiCall(baseUrl);

                if (response != null)
                    V3UpdateFixtures(response, dateToUpdate, leagueId);
            }
            catch (Exception e)
            {
                Logger.Info("ERROR --> RapidApiV3LeagueSeason - rapidApiV3LeagueSeasonId = {0}, dateToUpdate = {1}, LeagueId = {2}, error = {3}", rapidApiV3LeagueSeason.Id, dateToUpdate, leagueId, e.Message);
            }
        }

        public static void V3FixturesByLeague(RapidApiV3LeagueSeason rapidApiV3LeagueSeason, DateTime earliestTime, short leagueId)
        {

            try
            {
                Logger.Info("V3FixturesByLeague - rapidApiV3LeagueSeasonId = {0}, EarliestDate = {1}", rapidApiV3LeagueSeason.Id, earliestTime);
                var baseUrl = RapidApiV3FixturesUrl + "?league=" + rapidApiV3LeagueSeason.RapidApiV3LeagueId + "&season=" + rapidApiV3LeagueSeason.Year.ToString() +  "&" + Timezone;
                var response = MakeRapidApiCall(baseUrl);
                if (response != null)
                    V3UpdateFixtures(response, earliestTime, leagueId);
            }
            catch (Exception e)
            {
                Logger.Info("ERROR --> V3FixturesByLeague - Error = {0}",e.Message);
            }

        }

        private static void V3OddsByLeagueAndBookmaker(RapidApiV3LeagueSeason rapidApiV3LeagueSeason, DateTime earliestDateTime)
        {
            try
            {

                Logger.Info("V3OddsByLeagueAndBookmaker - rapidApiV3LeagueId = {0}", rapidApiV3LeagueSeason.RapidApiV3LeagueId);

                var pageNbr = 1;

                while (pageNbr != 0)
                {
                    var baseUrl = RapidApiV3OddsUrl + "?league=" + rapidApiV3LeagueSeason.RapidApiV3LeagueId.ToString()
                                    + "&season=" + rapidApiV3LeagueSeason.Year.ToString()
                                    + "&bookmaker=8" // 8 = Bet365
                                    + "&page=" + pageNbr;

                    var response = MakeRapidApiCall(baseUrl);
                    if (response == null)
                    {
                        pageNbr = 0;
                        Logger.Info("V3OddsByLeagueAndBookmaker (null response) - RapidApiV3LeagueId = {0}, pageNbr = {1}", rapidApiV3LeagueSeason.RapidApiV3LeagueId, pageNbr);
                    }
                    else
                    {
                        pageNbr = V3UpdateOdds(response, earliestDateTime); // This will return the next page to process. If 0, then no more pages
                        Logger.Info("V3OddsByLeagueAndBookmaker - RapidApiV3LeagueId = {0}, pageNbr = {1}", rapidApiV3LeagueSeason.RapidApiV3LeagueId, pageNbr);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR --> V3OddsByLeagueAndBookmaker - RapidApiV3LeagueId = {0}, Error = {1}", rapidApiV3LeagueSeason.RapidApiV3LeagueId, e.Message);
                throw;
            }
        }

        private static IRestResponse MakeRapidApiCall(string baseUrl)
        {

            var context = new ApplicationDbContext();
            var nbrTimesApiCalled = SettingCheck(context);

            if (nbrTimesApiCalled >= 100)
            {
                Logger.Info("MakeRapidApiCall CALLS OVER 100");
                return null;
            }

            // Set setting value
            IncrementSetting(context, nbrTimesApiCalled);
            context.Dispose();

            try
            {

                var client = new RestClient(baseUrl);
                var request = new RestRequest(Method.GET);
                request.AddHeader("x-rapidapi-host", "api-football-v1.p.rapidapi.com");
                request.AddHeader("x-rapidapi-key", "dd93656aa1msh15481f122393c01p11fd67jsnf7e74925fed3");
                IRestResponse response = client.Execute(request);

                return response;
            }
            catch (Exception e)
            {
                Logger.Error("ERROR --> MakeRapidApiCall - baseUrl = {0}, Error = {1}", baseUrl, e.Message);
                return null;
            }
        }

        private static int V3UpdateOdds(IRestResponse response, DateTime earliestDateTime)
        {
            var context = new ApplicationDbContext();
            var jsonSerializer = new JsonSerializer();
            var rapidApiOddsRoot = jsonSerializer.Deserialize<RapidApiV3OddsClassHelper.Root>(response);

            if (rapidApiOddsRoot == null)
                return 0;

            var currentPage = rapidApiOddsRoot.paging.current;
            var lastPage = rapidApiOddsRoot.paging.total;

            foreach (var rapidApiOdds in rapidApiOddsRoot.response)
            {
                var rapidApiFixtureId = rapidApiOdds.fixture.id;

                // Loop through all the bookmakers (we only want 1 bookmaker)
                foreach (var rapidApiBookmaker in rapidApiOdds.bookmakers)
                {
                    // Bookmaker 8 = bet365
                    var rapidApiBookmakerId = rapidApiBookmaker.id;
                    if (rapidApiBookmakerId != 8)
                        continue; // skip the rest of the code and get the item in the foreach loop

                    foreach (var rapidApiBet in rapidApiBookmaker.bets)
                    {
                        //label_id 1 = match winner
                        //label_id 10 = exact score

                        var rapidApiOddsLabelId = rapidApiBet.id;

                        if (rapidApiOddsLabelId == 1)
                            ProcessMatchWinnerOdds(context, rapidApiFixtureId, rapidApiBet.values);

                        if (rapidApiOddsLabelId == 10)
                            ProcessExactScoreOdds(context, rapidApiFixtureId, rapidApiBet.values);

                    }

                }
            }
            context.Dispose();

            // return the next page to process. 0 means stop processing
            if (currentPage == lastPage)
            {
                return 0;
            }
            else
            {
                return currentPage + 1;
            }
        }

        private static void ProcessExactScoreOdds(ApplicationDbContext context, int rapidApiFixtureId, List<RapidApiV3OddsClassHelper.Value> rapidApiBetValues)
        {
            
            foreach (var rapidApiBetValue in rapidApiBetValues)
            {
                var betValue = rapidApiBetValue.value.ToString();
                var odds = System.Convert.ToDecimal(rapidApiBetValue.odd);
                var addOrUpdate = false;

                string[] betSplit = betValue.Split(':');
                var homeScore = System.Convert.ToInt16(betSplit[0]);
                var awayScore = System.Convert.ToInt16(betSplit[1]);

                var fixtureOddsByScore = context.FixtureOddsByScores.FirstOrDefault(f => f.RapidApiFixtureId == rapidApiFixtureId
                                        && f.HomeScore == homeScore && f.AwayScore == awayScore);

                if (fixtureOddsByScore == null)
                {
                    fixtureOddsByScore = new FixtureOddsByScore
                    {
                        RapidApiFixtureId = rapidApiFixtureId
                        , HomeScore = homeScore
                        , AwayScore = awayScore
                        , Odds = odds
                        , CreatedDateTime = DateTime.UtcNow
                    };
                    addOrUpdate = true;
                }
                else
                {
                    if (fixtureOddsByScore.Odds != odds)
                    {
                        fixtureOddsByScore.Odds = odds;
                        addOrUpdate = true;
                    }

                }

                // If a new record or a change to the existing record, then save to the db
                if (addOrUpdate)
                {
                    fixtureOddsByScore.ModifiedDateTime = DateTime.UtcNow;
                    context.FixtureOddsByScores.AddOrUpdate(fixtureOddsByScore);
                }
            }

            context.SaveChanges();
        }

        private static void ProcessMatchWinnerOdds(ApplicationDbContext context, int rapidApiFixtureId, List<RapidApiV3OddsClassHelper.Value> rapidApiBetValues)
        {
            var addOrUpdate = false;
            var fixtureOddsByResult = context.FixtureOddsByResults.FirstOrDefault(f => f.RapidApiFixtureId == rapidApiFixtureId);
            if (fixtureOddsByResult == null)
            {
                fixtureOddsByResult = new FixtureOddsByResult
                {
                    RapidApiFixtureId = rapidApiFixtureId,
                    CreatedDateTime = DateTime.UtcNow,

                };
                addOrUpdate = true;
            }

            // Loop through the 3 different results and update the relevant property
            foreach (var rapidApiBetValue in rapidApiBetValues)
            {
                var betValue = rapidApiBetValue.value.ToString();
                if (betValue == "Home")
                {
                    // If an existing record and the odds have changed then ensure addOrUpdate is set to true to save to the dv
                    if (!addOrUpdate && fixtureOddsByResult.HomeOdds != System.Convert.ToDecimal(rapidApiBetValue.odd))
                        addOrUpdate = true;

                    fixtureOddsByResult.HomeOdds = System.Convert.ToDecimal(rapidApiBetValue.odd);
                }
                else if (betValue == "Away")
                {

                    // If an existing record and the odds have changed then ensure addOrUpdate is set to true to save to the dv
                    if (!addOrUpdate && fixtureOddsByResult.AwayOdds != System.Convert.ToDecimal(rapidApiBetValue.odd))
                        addOrUpdate = true;

                    fixtureOddsByResult.AwayOdds = System.Convert.ToDecimal(rapidApiBetValue.odd);
                }
                else if (betValue == "Draw")
                {

                    // If an existing record and the odds have changed then ensure addOrUpdate is set to true to save to the dv
                    if (!addOrUpdate && fixtureOddsByResult.DrawOdds != System.Convert.ToDecimal(rapidApiBetValue.odd))
                        addOrUpdate = true;

                    fixtureOddsByResult.DrawOdds = System.Convert.ToDecimal(rapidApiBetValue.odd);
                }
            }

            if (addOrUpdate)
            {
                fixtureOddsByResult.ModifiedDateTime = DateTime.UtcNow;
                context.FixtureOddsByResults.AddOrUpdate(fixtureOddsByResult);
                context.SaveChanges();
            }

        }
      
        private static void V3UpdateFixtures(IRestResponse response, DateTime earliestDate, short leagueId)
        {
            Logger.Info("V3UpdateFixtures - earliestDate = {0}, leagueId = {1}", earliestDate, leagueId);

            var context = new ApplicationDbContext();
            var jsonSerializer = new JsonSerializer();
            var rapidApiV3Fixtures = jsonSerializer.Deserialize<RapidApiV3FixtureClassHelper.Root>(response);
            var fixtures = context.Fixtures.Where(a => a.LeagueId==leagueId && a.FixtureDateTime >= earliestDate).ToList();
            var newResultFound = false;
            var eventsWithChangedFixtureDateTime = new List<short>();

            // loop through all fixtures in the future
            foreach (var rapidApiV3Fixture in rapidApiV3Fixtures.response.Where(f =>
                f.fixture.date >= earliestDate))
            {

                var homeTeam = AddOrUpdateTeam(context, rapidApiV3Fixture.teams.home.id,
                    rapidApiV3Fixture.teams.home.name, rapidApiV3Fixture.teams.home.logo);
                var awayTeam = AddOrUpdateTeam(context, rapidApiV3Fixture.teams.away.id,
                    rapidApiV3Fixture.teams.away.name, rapidApiV3Fixture.teams.away.logo);
                var rapidApiFixtureId = rapidApiV3Fixture.fixture.id;


                // Check if the fixture needs updating
                var updateDb = false;

                // Change time to UTC
                rapidApiV3Fixture.fixture.date = rapidApiV3Fixture.fixture.date.ToUniversalTime();

                var fixture = fixtures.FirstOrDefault(f => f.RapidApiFixtureId == rapidApiFixtureId);
                if (rapidApiV3Fixture.fixture.status.@long == "Match Postponed" || (rapidApiV3Fixture.fixture.status.@long == "Not Started" &&
                                                                    rapidApiV3Fixture.fixture.date.AddHours(3) <
                                                                    DateTime.UtcNow)
                ) // match was postponed but kept at not started status
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
                    if (fixture == null)
                    {
                        fixture = new Models.Fixture
                        {
                            RapidApiFixtureId = rapidApiFixtureId,
                            HomeTeamId = homeTeam.Id,
                            AwayTeamId = awayTeam.Id,
                            LeagueId = leagueId,
                            FixtureDateTime = rapidApiV3Fixture.fixture.date,
                            CreatedDateTime = DateTime.UtcNow,
                            ModifiedDateTime = DateTime.UtcNow
                        };
                        updateDb = true;
                    }
                    else
                    {
                        // Existing fixture
                        if (fixture.FixtureDateTime != rapidApiV3Fixture.fixture.date)
                        {
                            // Date has changed
                            fixture.FixtureDateTime = rapidApiV3Fixture.fixture.date;
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

                    if (fixture.HomeResult == null && rapidApiV3Fixture.fixture.status.@long == "Match Finished")
                    {
                        newResultFound = true;
                        updateDb = true;
                        fixture.HomeResult = (short)rapidApiV3Fixture.score.fulltime.home;
                        fixture.AwayResult = (short)rapidApiV3Fixture.score.fulltime.away;
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

        private static Team AddOrUpdateTeam(ApplicationDbContext context, int rapidApiTeamId, string teamName, string logo)
        {
            var team = context.Teams.FirstOrDefault(t => t.RapidApiTeamId == rapidApiTeamId);
            teamName = teamName.Replace(" United", " Utd");
            teamName = teamName.Replace(" Wednesday", " Wed");

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
                // Not updating the logo as this could be changed manually by me
                if (team.TeamName != teamName || team.FlagFileLocation != logo)
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

            context.SaveChanges();
        }

        private static int GetSiteSetting(ApplicationDbContext context, string settingName)
        {
            var siteSetting = context.SiteSettings.FirstOrDefault(f => f.SettingName == settingName);
            if (siteSetting == null)
                return 0;

            var numericValue = System.Convert.ToInt32(siteSetting.SettingValue);

            return numericValue;
        }

        public static int SettingCheck(ApplicationDbContext context)
        {
            var siteSettingName = "RapidApi_Fixtures" + DateTime.UtcNow.Year.ToString() + "_" + DateTime.UtcNow.Month.ToString() + "_" + DateTime.UtcNow.Day.ToString();
            return GetSiteSetting(context, siteSettingName);

        }
       
        private static void IncrementSetting(ApplicationDbContext context, int nbrTimesApiCalled)
        {
            var siteSettingName = "RapidApi_Fixtures" + DateTime.UtcNow.Year.ToString() + "_" + DateTime.UtcNow.Month.ToString() + "_" + DateTime.UtcNow.Day.ToString();
            SaveSiteSetting(context, siteSettingName, nbrTimesApiCalled + 1);
        }

        private static void DailyRapidApiUpdateEvents(ApplicationDbContext context)
        {
            // Check events that have been generated to see if any fixtures need to be added/removed
            var endDateToUse = DateTime.UtcNow.Date; // Linq doesnt like AddDays. Add 1 day to include fixtures with that date date
            var minDate = DateTime.MinValue;
            var eventGenerations = context.EventGenerations.Include(a => a.LeagueEventGeneration)
                                    .Where(a => a.LeagueEventGeneration.Enabled == true
                                     && (a.BaseEndDate >= endDateToUse | a.BaseEndDate == minDate)).ToList();

            foreach (var eventGeneration in eventGenerations)
            {
                // If generation type is time bound, then add any new fixtures that are now in scope and remove those that arent.
                var fixturesRemoved = false;
                var generationFrequencyId = eventGeneration.LeagueEventGeneration.GenerationFrequencyId;
                if (generationFrequencyId==1|| generationFrequencyId==2 || generationFrequencyId==3)
                {
                    fixturesRemoved = CheckAndRemoveFixturesForEvent(context, eventGeneration);
                }

                // For all generation types, check if there are any fixtures to add
                if (CheckAndAddFixturesForEvent(context, eventGeneration) | fixturesRemoved)
                {
                    Helper.Cache.UpdateEventStartEnd(context, eventGeneration.EventId);
                };
            }
        }
        
        private static bool CheckAndAddFixturesForEvent(ApplicationDbContext context, EventGeneration eventGeneration)
        {

            List<Fixture> fixtures;
            var existingFixtures = context.EventFixtures.Where(a => a.EventId == eventGeneration.EventId
                && a.Fixture.ResultProcessed == false).Select(a => a.Fixture.Id).ToList();

            if (eventGeneration.LeagueEventGeneration.GenerationFrequencyId==1
                || eventGeneration.LeagueEventGeneration.GenerationFrequencyId == 2
                || eventGeneration.LeagueEventGeneration.GenerationFrequencyId == 3)
            {
                var endDateToUse = eventGeneration.BaseEndDate.AddDays(1); // Linq doesnt like AddDays. Add 1 day to include fixtures with that date date

                fixtures = context.Fixtures.Where(a => a.ResultProcessed == false
                           && a.LeagueId == eventGeneration.LeagueEventGeneration.League.Id
                           && a.FixtureDateTime >= eventGeneration.BaseStartDate
                           && a.FixtureDateTime < endDateToUse
                           && !existingFixtures.Contains(a.Id))
                            .OrderBy(a => a.FixtureDateTime).ToList();

            }
            else if(eventGeneration.LeagueEventGeneration.TeamId!=null)
            {
                fixtures = context.Fixtures.Where(a => a.ResultProcessed == false
                           && a.LeagueId == eventGeneration.LeagueEventGeneration.League.Id
                           && !existingFixtures.Contains(a.Id)
                           && (a.HomeTeamId == eventGeneration.LeagueEventGeneration.TeamId | a.AwayTeamId == eventGeneration.LeagueEventGeneration.TeamId))
                            .OrderBy(a => a.FixtureDateTime).ToList();
            }
            else
            {

                fixtures = context.Fixtures.Where(a => a.ResultProcessed == false
                           && a.LeagueId == eventGeneration.LeagueEventGeneration.League.Id
                           && !existingFixtures.Contains(a.Id))
                            .OrderBy(a => a.FixtureDateTime).ToList();
            }

            if(fixtures.Count>0)
            {
                // Add new fixtures
                foreach(var fixture in fixtures)
                {
                    var eventFixture = new EventFixture
                    {
                        FixtureId = fixture.Id
                        ,CreatedDateTime = DateTime.UtcNow
                        ,ModifiedDateTime = DateTime.UtcNow
                        ,EventId = eventGeneration.EventId
                    };
                    context.EventFixtures.Add(eventFixture);
                }

                context.SaveChanges();

                // If a generation has been made with a fixture for today, then update the config to check for the result
                var earliestFixture = fixtures.Select(a => a.FixtureDateTime).Min();
                if (earliestFixture.Date <= DateTime.UtcNow.Date)
                    SetNextResultCheckDateTime(false, true);

                Logger.Info("CheckAndAddFixturesForEvent {0} Fixtures added for EventId {1} ", fixtures.Count, eventGeneration.EventId);
                return true; // a change has been made
            }
            return false;

        }
        
        private static bool CheckAndRemoveFixturesForEvent(ApplicationDbContext context, EventGeneration eventGeneration)
        {
            // Check if any fixtures associated to the event are out of range. If so, remove.
            var endDateToUse = eventGeneration.BaseEndDate.AddDays(1);
            var fixturesOutOfScope = context.EventFixtures.Where(a => a.Fixture.LeagueId == eventGeneration.LeagueEventGeneration.League.Id
                                        && a.EventId == eventGeneration.EventId
                                        && (a.Fixture.FixtureDateTime >= endDateToUse | a.Fixture.FixtureDateTime < eventGeneration.BaseStartDate)
                                        && a.Fixture.ResultProcessed == false).Select(a => a.Fixture.Id).ToList();
            if (fixturesOutOfScope.Count>0)
            {

                // remove any predictions
                context.FixturePredictions.RemoveRange(context.FixturePredictions
                                .Where(a => a.EventId == eventGeneration.EventId 
                                && fixturesOutOfScope.Contains(a.FixtureId)));

                context.EventFixtures.RemoveRange(context.EventFixtures
                                .Where(a => a.EventId == eventGeneration.EventId
                                && fixturesOutOfScope.Contains(a.FixtureId)));

                Logger.Info("CheckAndRemoveFixturesForEvent {0} Fixtures removed for EventId {1} ", fixturesOutOfScope.Count, eventGeneration.EventId);

                context.SaveChanges();
                Helper.Cache.UpdateEventStartEnd(context, eventGeneration.EventId);

                return true; // a change has bene made
            }
            return false;

        }

        private static void DailyRapidApiGenerateEvents(ApplicationDbContext context)
        {
            // Check if we need to generate events. 
            // AutoGenerateEvents --> 1 = Weekly, 2 = twice per month, 3 = Monthly, 11 = League Season, 21 = Team/League Season
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

                var weeklyWeeks = 1;

                var firstFixtureForLeague = context.Fixtures.Where(a => a.LeagueId == leagueEventGeneration.League.Id)
                    .OrderBy(a => a.FixtureDateTime)
                    .FirstOrDefault();

                var lastFixtureForLeague = context.Fixtures.Where(a => a.LeagueId == leagueEventGeneration.League.Id)
                    .OrderByDescending(a => a.FixtureDateTime)
                    .FirstOrDefault();

                if (firstFixtureForLeague != null && lastFixtureForLeague != null)
                {
                    var dateToCheck = firstFixtureForLeague.FixtureDateTime.Date;
                    var endDate = lastFixtureForLeague.FixtureDateTime.Date;

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
                            // If the first fixture is in the future, then set up the first event to be to 16 or end of month
                            if (dateToCheck.Day <= 16)
                            {
                                endDate = dateToCheck.AddDays(16 - dateToCheck.Day);
                            }
                            else
                            {
                                endDate = new DateTime(dateToCheck.AddMonths(1).Year, dateToCheck.AddMonths(1).Month, 1).AddDays(-1);
                            }
                        }
                        else
                        {
                            // Get next 1st of month, or 16th of month
                            if (DateTime.UtcNow.Day <= 16)
                            {
                                // 16th of current month
                                dateToCheck = DateTime.UtcNow.Date.AddDays(16 - DateTime.UtcNow.Date.Day);
                                endDate = new DateTime(dateToCheck.AddMonths(1).Year, dateToCheck.AddMonths(1).Month, 1).AddDays(-1);
                            }
                            else
                            {
                                // first of next month
                                dateToCheck = new DateTime(DateTime.UtcNow.AddMonths(1).Year, DateTime.UtcNow.AddMonths(1).Month, 1).Date;
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

                    if (dateToCheck > lastFixtureForLeague.FixtureDateTime.Date && lastFixtureForLeague.FixtureDateTime.Date < DateTime.UtcNow.AddDays(-14))
                    {
                        // There are no fixtures left. Disable any further generations . Wait 14 days in case there are additional fixtures like play offs
                        leagueEventGeneration.Enabled = false;
                        leagueEventGeneration.ModifiedDateTime = DateTime.UtcNow;
                    }
                    else
                    {
                        var eventExists = false;
                        if (leagueEventGeneration.GenerationFrequencyId == 1
                            || leagueEventGeneration.GenerationFrequencyId == 2
                            || leagueEventGeneration.GenerationFrequencyId == 3)
                        {
                            eventExists = context.EventGenerations.Any(a => a.LeagueEventGenerationId == leagueEventGeneration.Id
                                                && a.BaseStartDate == dateToCheck
                                                && a.BaseEndDate == endDate);
                        }
                        else
                        {
                            // Should only ever be 1
                            eventExists = context.EventGenerations.Any(a => a.LeagueEventGenerationId == leagueEventGeneration.Id);
                        }

                        if (!eventExists)
                        {
                            //Event hasnt been created. Create it and assign the fixtures.
                            string eventName = CalcEventName(context, leagueEventGeneration, weeklyWeeks, dateToCheck, endDate);
                            CreateEvent(leagueEventGeneration, dateToCheck, endDate, eventName, context);
                        }

                    }

                }

            }

        }

        private static string CalcEventName(ApplicationDbContext context, LeagueEventGeneration leagueEventGeneration, int weeklyWeeks, DateTime dateToCheck, DateTime endDate)
        {
            var eventName = leagueEventGeneration.League.ShortLeagueName;

            if (leagueEventGeneration.GenerationFrequencyId == 1) // Weekly
            {
                eventName = eventName + " Week " + weeklyWeeks.ToString();
            }
            else if (leagueEventGeneration.GenerationFrequencyId == 2) // 2 per month
            {
                eventName = eventName + " " + dateToCheck.ToString("MMM") + " " + dateToCheck.Day + " - " + endDate.Day;
            }
            else if (leagueEventGeneration.GenerationFrequencyId == 3) // Monthly
            {
                eventName = eventName + " " + dateToCheck.ToString("MMM");
            }
            else if (leagueEventGeneration.GenerationFrequencyId == 11) // All fixtures for a league
            {
                eventName = leagueEventGeneration.League.LeagueName;
            }
            else if (leagueEventGeneration.GenerationFrequencyId == 21) // All fixtures for a league/team
            {
                var teamName = context.Teams.Where(a => a.Id == leagueEventGeneration.TeamId).FirstOrDefault().TeamName;
                eventName = teamName;

                var rapidApiV3LeagueSeason = context.RapidApiV3LeagueSeasons
                        .Include(a => a.RapidApiV3League)
                        .Where(a => a.Id == leagueEventGeneration.League.RapidApiV3LeagueSeasonId)
                        .FirstOrDefault();

                var year = rapidApiV3LeagueSeason.Year.ToString();
                var nextyear = (rapidApiV3LeagueSeason.Year + 1).ToString();
                eventName = eventName + " " + (rapidApiV3LeagueSeason.RapidApiV3League.Type == "League" ? year.Substring(year.Length - 2) + "/" + nextyear.Substring(nextyear.Length - 2) : year);

                if(eventName.Length>26)
                {
                    //Reduce size of event name to fit in menu
                    eventName = teamName + " " + (rapidApiV3LeagueSeason.RapidApiV3League.Type == "League" ? year.Substring(year.Length - 2) + "/" + nextyear.Substring(nextyear.Length - 2) : year);
                }
            }

            return eventName;
        }

        private static void CreateEvent(LeagueEventGeneration leagueEventGeneration, DateTime startDate, DateTime endDate, string eventName, ApplicationDbContext context)
        {

            var endDateToUse = endDate.AddDays(1); // Linq doesnt like AddDays. Add 1 day to include fixtures with that date date
            var fixtures = new List<Fixture>();
            var useDefaultDates = true;

            if (leagueEventGeneration.GenerationFrequencyId == 1
            || leagueEventGeneration.GenerationFrequencyId == 2
            || leagueEventGeneration.GenerationFrequencyId == 3)
            {
                fixtures = context.Fixtures.Where(a => a.ResultProcessed == false
                           && a.LeagueId == leagueEventGeneration.League.Id
                           && a.FixtureDateTime >= startDate
                           && a.FixtureDateTime < endDateToUse)
                            .OrderBy(a => a.FixtureDateTime).ToList();

                useDefaultDates = false;
            }
            else if(leagueEventGeneration.TeamId!=null)
            {
                fixtures = context.Fixtures.Where(a => a.ResultProcessed == false
                           && a.LeagueId == leagueEventGeneration.League.Id
                           && (a.HomeTeamId == leagueEventGeneration.TeamId | a.AwayTeamId == leagueEventGeneration.TeamId))
                            .OrderBy(a => a.FixtureDateTime).ToList();

            }
            else
            {
                fixtures = context.Fixtures.Where(a => a.ResultProcessed == false
                                           && a.LeagueId == leagueEventGeneration.League.Id)
                                            .OrderBy(a => a.FixtureDateTime).ToList();
            }



            // If no fixtures available, then exit
            if (!fixtures.Any())
                return;

            // Find the default pool
            var defaultPool = context.Pools.First(a => a.DefaultPoolForEvent == true);

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
                DefaultPoolId = defaultPool.Id,
                Fixtures = fixtures.Count
            };
            context.Events.Add(myEvent);
            context.SaveChanges();

            Helper.Cache.SetEventCache();

            var myEventPool = new EventPool
            {
                PoolId = defaultPool.Id,
                EventId = myEvent.Id,
                CreatedDateTime = DateTime.UtcNow,
                Enabled = true,
                ModifiedDateTime = DateTime.UtcNow
            };

            context.EventPools.Add(myEventPool);

            var myEventGeneration = new EventGeneration
            {
                EventId = myEvent.Id,
                CreatedDateTime = DateTime.UtcNow,
                ModifiedDateTime = DateTime.UtcNow,
                BaseStartDate = (useDefaultDates==true? DateTime.MinValue: startDate),
                BaseEndDate = (useDefaultDates == true ? DateTime.MinValue : endDate),
                LeagueEventGenerationId = leagueEventGeneration.Id
            };

            context.EventGenerations.Add(myEventGeneration);

            foreach (var fixture in fixtures)
            {
                var eventFixture = new EventFixture
                {
                    EventId = myEvent.Id,
                    FixtureId = fixture.Id,
                    CreatedDateTime = DateTime.UtcNow,
                    ModifiedDateTime = DateTime.UtcNow

                };
                context.EventFixtures.Add(eventFixture);
            }

            context.SaveChanges();

            // If a generation has been made with a fixture for today, then update the config to check for the result
            var earliestFixture = fixtures.Select(a => a.FixtureDateTime).Min();
            if(earliestFixture.Date <= DateTime.UtcNow.Date)
                SetNextResultCheckDateTime(false, true);
        }
        
        private static DateTime RoundUp(DateTime dt, TimeSpan d)
        {
            // sets to the next 5 minute (rounded) time i.e. 9:57 will round to 10:00, 9:58 will round to 10:00
            return new DateTime((dt.Ticks + d.Ticks - 1) / d.Ticks * d.Ticks, dt.Kind);
        }
    }

}