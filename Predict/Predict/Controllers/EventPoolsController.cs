using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Predict.Models;
using Predict.ViewModels;

namespace Predict.Controllers
{
    public class EventPoolsController : Controller
    {

        private readonly ApplicationDbContext _context;

        public EventPoolsController()
        {
            _context = new ApplicationDbContext();
        }

        // GET: EventPools
        public ActionResult EventsLinkedToPool(int id)
        {

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            var playerId = User.Identity.GetUserId();
            var eventPoolsViewModel = new EventPoolsViewModel();

            var eventPools = _context.EventPools
                .Include(a => a.Event)
                .Include(a => a.Pool)
                .Where(a => a.PoolId == id)
                .Where(a => a.Enabled==true)
                .Where(a => a.Pool.AdminPlayerId==playerId)
                .ToList();

            var poolName = _context.Pools.First(a => a.Id == id).PoolName;

            var myEvents = (List<Event>)Predict.Helper.Cache.GetCachedItem("Events");

            eventPoolsViewModel.EventPools = eventPools;
            eventPoolsViewModel.Events = myEvents;
            eventPoolsViewModel.PoolId = id;

            ViewBag.PoolName = poolName;

            return View(eventPoolsViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(EventPoolsViewModel eventPoolsViewModel)
        {

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            var myEventPools = _context.EventPools.Where(a => a.PoolId == eventPoolsViewModel.PoolId);
            var myEvents = (List<Event>)Predict.Helper.Cache.GetCachedItem("Events");
            var poolPlayers = _context.PoolPlayers
                .Where(a => a.Enabled == true)
                .Where(a => a.PoolId == eventPoolsViewModel.PoolId).ToList();

            foreach (Event myEvent in myEvents)
            {
                var changeMade = false;
                if (!myEvent.EventStarted)
                {
                    var myEventPool = myEventPools.FirstOrDefault(m => m.EventId == myEvent.Id);
                    var playingIn = Request["event_" + myEvent.Id];

                    if (playingIn.IsNullOrWhiteSpace() && myEventPool != null && myEventPool.Enabled == true)
                    {
                        // User has selected for this pool NOT to be in this event and the record is enabled and exists
                        myEventPool.Enabled = false;
                        myEventPool.ModifiedDateTime = DateTime.UtcNow;
                        _context.EventPools.AddOrUpdate(myEventPool);

                        // Remove all EventPoolPlayer records linked to this event/pool
                        var eventPoolPlayers = _context.EventPoolPlayers
                            .Where(f => f.PoolId == eventPoolsViewModel.PoolId && f.EventId == myEvent.Id).ToList();
                        eventPoolPlayers.ForEach(a => a.Enabled = false);
                        eventPoolPlayers.ForEach(a => a.ModifiedDateTime = DateTime.UtcNow);

                        changeMade = true;

                    }
                    else if (!playingIn.IsNullOrWhiteSpace() && myEventPool != null && myEventPool.Enabled == false)
                    {
                        // User has selected for the pool to be in this event and the record exists and disabled
                        myEventPool.Enabled = true;
                        myEventPool.ModifiedDateTime = DateTime.UtcNow;
                        _context.EventPools.AddOrUpdate(myEventPool);

                        changeMade = true;
                    }
                    else if (!playingIn.IsNullOrWhiteSpace() && myEventPool == null)
                    {
                        // User has selected for this pool to be in this event and the record does not
                        myEventPool = new EventPool
                        {
                            EventId = myEvent.Id,
                            PoolId = eventPoolsViewModel.PoolId,
                            CreatedDateTime = DateTime.UtcNow,
                            ModifiedDateTime = DateTime.UtcNow,
                            Enabled = true
                        };
                        _context.EventPools.Add(myEventPool);
                        changeMade = true;
                    }

                    if (!playingIn.IsNullOrWhiteSpace())
                    {
                        // Event 
                        var eventPoolPlayers = _context.EventPoolPlayers
                            .Where(a => a.PoolId == eventPoolsViewModel.PoolId)
                            .Where(a => a.EventId == myEvent.Id).ToList();

                        //
                        var eventPlayers = _context.EventPlayers.Where(a => a.EventId == myEvent.Id)
                            .Where(a => a.Enabled == true).ToList();

                        foreach (var poolPlayer in poolPlayers)
                        {

                            // Check if the player is playing this event
                            var playingEvent = eventPlayers.Exists(a => a.PlayerId == poolPlayer.PlayerId);
                            if (playingEvent)
                            {
                                var eventPoolPlayer =
                                eventPoolPlayers.FirstOrDefault(a =>
                                    a.PlayerId == poolPlayer.PlayerId && a.Enabled == false);

                                if (eventPoolPlayer == null)
                                {
                                    eventPoolPlayer = new EventPoolPlayer
                                    {
                                        Enabled = true,
                                        PlayerId = poolPlayer.PlayerId,
                                        PoolId = eventPoolsViewModel.PoolId,
                                        EventId = myEvent.Id,
                                        AdminApprovedDateTime = DateTime.UtcNow,
                                        CreatedDateTime = DateTime.UtcNow,
                                        ModifiedDateTime = DateTime.UtcNow
                                    };
                                }
                                else
                                {
                                    eventPoolPlayer.Enabled = true;
                                    eventPoolPlayer.ModifiedDateTime = DateTime.UtcNow;
                                }

                                _context.EventPoolPlayers.AddOrUpdate(eventPoolPlayer);
                            }
                        }   
                    }
                }

                if (changeMade == true)
                {
                    Helper.SessionHelper.RefreshPlayerPoolInfo(Session, User.Identity.GetUserId(), myEvent.Id);
                    _context.SaveChanges();
                }
            }

            return RedirectToAction("Index", "PoolDashboard");
        }
    }
}