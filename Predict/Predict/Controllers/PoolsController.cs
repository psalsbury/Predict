using AutoMapper;
using Microsoft.AspNet.Identity;
using Predict.Models;
using Predict.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace Predict.Controllers
{
    public class PoolsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public PoolsController()
        {
            _context = new ApplicationDbContext();
        }

        // GET: Pools
        public ActionResult Index()
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            // Normal user can see only their pools
            var userid = User.Identity.GetUserId();

            var pools = _context.Pools.Include(b => b.AdminPlayer)
                .Where(p => p.AdminPlayerId == userid).ToList();

            ViewBag.GlobalPoolId = Convert.ToInt32(ConfigurationManager.AppSettings["GlobalPoolId"]);
            return View(pools);
        }


        // GET: Pools
        public ActionResult JoinPool()
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var playerId = User.Identity.GetUserId();

            var availablePools = (from pool in _context.Pools
                where !_context.PoolPlayers.Any(f => f.PlayerId == playerId && f.PoolId==pool.Id && f.Enabled==true)
                select pool)
                .OrderBy(a => a.PoolName)
                .ToList();

            return View("JoinPool", availablePools);
        }

        // GET: Pools
        public ActionResult PoolsMemberOf()
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var playerId = User.Identity.GetUserId();

            var globalPoolId =
                Convert.ToInt32(ConfigurationManager.AppSettings["GlobalPoolId"]);

            // Admin of the site can see all pools
            var joinedPools = _context.PoolPlayers
                .Include(a => a.Pool)
                .Where(a => a.PlayerId == playerId && a.Enabled == true);

            return View("PoolsMemberOf", joinedPools);
        }

        public ActionResult New()
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var poolModel = new Pool
            {
                AdminPlayerId = User.Identity.GetUserId()
                , CorrectScorePoints = 5
                , CorrectResultPoints = 2
                , WinMarginPoints = 1
                , KoLast16Points = 1
                , KoLast8Points = 2
                , KoLast4Points = 4
                , KoLast2Points = 6
                , KoLast1Points = 10
                , FreezePredictions = false
            };

            return View("EditPool", poolModel);
        }


        public ActionResult Edit(int id)
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var loggedInUserId = User.Identity.GetUserId();
            var isPoolAdmin = _context.Pools.Any(o => o.Id == id && o.AdminPlayerId == loggedInUserId);
            var poolModel = _context.Pools.SingleOrDefault(p => p.Id == id);
            var isInLockDown = false;
            if (!isPoolAdmin) throw new Exception("Only pool admin is allowed to edit the pool");

            ViewBag.isInLockDown = isInLockDown;
            return View("EditPool", poolModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(Pool poolModel)
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                return View("EditPool", poolModel);
            }

            var newPool = false;
            var poolFromDb = new Pool();
            if (poolModel.Id != 0) poolFromDb = _context.Pools.Single(m => m.Id == poolModel.Id);

            // Create or update pool
            Mapper.Map(poolModel, poolFromDb);
            poolFromDb.ModifiedDateTime = DateTime.UtcNow;

            if (poolModel.Id == 0)
            {
                newPool = true;
                poolFromDb.CreatedDateTime = DateTime.UtcNow;
                _context.Pools.Add(poolFromDb);
            }


            _context.SaveChanges();

            if (newPool)
            {

                var poolPlayer = new PoolPlayer
                {
                    PoolId = poolFromDb.Id,
                    PlayerId = poolFromDb.AdminPlayerId,
                    Enabled = true,
                    CreatedDateTime = DateTime.UtcNow,
                    ModifiedDateTime = DateTime.UtcNow,
                    EmailSentToAdminDateTime = DateTime.UtcNow
                };
                _context.PoolPlayers.Add(poolPlayer);

                // Add this pool to all unfinished events entered by the person who has created the pool
                var eventPlayersItemList = _context.EventPlayers
                                        .Include(e => e.Event)
                                        .Where(a => a.Event.StartDateTime >= DateTime.UtcNow && a.PlayerId == poolFromDb.AdminPlayerId)
                                        .Select(x => new { x.PlayerId, x.EventId }).Distinct().ToList();

                foreach (var item in eventPlayersItemList)
                {
                    // Add the new pool to the event
                    var eventPool = new EventPool
                    {
                        CreatedDateTime = DateTime.UtcNow,
                        ModifiedDateTime = DateTime.UtcNow,
                        EventId = item.EventId,
                        Enabled = true,
                        PoolId = poolFromDb.Id
                    };
                    _context.EventPools.Add(eventPool);

                    // Add the player to the event/pool
                    var eventPoolPlayer = new EventPoolPlayer
                    {
                        PlayerId = poolFromDb.AdminPlayerId,
                        EventId = item.EventId,
                        PoolId = poolFromDb.Id,
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

                    // Set this so when user goes on the home page it updates the screen
                    Helper.Cache.SetCachedItem("ForceUpdate*" + poolFromDb.AdminPlayerId + "*" + item.EventId, DateTime.Now.AddHours(1));

                }
                _context.SaveChanges();
            }
            else
            {

                var itemList = _context.EventPoolPlayers
                    .Include(e => e.Event)
                    .Where(a => a.PoolId == poolModel.Id && a.Event.EndDateTime >= DateTime.UtcNow)
                    .Select(x => new {x.PlayerId, x.EventId}).Distinct().ToList();

                foreach (var item in itemList)
                {
                    Helper.Cache.SetCachedItem("ForceUpdate*" + item.PlayerId + "*" + item.EventId, DateTime.Now.AddDays(7));
                }
            }

            return RedirectToAction("Index", "PoolDashboard");
        }

        public ActionResult Delete(int id)
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var pool = _context.Pools.FirstOrDefault(a => a.Id == id);
            if (pool == null) return RedirectToAction("Index", "Home");

            var loggedInUserId = User.Identity.GetUserId();
            var isPoolAdmin = _context.Pools.Any(o => o.Id == id && o.AdminPlayerId == loggedInUserId);

            if (!isPoolAdmin) throw new Exception("Only pool admin is allowed to edit the pool");

            // Remove all the players from the pool
            var poolPlayers = _context.EventPoolPlayers.Where(b => b.PoolId == id);
            foreach (var poolPlayer in poolPlayers) _context.EventPoolPlayers.Remove(poolPlayer);

            var eventPools = _context.EventPools.Where(b => b.PoolId == id);
            foreach (var eventPool in eventPools) _context.EventPools.Remove(eventPool);

            _context.Pools.Remove(pool);
            _context.SaveChanges();
            return RedirectToAction("Index", "PoolDashboard");
        }

        public ActionResult Players(int id)
        {
            return RedirectToAction("PoolAdmin", "PoolPlayers", new { id });
        }


        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
            base.Dispose(disposing);
        }
    }
}