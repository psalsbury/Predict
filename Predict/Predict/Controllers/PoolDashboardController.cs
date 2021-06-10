using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Predict.Models;
using Predict.ViewModels;

namespace Predict.Controllers
{
    public class PoolDashboardController : Controller
    {

        private readonly ApplicationDbContext _context;


        public PoolDashboardController()
        {
            _context = new ApplicationDbContext();
        }

        // GET: PoolDashboard
        public ActionResult Index()
        {

            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");


            var playerId = User.Identity.GetUserId();

            var pools = _context.Pools.Where(a => a.AdminPlayerId == playerId).ToList();

            var nbrPoolsWithComp = (from dr in _context.EventPools
                join e in _context.Pools on dr.PoolId equals e.Id
                where e.AdminPlayerId == playerId
                select dr.PoolId).Distinct().Count();

            var nbrPoolsOwnedButNotJoined = (from dr in _context.EventPoolPlayers
                join e in _context.Pools on dr.PoolId equals e.Id
                where e.AdminPlayerId == playerId
                && dr.PlayerId == playerId
                && dr.Enabled == true
                select dr.PoolId).Distinct().Count();

            var poolDashboardViewModel = new PoolDashboardViewModel
            {
                NumberOfPoolsAdminOf = pools.Count()
                , NumberOfPoolsMemberOf = _context.PoolPlayers.Count(a => a.PlayerId == playerId && a.Enabled==true)
                , HasPoolWithoutComp = nbrPoolsWithComp < pools.Count() ? true : false
                , HasOwnedPoolsButNotAMember = nbrPoolsOwnedButNotJoined < pools.Count ? true : false
            };

            return View(poolDashboardViewModel);
        }
    }
}