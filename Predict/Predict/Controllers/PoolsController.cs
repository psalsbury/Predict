using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Predict.Models;
using Predict.ViewModels;
using AutoMapper;
using System.Data.Entity;
using Microsoft.AspNet.Identity;

namespace Predict.Controllers
{
    public class PoolsController : Controller
    {
        private ApplicationDbContext _context;
        public PoolsController()
        {
            _context = new ApplicationDbContext();
        }

        // GET: Pools
        public ActionResult Index(short eventId)
        {
            List<Pool> pools;
            if (User.IsInRole("Admin"))
            {
                // Admin of the site can see all pools
                pools = _context.Pools.Include(b => b.AdminPlayer).Where(p => p.EventId == eventId).ToList();
            }
            else
            {
                // Normal user can see only their pools
                var userid = User.Identity.GetUserId();
                pools = _context.Pools.Include(b => b.AdminPlayer).Where(p => p.EventId == eventId && p.AdminPlayerId == userid).ToList();
            }

            return View(pools);  
        }


        // GET: Pools
        public ActionResult PoolMembershipIndex(short eventId)
        {
            var playerId = User.Identity.GetUserId();
            var poolMembershipViewModel = new PoolMembershipViewModel();
            var globalPoolId =
                System.Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["GlobalPoolId"]);

            poolMembershipViewModel.ReadOnlyPools.Add(globalPoolId);

            var adminPools = _context.Pools
                .Where(p => p.AdminPlayerId == playerId)
                .Where(p => p.EventId == eventId);

            foreach(Pool pool in adminPools)
            {
                poolMembershipViewModel.ReadOnlyPools.Add(pool.Id);
            }

           // Admin of the site can see all pools
           var availablePools = _context.Pools.Include(b => b.AdminPlayer).Where(p => p.EventId == eventId).ToList();

           // Normal user can see only their pools           
           var joinedPools = _context.PoolPlayers
                .Include(b => b.Pool)
               .Where(p => p.Pool.EventId==eventId)
               .Where(p => p.PlayerId == playerId).ToList();
           poolMembershipViewModel.Pools = availablePools;
           poolMembershipViewModel.JoinedPools = joinedPools;

           return View("PoolMembershipIndex", poolMembershipViewModel);
        }


        public ActionResult New(short eventId)
        {
            var poolModel = new Pool
            {
                AdminPlayerId = User.Identity.GetUserId()
                ,EventId = eventId
                ,CorrectScorePoints = 3
                ,CorrectResultPoints = 1
                ,WinMarginPoints = 0
                ,KoLast16Points = 1
                ,KoLast8Points = 2
                ,KoLast4Points = 4
                ,KoLast2Points = 6
                ,KoLast1Points = 10
                ,FreezePredictions = false
            };
            
            return View("EditPool", poolModel);
        }


        public ActionResult Edit(int id)
        {
            
            var loggedInUserId = User.Identity.GetUserId();
            var isPoolAdmin = _context.Pools.Any(o => o.Id == id && o.AdminPlayerId == loggedInUserId);
            var poolModel = _context.Pools.SingleOrDefault(p => p.Id == id);
            var isInLockDown = Predict.Helper.Cache.HasEventStarted(poolModel.EventId);
            if (!isPoolAdmin)
            {
                throw new Exception("Only pool admin is allowed to edit the pool");
            }

            ViewBag.isInLockDown = isInLockDown;
            return View("EditPool", poolModel);
        }


        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(Pool poolModel)
        {
            var poolFromDb = new Pool();
            if (poolModel.Id != 0)
            {
                poolFromDb = _context.Pools.Single(m => m.Id == poolModel.Id);
            }

            Mapper.Map(poolModel, poolFromDb);
            poolFromDb.ModifiedDateTime = DateTime.Now;

            if (poolModel.Id == 0)
            {
                poolFromDb.CreatedDateTime = DateTime.Now;              
                _context.Pools.Add(poolFromDb);

                // Need to add the current player in the PoolPlayer table
                var poolPlayer = new PoolPlayer
                {
                    PlayerId = poolFromDb.AdminPlayerId,
                    PoolPosition = 1, // Default position
                    CreatedDateTime = DateTime.Now,
                    ModifiedDateTime = DateTime.Now
                };
                _context.PoolPlayers.Add(poolPlayer);

            }
            _context.SaveChanges();

            return RedirectToAction("Index", "Pools");
        }

        public ActionResult Delete(int id)
        {
            var pool = _context.Pools.FirstOrDefault(a => a.Id == id);
            if (pool==null)
            {
                return HttpNotFound();
            }

            var eventId = pool.EventId;
            var loggedInUserId = User.Identity.GetUserId();
            var isInLockDown = Predict.Helper.Cache.HasEventStarted(eventId);
            var isPoolAdmin = _context.Pools.Any(o => o.Id == id && o.AdminPlayerId == loggedInUserId);

            if (!isPoolAdmin)
            {
                throw new Exception("Only pool admin is allowed to edit the pool");
            }

            if(isInLockDown)
            {
                throw new Exception("Pool cannot be removed after the tournament has started");
            }


            // Remove all the players from the pool
            var poolPlayers = _context.PoolPlayers.Where(b => b.PoolId==id);
            foreach (var poolPlayer in poolPlayers)
            {
                _context.PoolPlayers.Remove(poolPlayer);
            }

            _context.Pools.Remove(pool);
            _context.SaveChanges();
            return RedirectToAction("Index", "Pools");
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