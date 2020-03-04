using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Runtime.Caching;
using Microsoft.AspNet.Identity;
using Predict.Models;

namespace Predict.Helper
{
    // this class is for perm data. Not user specific.
    public static class Cache
    {
        public static void SendEmail(IdentityMessage message)
        {
            MailMessage smtpMessage = new MailMessage
            {
                From = new MailAddress(ConfigurationManager.AppSettings["SupportEmailAddr"])
            };
            smtpMessage.To.Add(new MailAddress(message.Destination));
            smtpMessage.Subject = message.Subject;
            smtpMessage.Body = message.Body;
            smtpMessage.IsBodyHtml = true;

            SmtpClient client = new SmtpClient();
            if (System.Environment.MachineName == "THINKPAD")
            {
                client.Host = "ignored";
                client.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                client.PickupDirectoryLocation = @"c:\predictemails";
            }
            client.Send(smtpMessage);
        }
    
        public static object GetCachedItem(string cacheId)
        {            
            return MemoryCache.Default.Get(cacheId);

        }
        public static void SetCachedItem(string cacheId, object cachedItem)
        {
            MemoryCache.Default.Add(cacheId, cachedItem, DateTime.Now.AddDays(1));
        }

        public static string GetEventName()
        {
            var currentEvent = (Event) GetCachedItem("Event");
            if (currentEvent == null)
            {
                SetGlobalCache();
                currentEvent = (Event) GetCachedItem("Event");
            }
            return currentEvent.EventName;
        }

        public static bool IsInLockDown()
        {
            var predictionsLockDown = GetCachedItem("PredictionsLockDownDateTime");
            if (predictionsLockDown == null)
            {
                SetGlobalCache();
                predictionsLockDown = GetCachedItem("PredictionsLockDownDateTime");
            }
            var predictionsLockDownDateTime = Convert.ToDateTime(predictionsLockDown);
            if (predictionsLockDownDateTime < DateTime.Today.ToUniversalTime())
            {
                return true;
            }
            return false;
        }

        public static DateTime GetLastFixtureDate()
        {
            var lastFixture = GetCachedItem("LastFixture");
            if (lastFixture == null)
            {
                SetGlobalCache();
                lastFixture = GetCachedItem("LastFixture");
            }
            var lastFixtureDatetime = Convert.ToDateTime(lastFixture);
            return lastFixtureDatetime;
        }

        public static DateTime GetFirstFixtureDate()
        {
            var firstFixture = GetCachedItem("PredictionsLockDownDateTime");
            if (firstFixture == null)
            {
                SetGlobalCache();
                firstFixture = GetCachedItem("PredictionsLockDownDateTime");
            }
            var firstFixtureDateTime = Convert.ToDateTime(firstFixture);
            return firstFixtureDateTime;
        }


        public static bool HasLastFixturePassed()
        {
            var predictionsLockDown = GetCachedItem("LastFixture");
            if (predictionsLockDown == null)
            {
                SetGlobalCache();
                predictionsLockDown = GetCachedItem("LastFixture");
            }
            var predictionsLockDownDateTime = Convert.ToDateTime(predictionsLockDown);
            if (predictionsLockDownDateTime < DateTime.Today.ToUniversalTime())
            {
                return true;
            }
            return false;
        }

        
        public static short GetEventId()
        {
            var currentEvent = (Event)GetCachedItem("Event");
            if (currentEvent == null)
            {
                SetGlobalCache();
                currentEvent = (Event)GetCachedItem("Event");
            }

            return currentEvent.Id;
        }
        public static int GetGlobalPoolId()
        {
            var globalPoolId = GetCachedItem("GlobalPoolId");
            if (globalPoolId == null)
            {
                SetGlobalCache();
                globalPoolId = GetCachedItem("GlobalPoolId");
            }

            globalPoolId = (globalPoolId ?? 1); // ?? use p, but if null use 1
            return (int)globalPoolId;
        }

        private static void SetGlobalCache()
        {
            var context = new ApplicationDbContext();
            var eventId = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["EventId"]);
            var globalPoolId = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["GlobalPoolId"]);
            var predictionsLockDown = context.Fixtures.Where(e => e.EventId == eventId).Min(f => f.FixtureDateTime);
            var lastFixture = context.KoFixtures.Where((e => e.EventId == eventId)).Max(f => f.FixtureDateTime);
            if (predictionsLockDown > lastFixture)
            {
                lastFixture = predictionsLockDown;
            }

            Helper.Cache.SetCachedItem("Event", context.Events.Single(e=> e.Id == eventId));
            Helper.Cache.SetCachedItem("GlobalPoolId", globalPoolId);
            Helper.Cache.SetCachedItem("PredictionsLockDownDateTime", predictionsLockDown);
            Helper.Cache.SetCachedItem("LastFixture", lastFixture);

        }
    }

}