using System;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
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

        public static bool HasEventStarted(short eventId)
        {
            var myEvent = (Event) GetCachedItem("Event*" + eventId);
            if (myEvent.EventStartDateTime < DateTime.Now)
            {
                return true;
            }
            return false;
        }
        
        public static void SetEventCache()
        {
            var context = new ApplicationDbContext();

            var events = context.Events.ToList();
            foreach(var myEvent in events)
            {
                SetCachedItem("Event*"+myEvent.Id,myEvent);
            }
        }
        public static void SetEventCache(short eventId)
        {
            var context = new ApplicationDbContext();

            var myEvent = context.Events.SingleOrDefault(a => a.Id==eventId);
            if(myEvent!=null)
            { 
                SetCachedItem("Event*" + myEvent.Id, myEvent);
            }
        }
    }
}