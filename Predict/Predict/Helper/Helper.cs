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

        public static string GetEventName(int eventId)
        {
            var currentEvent = (Event) GetCachedItem("Event");
            if (currentEvent == null)
            {
                SetGlobalCache();
                currentEvent = (Event) GetCachedItem("Event");
            }
            return currentEvent.EventName;
        }

        public static bool HasEventStarted(short eventId)
        {
            return false;
        }

        public static DateTime GetFirstFixtureDate(short eventId)
        {
            return DateTime.Now;
        }


        public static bool HasLastFixturePassed(short eventId)
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

        
        //public static short GetEventId()
        //{
        //    var currentEvent = (Event)GetCachedItem("Event");
        //    if (currentEvent == null)
        //    {
        //        SetGlobalCache();
        //        currentEvent = (Event)GetCachedItem("Event");
        //    }

        //    return currentEvent.Id;
        //}
        //public static int GetGlobalPoolId()
        //{
        //    var globalPoolId = GetCachedItem("GlobalPoolId");
        //    if (globalPoolId == null)
        //    {
        //        SetGlobalCache();
        //        globalPoolId = GetCachedItem("GlobalPoolId");
        //    }

        //    globalPoolId = (globalPoolId ?? 1); // ?? use p, but if null use 1
        //    return (int)globalPoolId;
        //}

        private static void SetGlobalCache()
        {
            var context = new ApplicationDbContext();
            var eventId = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["EventId"]);
            var globalPoolId = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["GlobalPoolId"]);
            var predictionsLockDown = context.Fixtures.Where(e => e.EventId == eventId).Min(f => f.FixtureDateTime);
            var lastFixture = predictionsLockDown;
            var hasKo = context.KoFixtures.Count(e => e.EventId == eventId);
            if (hasKo>0)
            {
                var lastKoFixture = context.KoFixtures.Where((e => e.EventId == eventId)).Max(f => f.FixtureDateTime);
                if (lastKoFixture > lastFixture)
                {
                    lastFixture = lastKoFixture;
                }
            }


            Helper.Cache.SetCachedItem("Event", context.Events.Single(e=> e.Id == eventId));
            Helper.Cache.SetCachedItem("GlobalPoolId", globalPoolId);
            Helper.Cache.SetCachedItem("PredictionsLockDownDateTime", predictionsLockDown);
            Helper.Cache.SetCachedItem("LastFixture", lastFixture);

        }
    }

}