using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Predict.Models;

namespace Predict.Controllers
{
    public class LeaguesController : Controller
    {

        private readonly ApplicationDbContext _context;

        public LeaguesController()
        {
            _context = new ApplicationDbContext();
        }

        // GET: Leagues
        public ActionResult LeaguesIndex()
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var leagues = _context.Leagues.ToList();

            return View(leagues);
        }

        public ActionResult EditLeague(int leagueId)
        {
            var league = _context.Leagues.SingleOrDefault(a => a.Id == leagueId);

            return View(league);

        }

        public ActionResult Save(League league)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            league.ModifiedDateTime = DateTime.UtcNow;

            if (league.Id == 0)
                league.CreatedDateTime = DateTime.UtcNow;

            _context.Leagues.AddOrUpdate(league);
            _context.SaveChanges();

            return RedirectToAction("LeaguesIndex");
        }
    }
}