using System;
using Predict.Models;
using System.Web.Mvc;

namespace Predict.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController()
        {
            _context = new ApplicationDbContext();
        }

        // GET: Admin
        public ActionResult AdminHome()
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            if (!User.IsInRole("Admin")) return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        public ActionResult SaveUpdateScoring()
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            Helper.Cache.UpdateScoring(_context);

            return RedirectToAction("AdminHome", "Admin");
        }

        [HttpPost]
        public ActionResult PremierLeagueUpdate()
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var rapidApiLeagueId = 2790;
            RapidApi.RapidApiHelper.FixturesByLeague(rapidApiLeagueId, DateTime.MinValue);

            return RedirectToAction("AdminHome", "Admin");
        }
        [HttpPost]
        public ActionResult ChampLeagueUpdate()
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var rapidApiLeagueId = 2794;
            RapidApi.RapidApiHelper.FixturesByLeague(rapidApiLeagueId, DateTime.MinValue);

            return RedirectToAction("AdminHome", "Admin");
        }
    }
}