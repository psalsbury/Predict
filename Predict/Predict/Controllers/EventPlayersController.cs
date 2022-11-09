using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Predict.Models;
using Predict.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Data.Entity;
using System.Web.Mvc;
using Predict.Helper;
using System.Data.SqlClient;

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
        public ActionResult JoinAComp()
        {
            // Session["EventPlayers"] is required for the view. if its null, go back to the home page
            if (!User.Identity.IsAuthenticated | Session["EventPlayers"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var eventPlayersViewModel = _context.Database.SqlQuery<EventPlayersViewModel>(
                "spGetEventListForDisplay @OnlyShowActive"
                , new SqlParameter("@OnlyShowActive", true)).OrderBy(a => a.EventName).ToList();

            var eventPlayers = (List<EventPlayer>)Session["EventPlayers"];
            var eventIdsPlaying = eventPlayers.Where(a => a.Event.EventFinished == false).Select(a => a.EventId).ToList();

            // Only show the active comps that this player is not already playing
            var filteredList = eventPlayersViewModel.Where(a => !eventIdsPlaying.Contains(a.EventId)).ToList();
            return View(filteredList);
        }

        // GET: EventPlayers
        public ActionResult ActiveComps()
        {
            // Session["EventPlayers"] is required for the view. if its null, go back to the home page
            if (!User.Identity.IsAuthenticated | Session["EventPlayers"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var userId = User.Identity.GetUserId();
            var eventPlayersViewModel = _context.Database.SqlQuery<EventPlayersViewModel>(
                "spGetEventListForDisplay @OnlyShowActive, @CreatedByUserId, @ParticipatingInUserId"
                , new SqlParameter("@OnlyShowActive", true)
                , new SqlParameter("@CreatedByUserId", DBNull.Value)
                , new SqlParameter("@ParticipatingInUserId", userId))
                .OrderBy(a => a.EventName)
                .ToList();

            return View(eventPlayersViewModel);
        }

        public ActionResult FinishedComps()
        {
            // Session["EventPlayers"] is required for the view. if its null, go back to the home page
            if (!User.Identity.IsAuthenticated | Session["EventPlayers"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var userId = User.Identity.GetUserId();
            var eventPlayersViewModel = _context.Database.SqlQuery<EventPlayersViewModel>(
                "spGetEventListForDisplay @OnlyShowActive, @CreatedByUserId, @ParticipatingInUserId"
                , new SqlParameter("@OnlyShowActive", false)
                , new SqlParameter("@CreatedByUserId", DBNull.Value)
                , new SqlParameter("@ParticipatingInUserId", userId))
                .OrderByDescending(a => a.EndDateTime)
                .ToList();

            return View(eventPlayersViewModel);
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

            return View(myEvents.OrderBy(a => a.EventDescription));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save()
        {

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            var globalPoolId = Convert.ToInt32(ConfigurationManager.AppSettings["GlobalPoolId"]);
            var playerId = User.Identity.GetUserId();
            var myEventPlayers = _context.EventPlayers
                .Include(a => a.Event)
                .Where(a => a.PlayerId == playerId).ToList();

            var myEvents = (List<Event>)Predict.Helper.Cache.GetCachedItem("Events");

            var poolPlayers = _context.PoolPlayers.Where(a => a.PlayerId == playerId)
                .Where(a => a.Enabled == true).ToList();
            var compNewlyEntered = false;

            foreach (Event myEvent in myEvents)
            {
                if (!myEvent.EventFinished)
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
                        compNewlyEntered = true;
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
                        compNewlyEntered = true;
                    }

                    if (compNewlyEntered)
                    {
                        // If this member is the owner of a pool, then join the event to the pool
                        var pools = _context.Pools.Where(a => a.AdminPlayerId == playerId && a.Id != globalPoolId).ToList();
                        foreach (var pool in pools)
                        {
                            var eventPool = _context.EventPools.Where(a => a.PoolId == pool.Id && a.EventId == myEvent.Id).FirstOrDefault();
                            if (eventPool != null && eventPool.Enabled == false)
                            {
                                eventPool.Enabled = true;
                                eventPool.ModifiedDateTime = DateTime.UtcNow;
                            }
                            else if (eventPool == null)
                            {
                                eventPool = new EventPool
                                {
                                    EventId = myEvent.Id,
                                    PoolId = pool.Id,
                                    Enabled = true,
                                    ModifiedDateTime = DateTime.UtcNow,
                                    CreatedDateTime = DateTime.UtcNow
                                };
                            }
                            _context.EventPools.AddOrUpdate(eventPool);

                            var thisPoolPoolPlayers = _context.PoolPlayers.Where(a => a.PoolId == pool.Id);
                            foreach (var thisPoolPlayer in thisPoolPoolPlayers)
                            {
                                // If this player is participating in this comp then ensure that the player/pool/event is linked

                                var playerPlayingThisEvent = _context.EventPlayers.Where(a => a.EventId == myEvent.Id && a.PlayerId == thisPoolPlayer.PlayerId).Any();

                                if (!playerPlayingThisEvent)
                                    continue;

                                var eventPoolPlayer = _context.EventPoolPlayers.Where(a => a.PoolId == pool.Id && a.EventId == myEvent.Id && a.PlayerId == thisPoolPlayer.PlayerId).FirstOrDefault();
                                if (eventPoolPlayer != null && eventPoolPlayer.Enabled == false)
                                {
                                    eventPoolPlayer.Enabled = true;
                                    eventPoolPlayer.ModifiedDateTime = DateTime.UtcNow;
                                }
                                else if (eventPoolPlayer == null)
                                {
                                    // Add the player to the event/pool
                                    eventPoolPlayer = new EventPoolPlayer
                                    {
                                        PlayerId = thisPoolPlayer.PlayerId,
                                        EventId = myEvent.Id,
                                        PoolId = pool.Id,
                                        Enabled = true,
                                        CreatedDateTime = DateTime.UtcNow,
                                        ModifiedDateTime = DateTime.UtcNow,
                                        AdminApprovedDateTime = DateTime.UtcNow,
                                        PoolPosition = 0,
                                        CorrectScore = 0,
                                        CorrectResult = 0,
                                        WinMargin = 0,
                                        KoScore = 0,
                                        BonusScore = 0,
                                        TotalScore = 0
                                    };
                                    _context.EventPoolPlayers.Add(eventPoolPlayer);
                                }
                            }
                        }
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
