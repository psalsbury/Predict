using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using Microsoft.AspNet.Identity;
using Predict.Helper;
using Predict.Models;
using Predict.ViewModels;

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
        public ActionResult Index(short eventId)
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            List<Pool> pools;
            if (User.IsInRole("Admin"))
            {
                // Admin of the site can see all pools
                pools = _context.Pools.Include(b => b.AdminPlayer).ToList();
            }
            else
            {
                // Normal user can see only their pools
                var userid = User.Identity.GetUserId();
                pools = _context.Pools.Include(b => b.AdminPlayer)
                    .Where(p => p.AdminPlayerId == userid).ToList();
            }

            return View(pools);
        }


        // GET: Pools
        public ActionResult PoolMembershipIndex(short eventId)
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var playerId = User.Identity.GetUserId();
            var poolMembershipViewModel = new PoolMembershipViewModel();
            var globalPoolId =
                Convert.ToInt32(ConfigurationManager.AppSettings["GlobalPoolId"]);

            poolMembershipViewModel.ReadOnlyPools.Add(globalPoolId);

            var adminPools = _context.Pools
                .Where(p => p.AdminPlayerId == playerId);

            foreach (var pool in adminPools) poolMembershipViewModel.ReadOnlyPools.Add(pool.Id);

            // Admin of the site can see all pools
            var availablePools = _context.Pools.Include(b => b.AdminPlayer).ToList();

            // Normal user can see only their pools           
            var joinedPools = _context.EventPoolPlayers
                .Include(b => b.Pool)
                .Where(p => p.PlayerId == playerId).ToList();
            poolMembershipViewModel.Pools = availablePools;
            poolMembershipViewModel.JoinedPools = joinedPools;

            return View("PoolMembershipIndex", poolMembershipViewModel);
        }

        public ActionResult New(short eventId)
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var poolModel = new Pool
            {
                AdminPlayerId = User.Identity.GetUserId()
                , CorrectScorePoints = 3
                , CorrectResultPoints = 1
                , WinMarginPoints = 0
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

            var poolFromDb = new Pool();
            if (poolModel.Id != 0) poolFromDb = _context.Pools.Single(m => m.Id == poolModel.Id);

            Mapper.Map(poolModel, poolFromDb);
            poolFromDb.ModifiedDateTime = DateTime.Now;

            if (poolModel.Id == 0)
            {
                poolFromDb.CreatedDateTime = DateTime.Now;
                _context.Pools.Add(poolFromDb);
            }

            _context.SaveChanges();

            return RedirectToAction("Index", "Pools");
        }

        public ActionResult Delete(int id)
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var pool = _context.Pools.FirstOrDefault(a => a.Id == id);
            if (pool == null) return HttpNotFound();

            var loggedInUserId = User.Identity.GetUserId();
            var isInLockDown = false;
            var isPoolAdmin = _context.Pools.Any(o => o.Id == id && o.AdminPlayerId == loggedInUserId);

            if (!isPoolAdmin) throw new Exception("Only pool admin is allowed to edit the pool");

            if (isInLockDown) throw new Exception("Pool cannot be removed after the comp has started");

            // Remove all the players from the pool
            var poolPlayers = _context.EventPoolPlayers.Where(b => b.PoolId == id);
            foreach (var poolPlayer in poolPlayers) _context.EventPoolPlayers.Remove(poolPlayer);

            var eventPools = _context.EventPools.Where(b => b.PoolId == id);
            foreach (var eventPool in eventPools) _context.EventPools.Remove(eventPool);

            _context.Pools.Remove(pool);
            _context.SaveChanges();
            return RedirectToAction("Index", "Pools");
        }

        public ActionResult Players(int id)
        {
            return RedirectToAction("PoolAdmin", "PoolPlayers", new {id});
        }


        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
            base.Dispose(disposing);
        }
    }
}