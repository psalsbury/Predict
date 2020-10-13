using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Predict.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web.Mvc;

namespace Predict.Controllers
{
    public class EventPlayersController : Controller
    {

        private readonly ApplicationDbContext _context;

        public EventPlayersController()
        {
            _context = new ApplicationDbContext();
        }

        // GET: EventPlayers
        public ActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            var myEvents = (List<Event>)Predict.Helper.Cache.GetCachedItem("Events");

            return View(myEvents);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save()
        {
            var playerId = User.Identity.GetUserId();
            var myEventPlayers = _context.EventPlayers.Where(a => a.PlayerId == playerId).ToList();
            var myEvents = (List<Event>)Predict.Helper.Cache.GetCachedItem("Events");

            foreach (Event myEvent in myEvents)
            {
                var myEventPlayer = myEventPlayers.FirstOrDefault(m => m.EventId == myEvent.Id);
                var changeMade = false;
                var playingIn = Request["event_" + myEvent.Id];
                if (!myEvent.EventStarted)
                {

                    if (playingIn.IsNullOrWhiteSpace() && myEventPlayer != null && myEventPlayer.Enabled == true)
                    {
                        // User has selected NOT to be in this event and the record exists
                        myEventPlayer.Enabled = false;
                        myEventPlayer.ModifiedDateTime = DateTime.UtcNow;

                        // disable all entries to pools for this event
                        var poolPlayers = _context.EventPoolPlayers
                            .Where(f => f.PlayerId == playerId && f.EventId == myEvent.Id).ToList();
                        poolPlayers.ForEach(a => a.Enabled = false);
                        poolPlayers.ForEach(a => a.ModifiedDateTime = DateTime.UtcNow);
                        _context.EventPlayers.AddOrUpdate(myEventPlayer);
                        changeMade = true;

                    }
                    else if (!playingIn.IsNullOrWhiteSpace() && myEventPlayer != null && myEventPlayer.Enabled == false)
                    {
                        // User has selected to be in this event and the record exists
                        myEventPlayer.Enabled = true;
                        myEventPlayer.ModifiedDateTime = DateTime.UtcNow;
                        _context.EventPlayers.AddOrUpdate(myEventPlayer);
                        changeMade = true;
                    }
                    else if (!playingIn.IsNullOrWhiteSpace() && myEventPlayer == null)
                    {
                        // User has selected to be in this event and the record does not exist
                        myEventPlayer = new EventPlayer
                        {
                            EventId = myEvent.Id,
                            PlayerId = playerId,
                            CreatedDateTime = DateTime.UtcNow,
                            ModifiedDateTime = DateTime.UtcNow,
                            Enabled = true

                        };
                        _context.EventPlayers.Add(myEventPlayer);
                        changeMade = true;
                    }

                    if (!playingIn.IsNullOrWhiteSpace())
                    {
                        // Make sure the player is associated to the global pool.
                        var globalPoolId = (System.Convert.ToInt32(ConfigurationManager.AppSettings["GlobalPoolId"]));

                        var myPoolPlayer = _context.PoolPlayers.FirstOrDefault();
                        if (myPoolPlayer == null)
                        {
                            myPoolPlayer = new PoolPlayer();
                            myPoolPlayer.CreatedDateTime = DateTime.UtcNow;
                            myPoolPlayer.ModifiedDateTime = DateTime.UtcNow;
                            myPoolPlayer.PlayerId = playerId;
                            myPoolPlayer.PoolId = globalPoolId;
                            myPoolPlayer.Enabled = true;
                        }
                        else if (myPoolPlayer.Enabled == false)
                        {
                            myPoolPlayer.ModifiedDateTime = DateTime.UtcNow;
                            myPoolPlayer.Enabled = true;
                        }

                        _context.PoolPlayers.AddOrUpdate(myPoolPlayer);

                        // Associate the player/pool to the event
                        var myEventPoolPlayer = _context.EventPoolPlayers.FirstOrDefault(f =>
                            f.EventId == myEvent.Id && f.PlayerId == playerId && f.PoolId == globalPoolId);
                        if (myEventPoolPlayer == null)
                        {
                            myEventPoolPlayer = new EventPoolPlayer()
                            {
                                CreatedDateTime = DateTime.UtcNow,
                                ModifiedDateTime = DateTime.UtcNow,
                                EventId = myEvent.Id,
                                PlayerId = playerId,
                                PoolId = globalPoolId,
                                AdminApprovedDateTime = DateTime.UtcNow,
                                PoolPosition = 0,
                                Enabled = true
                            };
                        }
                        else
                        {
                            myEventPoolPlayer.Enabled = true;
                            myEventPoolPlayer.ModifiedDateTime = DateTime.UtcNow;
                        }

                        _context.EventPoolPlayers.AddOrUpdate(myEventPoolPlayer);
                        changeMade = true;
                    }

                    if (changeMade == true)
                        _context.SaveChanges();
                }
            }

            Helper.SessionHelper.SetUserSessionVariables(Session, playerId, true);
            return RedirectToAction("Index", "Home");
        }
    }

}
