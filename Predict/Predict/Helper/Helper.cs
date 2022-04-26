using Microsoft.AspNet.Identity;
using Predict.Models;
using Predict.RapidApi;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Migrations;
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
            Logger.Info("SetCachedItem = setting cache for {0}, with Expiry Date of {1}", cacheId, expiryDate);
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

        public static void SetEventCache()
        {

            Logger.Info("SetEventCache - Start");
            var daysAgo = DateTime.UtcNow.AddDays(-30);
            var context = new ApplicationDbContext();
            var events = context.Events.Where(a => a.EndDateTime >= daysAgo || a.EndDateTime == DateTime.MinValue).ToList();
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
                if(myOriginalEventIndex==-1)
                {
                    myEvents.Add(myNewEvent);
                }
                else
                {
                    myEvents[myOriginalEventIndex] = myNewEvent;
                }

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