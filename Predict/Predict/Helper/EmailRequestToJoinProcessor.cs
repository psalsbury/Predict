using Predict.Models;
using System;
using System.Data.Entity;
using System.Linq;
using Microsoft.AspNet.Identity;
using System.Configuration;
using System.Data.Entity.Migrations;

namespace Predict.Helper
{
    public static class EmailRequestToJoinProcessor
    {

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static void SendRequestToJoinEmail()
        {

            var _context = new ApplicationDbContext();
            var emailRequestToJoinList = _context.EmailRequestToJoin.Where(a => a.StatusId == 1).Take(1);
            var emailRequestToJoin = emailRequestToJoinList.FirstOrDefault();

            if (emailRequestToJoin != null)
            {
                Logger.Info("SendRequestToJoinEmail  --> {0}, {1}, {2} ", emailRequestToJoin.EventId, emailRequestToJoin.PoolId, emailRequestToJoin.PlayerId);
                var exists = _context.EventPoolPlayers.Any(a => a.PlayerId == emailRequestToJoin.PlayerId
                                & a.PoolId == emailRequestToJoin.PoolId
                                & a.EventId == emailRequestToJoin.EventId
                                & a.Enabled==true);

                if(exists)
                {
                    emailRequestToJoin.ModifiedDateTime = DateTime.UtcNow;
                    emailRequestToJoin.StatusId = 4; // not sent as already joined
                }
                else
                {
                    var poolPlayer = _context.PoolPlayers
                        .Include(p => p.Player)
                        .Include(u => u.Player.AspNetUser)
                        .Include(l => l.Pool)
                        .Where(a => a.PoolId == emailRequestToJoin.PoolId 
                                & a.Enabled ==true    
                                & a.PlayerId == emailRequestToJoin.PlayerId).FirstOrDefault();

                    var myEvent = Helper.Cache.GetCachedEvent(emailRequestToJoin.EventId);

                    if (poolPlayer == null)
                    {
                        emailRequestToJoin.ModifiedDateTime = DateTime.UtcNow;
                        emailRequestToJoin.StatusId = 3; // 3 = not sent as no longer in pool
                    }
                    else
                    { 
                        var adminPlayer = _context.Players.FirstOrDefault(a => a.Id == poolPlayer.Pool.AdminPlayerId);
                        var email = new IdentityMessage
                        {

                                Body = "Dear " + poolPlayer.Player.PlayerName + "," + "<br>" + "<br>" +
                                adminPlayer.DisplayName + " is running a prediction comp and has invited you to join. " +
                                "<br><br>The comp is called " + myEvent.EventName +
                                "and starts on " + myEvent.StartDateTime.ToString("dd-MMM-yyyy HH:mm:ss") +
                                "<br><br>To join, log into <a href='www.predictioncomp.com'>www.predictioncomp.com</a> and select CREATE/JOIN/MANAGE then select 'A Competition'. From there you can Join the comp by selecting 'Join A Comp'" +
                                "<br><br>" +
                                "Good Luck!" + "<br>" +
                                "The Prediction Comp Team"
                                 ,Subject = "Prediction Comp - Request to join comp"
                                 ,Destination = poolPlayer.Player.AspNetUser.Email
                        };
                        Helper.Cache.SendEmail(email);
                        emailRequestToJoin.ModifiedDateTime = DateTime.UtcNow;
                        emailRequestToJoin.SentDateTime = DateTime.UtcNow;
                        emailRequestToJoin.StatusId = 2; // Sent
                    }
                }
                _context.EmailRequestToJoin.AddOrUpdate(emailRequestToJoin);
                _context.SaveChanges();

            }
            _context.Dispose();

        }
    }
}