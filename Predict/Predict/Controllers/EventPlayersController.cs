using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Predict.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Data.Entity;
using System.Web.Mvc;
using Predict.Helper;

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
            // Session["EventPlayers"] is required for the view. if its null, go back to the home page
            if (!User.Identity.IsAuthenticated | Session["EventPlayers"] == null)
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

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            var playerId = User.Identity.GetUserId();
            var myEventPlayers = _context.EventPlayers
                .Include(a => a.Event)
                .Where(a => a.PlayerId == playerId).ToList();
            var myEvents = (List<Event>)Predict.Helper.Cache.GetCachedItem("Events");
            var poolPlayers = _context.PoolPlayers.Where(a => a.PlayerId == playerId)
                .Where(a => a.Enabled == true).ToList();

            foreach (Event myEvent in myEvents)
            {
                if (!myEvent.EventStarted)
                {
                    var myEventPlayer = myEventPlayers.FirstOrDefault(m => m.EventId == myEvent.Id);
                    var changeMade = false;
                    var playingIn = Request["event_" + myEvent.Id];

                    if (playingIn.IsNullOrWhiteSpace() && myEventPlayer != null && myEventPlayer.Enabled == true)
                    {
                        // User has selected NOT to be in this event and the record exists
                        myEventPlayer.Enabled = false;
                        myEventPlayer.ModifiedDateTime = DateTime.UtcNow;

                        // disable all EventPoolPlayers entries to pools for this event
                        var eventPoolPlayers = _context.EventPoolPlayers
                            .Where(f => f.PlayerId == playerId && f.EventId == myEvent.Id).ToList();

                        eventPoolPlayers.ForEach(a => a.Enabled = false);
                        eventPoolPlayers.ForEach(a => a.ModifiedDateTime = DateTime.UtcNow);

                        // remove any existing fixture predictions as player is no longer playing this event
                        var fixturePredictions = _context.FixturePredictions
                            .Where(a => a.EventId == myEvent.Id)
                            .Where(a => a.PlayerId == playerId).ToList();
                        _context.FixturePredictions.RemoveRange(fixturePredictions);

                        // remove any ko fixture predictions as player is no longer playing this event
                        var koFixturePredictions = _context.KoFixturePredictions
                            .Include(a => a.KoFixture)
                            .Where(a => a.KoFixture.EventId == myEvent.Id)
                            .Where(a => a.PlayerId == playerId).ToList();
                        _context.KoFixturePredictions.RemoveRange(koFixturePredictions);

                        // remove any bonus question predictions as player is no longer playing this event
                        var bonusPredictions = _context.BonusQuestionPredictions
                            .Include(a => a.BonusQuestion)
                            .Where(a => a.BonusQuestion.EventId == myEvent.Id)
                            .Where(a => a.PlayerId == playerId).ToList();
                        _context.BonusQuestionPredictions.RemoveRange(bonusPredictions);

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

                    if (changeMade == true)
                        _context.SaveChanges();

                    if (!playingIn.IsNullOrWhiteSpace() && changeMade)
                    {
                        // Make sure the player is associated to all pools that are associated to the player that are associated to this event
                        var eventPools = _context.EventPools.Where(a => a.EventId == myEvent.Id)
                            .Where(a => a.Enabled==true).ToList();

                        foreach (var eventPool in eventPools)
                        {

                            var exists = poolPlayers.Exists(a => a.PoolId == eventPool.PoolId && a.Enabled==true);
                            if (exists)
                            {
                                // If the player is also associated the the pool then associate the player/pool to the event
                                var myEventPoolPlayer = _context.EventPoolPlayers.FirstOrDefault(f =>
                                    f.EventId == myEvent.Id && f.PlayerId == playerId && f.PoolId == eventPool.PoolId);
                                if (myEventPoolPlayer == null)
                                {
                                    myEventPoolPlayer = new EventPoolPlayer()
                                    {
                                        CreatedDateTime = DateTime.UtcNow,
                                        ModifiedDateTime = DateTime.UtcNow,
                                        EventId = myEvent.Id,
                                        PlayerId = playerId,
                                        PoolId = eventPool.PoolId,
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
                                _context.SaveChanges();
                                SessionHelper.UpdateSessionForHomePage(_context, Session, myEventPlayer, true,true);

                            }
                        }
                    }
                }
            }

            Helper.SessionHelper.SetUserSessionVariables(Session, playerId, true);
            return RedirectToAction("Index", "Home");
        }
    }

}
