using Microsoft.AspNet.Identity;
using Predict.Models;
using Predict.RapidApi;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Runtime.Caching;

namespace Predict.Helper
{


    // this class is for perm data. Not user specific.
    public static class Cache
    {

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static void SendEmail(IdentityMessage message)
        {
            var smtpMessage = new MailMessage
            {
                From = new MailAddress(ConfigurationManager.AppSettings["SupportEmailAddr"])
            };
            smtpMessage.To.Add(new MailAddress(message.Destination));
            smtpMessage.Bcc.Add(new MailAddress("pete@salsbury.co.uk"));
            smtpMessage.Subject = message.Subject;
            smtpMessage.Body = message.Body;
            smtpMessage.IsBodyHtml = true;

            var client = new SmtpClient();
            if (Environment.MachineName == "PETESXPS")
            {
                client.Host = "ignored";
                client.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                client.PickupDirectoryLocation = @"C:\repos\Predict\predictemails";
            }

            client.Send(smtpMessage);
        }

        public static void RemoveCachedItem(string cacheId)
        {
            if (MemoryCache.Default.Contains(cacheId))
            {
                MemoryCache.Default.Remove(cacheId);
            }
        }

        public static object GetCachedItem(string cacheId)
        {
            if (!MemoryCache.Default.Contains(cacheId))
            {
                switch (cacheId)
                {
                    case "Events":
                        SetEventCache();
                        break;
                    default:
                        break;
                }

                return null;
            }
            return MemoryCache.Default.Get(cacheId);
        }

        public static void SetCachedItem(string cacheId, object cachedItem)
        {
            Logger.Info("SetCachedItem = setting cache for {0}", cacheId);
            SetCachedItem(cacheId, cachedItem, DateTime.UtcNow.AddDays(30));
        }

        public static void SetCachedItem(string cacheId, object cachedItem, DateTime expiryDate)
        {
            Logger.Info("SetCachedItem = setting cache for {0}", cacheId);
            MemoryCache.Default.Set(cacheId, cachedItem, expiryDate);
        }

        public static Event GetCachedEvent(int eventId)
        {
            var myEvents = (List<Event>)GetCachedItem("Events");
            var myEvent = myEvents.FirstOrDefault(e => e.Id == eventId);
            return myEvent;
        }

        public static void UpdateEventStartEnd(ApplicationDbContext context, short eventId)
        {
            var eventParam = new SqlParameter("@intEventId", eventId);
            context.Database.ExecuteSqlCommand("EXEC spUpdateEventStartEnd @intEventId", eventParam);

            // update the application cache for events
            SetEventCache(eventId);
        }

        public static DateTime GetNextEventStartDate()
        {
            var nextDateTime = DateTime.Today.AddDays(365);
            var myEvents = (List<Event>)GetCachedItem("Events");
            foreach (Event myEvent in myEvents)
            {
                if (myEvent.StartDateTime > DateTime.UtcNow)
                {
                    if (nextDateTime == DateTime.Today.AddDays(365))
                    {
                        nextDateTime = myEvent.StartDateTime;
                    }
                    else
                    {
                        if (myEvent.StartDateTime < nextDateTime)
                        {
                            nextDateTime = myEvent.StartDateTime;
                        }
                    }
                }

            }
            return nextDateTime;
        }

        public static bool HasEventStarted(short eventId)
        {
            var myEvents = (List<Event>)GetCachedItem("Events");
            var myEvent = myEvents.FirstOrDefault(e => e.Id == eventId);
            if (myEvent != null)
            {
                if (myEvent.StartDateTime < DateTime.UtcNow)
                {
                    return true;
                }
            }

            return false;
        }

        public static DateTime RoundUp(DateTime dt, TimeSpan d)
        {
            return new DateTime((dt.Ticks + d.Ticks - 1) / d.Ticks * d.Ticks, dt.Kind);
        }

