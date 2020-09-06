using System;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Runtime.Caching;
using Microsoft.AspNet.Identity;
using Predict.Models;
using System.Collections.Generic;

namespace Predict.Helper
{
    // this class is for perm data. Not user specific.
    public static class Cache
    {
        public static void SendEmail(IdentityMessage message)
        {
            var smtpMessage = new MailMessage
            {
                From = new MailAddress(ConfigurationManager.AppSettings["SupportEmailAddr"])
            };
            smtpMessage.To.Add(new MailAddress(message.Destination));
            smtpMessage.Bcc.Add(new MailAddress("peter@salsbury.co.uk"));
            smtpMessage.Subject = message.Subject;
            smtpMessage.Body = message.Body;
            smtpMessage.IsBodyHtml = true;

            var client = new SmtpClient();
            if (Environment.MachineName == "THINKPAD")
            {
                client.Host = "ignored";
                client.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                client.PickupDirectoryLocation = @"c:\predictemails";
            }

            client.Send(smtpMessage);
        }

        public static object GetCachedItem(string cacheId)
        {
            if (!MemoryCache.Default.Contains(cacheId))
            {
                return null;
            }
            return MemoryCache.Default.Get(cacheId);
        }

        public static void SetCachedItem(string cacheId, object cachedItem)
        {
            MemoryCache.Default.Set(cacheId, cachedItem, DateTime.Now.AddDays(30));
        }

        public static Event GetCachedEvent(int eventId)
        {
            var myEvents = (List<Event>)GetCachedItem("Events");
            var myEvent = myEvents.FirstOrDefault(e => e.Id == eventId);
            return myEvent;
        }

        public static bool HasEventStarted(short eventId)
        {
            var myEvents = (List<Event>)GetCachedItem("Events");
            var myEvent = myEvents.FirstOrDefault(e => e.Id == eventId);
            if (myEvent != null)
            {
                if (myEvent.StartDateTime < DateTime.Now)
                {
                    return true;
                }
            }

            return false;
        }
        
        public static void SetEventCache()
        {
            var context = new ApplicationDbContext();

            var events = context.Events.ToList();
            SetCachedItem("Events",events);
        }
        public static void SetEventCache(short eventId)
        {
            var myEvents = (List<Event>)GetCachedItem("Events");
            var context = new ApplicationDbContext();
            var myNewEvent = context.Events.SingleOrDefault(a => a.Id == eventId);
            if (myNewEvent != null)
            {
                var myOriginalEventIndex = myEvents.FindIndex(e => e.Id == eventId);
                myEvents[myOriginalEventIndex] = myNewEvent;
            }
            SetCachedItem("Events", myEvents);
        }
    }
}