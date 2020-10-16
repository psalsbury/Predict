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
            var poolDashboardViewModel = new PoolDashboardViewModel
            {
                NumberOfPoolsAdminOf = _context.Pools.Count(a => a.AdminPlayerId == playerId)
                , NumberOfPoolsMemberOf = _context.PoolPlayers.Count(a => a.PlayerId == playerId && a.Enabled==true)
            };

            return View(poolDashboardViewModel);
        }
    }
}