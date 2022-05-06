using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web.Mvc;
using Predict.Models;
using Predict.ViewModels;
using System.Collections.Generic;

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

            var leagues = _context.Leagues.OrderByDescending(a => a.CreatedDateTime).ToList();
            return View(leagues);
        }

        public ActionResult EditLeague(int leagueId)
        {
            var league = _context.Leagues.SingleOrDefault(a => a.Id == leagueId);
            if(league==null)
            {
                league = new League();
            }

            ViewBag.rapidApiV3LeagueSeasons = GetrapidApiV3LeagueSeasons();
            return View(league);

        }

        private SelectList GetrapidApiV3LeagueSeasons()
        {
            var rapidApiV3LeagueSeasons = _context.RapidApiV3LeagueSeasons
            .Include(a => a.RapidApiV3League)
            .Where(a => a.Current == true)
            .Select(s => new
            {
                Id = s.Id,
                Description = s.RapidApiV3League.CountryName + " " + s.RapidApiV3League.Name + " " + s.Year.ToString()
            })
            .ToList();

            return new SelectList(rapidApiV3LeagueSeasons, "Id", "Description", "League /Season");
        }

        [HttpPost]
        public ActionResult Save(League league)
        {

            if (!ModelState.IsValid)
            {
                ViewBag.rapidApiV3LeagueSeasons = GetrapidApiV3LeagueSeasons();
                return View("EditLeague", league);
            }

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