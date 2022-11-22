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
    public class EventPoolPlayersController : Controller
    {

        private readonly ApplicationDbContext _context;

        public EventPoolPlayersController()
        {
            _context = new ApplicationDbContext();
        }

        // GET: EventPoolPlayers
        public ActionResult AddRemovePlayers(short eventId, int poolId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            var userId = User.Identity.GetUserId();
            var pool = _context.Pools.FirstOrDefault(a => a.Id == poolId & (a.AdminPlayerId == userId | userId == "e51699d7-7cf2-4565-905c-4a89c4f80063"));
            if (pool==null)
            {
                return RedirectToAction("Index", "Home");
            }

            var myEvent = Helper.Cache.GetCachedEvent(eventId);
            var eventPoolPlayers = _context.EventPoolPlayers.Where(a => a.Enabled == true & a.PoolId == poolId & a.EventId == eventId).ToList();
            var poolPlayers = _context.PoolPlayers
                .Include(p => p.Player)
                .Where(a => a.Enabled == true & a.PoolId == poolId)
                .OrderBy(a => a.Player.DisplayName)
                .ToList();

            var eventPoolPlayersViewModel = new EventPoolPlayersViewModel
            {
                EventPoolPlayers = eventPoolPlayers,
                PoolPlayers = poolPlayers,
                Pool = pool,
                Event = myEvent
            };

            return View(eventPoolPlayersViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveAddRemovePlayers(EventPoolPlayersViewModel eventPoolPlayersViewModel)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            var poolId = eventPoolPlayersViewModel.Pool.Id;
            var eventId = eventPoolPlayersViewModel.Event.Id;
            var changesMade = false;

            var eventPoolPlayers = _context.EventPoolPlayers.Where(a => a.PoolId == poolId & a.EventId == eventId).ToList();
            var poolPlayers = _context.PoolPlayers
                .Include(p => p.Player)
                .Where(a => a.Enabled == true & a.PoolId == poolId)
                .OrderBy(a => a.Player.DisplayName)
                .ToList();

            foreach(var poolPlayer in poolPlayers)
            {
                var playingIn = Request["player_" + poolPlayer.PlayerId];
                var eventPoolPlayer = eventPoolPlayers.FirstOrDefault(a => a.PlayerId == poolPlayer.PlayerId);

                if (playingIn.IsNullOrWhiteSpace())
                {

                    // Player is not in this
                    if (eventPoolPlayer != null && eventPoolPlayer.Enabled == true)
                    {
                        eventPoolPlayer.Enabled = false;
                        eventPoolPlayer.ModifiedDateTime = DateTime.UtcNow;
                        changesMade = true;
                        _context.EventPoolPlayers.AddOrUpdate(eventPoolPlayer);
                    }
 
                }
                else
                {
                    // this player should be in this
                    if (eventPoolPlayer == null)
                    {
                        eventPoolPlayer = new EventPoolPlayer
                        {
                            Enabled = true,
                            PlayerId = poolPlayer.PlayerId,
                            PoolId = poolId,
                            EventId = eventId,
                            AdminApprovedDateTime = DateTime.UtcNow,
                            CreatedDateTime = DateTime.UtcNow,
                            ModifiedDateTime = DateTime.UtcNow
                        };
                        changesMade = true;
                        _context.EventPoolPlayers.Add(eventPoolPlayer);
                    }
                    else
                    {
                        if (eventPoolPlayer.Enabled == false)
                        {
                            eventPoolPlayer.Enabled = true;
                            eventPoolPlayer.ModifiedDateTime = DateTime.UtcNow;
                            changesMade = true;
                            _context.EventPoolPlayers.AddOrUpdate(eventPoolPlayer);
                        }

                    }

                }
            }

            if(changesMade)
                _context.SaveChanges();

            return RedirectToAction("ShowTable", "Table", new { eventId = eventId , poolId = poolId });

        }
    }
}