        public static void GetRapidApiResults()
        {
            Logger.Info("GetRapidApiResults - Start");

            string cacheKey = "NextFixtureCheckDateTime";
            bool checkPerformed = false;
            var rapidApiResultChecks = (List<RapidApiResultCheck>)GetCachedItem(cacheKey);
            if (rapidApiResultChecks == null)
            {
                Logger.Info("GetRapidApiResults = rapidApiResultChecks == null");
                SetNextResultCheckDateTime(false);
                rapidApiResultChecks = (List<RapidApiResultCheck>)GetCachedItem(cacheKey);
            }
            else
            {
                Logger.Info("GetRapidApiResults = rapidApiResultChecks has {0} items", rapidApiResultChecks.Count);
            }

            foreach (var rapidApiResultCheck in rapidApiResultChecks)
            {
                if (rapidApiResultCheck.FixtureDateTime != DateTime.MinValue && rapidApiResultCheck.FixtureDateTime <= DateTime.UtcNow)
                {
                    var rapidApiLeagueId = rapidApiResultCheck.RapidApiLeagueId;

                    Logger.Info("GetRapidApiResults = Getting results from RapidApi {0}", rapidApiLeagueId);

                    if (rapidApiResultCheck.FixtureDateTime.Date < DateTime.Today)
                    {
                        // If the date of the fixture is less than today then get all results
                        RapidApiHelper.UpdateRapidApiLeague(rapidApiLeagueId);
                    }
                    else
                    {
                       // If the date of the fixture is today, then get the results for today only
                       RapidApiHelper.UpdateRapidApiLeagueByDate(rapidApiLeagueId, DateTime.UtcNow);
                    }

                    checkPerformed = true;
                }
                else
                {
                    Logger.Info("GetRapidApiResults = Waiting for result. Time now is {0}, time to wait for is {1}", DateTime.UtcNow, rapidApiResultCheck.FixtureDateTime);
                }
            }

            if (checkPerformed)
                SetNextResultCheckDateTime(true);
        }

        public static void SetNextResultCheckDateTime(bool roundUp)
        {
            // Get all the fixtures that are associated to events, that do not have a result
            Logger.Info("SetNextResultCheckDateTime - Start - roundUp = {0}", roundUp);

            string cacheKey = "NextFixtureCheckDateTime";
            bool updateNeeded = false;

            var rapidApiResultChecks = (List<RapidApiResultCheck>)GetCachedItem(cacheKey);
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

                    if(roundUp)
                        fixtureDateTime = RoundUp(fixtureDateTime, TimeSpan.FromMinutes(5));

                    rapidApiResultCheck.FixtureDateTime = fixtureDateTime;

                    Logger.Info("SetNextResultCheckDateTime - Set League {0} next check date to {1}", rapidApiResultCheck.RapidApiLeagueId, rapidApiResultCheck.FixtureDateTime);
                }
                SetCachedItem(cacheKey, rapidApiResultChecks);
                context.Dispose();
            }
        }

        public static void SetEventCache()
        {

            Logger.Info("SetEventCache - Start");

            var context = new ApplicationDbContext();

            var events = context.Events.ToList();
            SetCachedItem("Events", events);

            context.Dispose();
        }
        public static void SetEventCache(short eventId)
        {
            // Sets/Updates one specific event
            var myEvents = (List<Event>)GetCachedItem("Events");
            if (myEvents == null)
            {
                SetEventCache();
                return;
            }

            var context = new ApplicationDbContext();
            var myNewEvent = context.Events.SingleOrDefault(a => a.Id == eventId);
            if (myNewEvent != null)
            {
                var myOriginalEventIndex = myEvents.FindIndex(e => e.Id == eventId);
                myEvents[myOriginalEventIndex] = myNewEvent;
            }
            SetCachedItem("Events", myEvents);
            context.Dispose();
        }

        public static void UpdateScoring(ApplicationDbContext context)
        {
            // Run stored procedure to update all scoring
            var today = DateTime.Today;
            var todayParam = new SqlParameter("@dteDate", today);
            context.Database.ExecuteSqlCommand("EXEC spProcessScores @dteDate", todayParam);

            // Update the cache for this event
            SetEventCache();
        }


    }
}