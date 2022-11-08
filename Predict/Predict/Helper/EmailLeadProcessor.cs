using Predict.Models;
using System;
using System.Data.Entity;
using System.Linq;
using Microsoft.AspNet.Identity;
using System.Configuration;
using System.Data.Entity.Migrations;

namespace Predict.Helper
{
    public static class EmailLeadProcessor
    {

        public static void SendLeadEmail()
        {

            var confirmEmailAddress = System.Convert.ToBoolean(ConfigurationManager.AppSettings["SendEmailLeads"]);
            if (!confirmEmailAddress)
                return;

            var _context = new ApplicationDbContext();
            var emailLeads = _context.EmailLead.Where(a => a.EmailDate == null).OrderByDescending(a => a.WasAdmin).Take(1);
            var emailLead = emailLeads.FirstOrDefault();

            if(emailLead != null)
            {
                var email = new IdentityMessage
                {

                    Body = "Dear " + emailLead.PlayerName + "," + "<br>" + "<br>" +
                    "It may seem like a long time ago now, but as you entered our prediction comp for the World Cup previously " +
                    "I thought you may be interested to know that I have launched another competition for the upcoming World Cup in Qatar." +
                    "The site can be found here <a href='http://www.predictioncomp.com'>http://www.predictioncomp.com</a>" + "<br>" + "<br>" +
                    "The comp is the same format as previous ones, and is now mobile friendly." + "<br>" + "<br>" +
                    "Please feel free to enter..." + "<br>" + "<br>" +
                    "Kind regards" + "<br>" +
                    "The Prediction Comp Team"

                    ,Subject = "World Cup Prediction Comp"
                    ,Destination = emailLead.EmailAddress
                    ,
                };
                Helper.Cache.SendEmail(email);

                emailLead.EmailDate = DateTime.UtcNow;
                _context.EmailLead.AddOrUpdate(emailLead);
                _context.SaveChanges();

            }
            _context.Dispose();

        }
    }
}