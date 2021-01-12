using Predict.Helper;
using Predict.Models;
using Predict.ViewModels;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace Predict.Controllers
{
    public class ResultsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ResultsController()
        {
            _context = new ApplicationDbContext();
        }

        // GET: LeagueTableResults
        public ActionResult LeagueTableResults(int eventId)
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");


            var leagueTablesViewModel = new LeagueTablesViewModel();
            leagueTablesViewModel.LeagueTables = LeagueTableHelper.FetchLeagueTablesFromResults(eventId);
            leagueTablesViewModel.Results = true;
            return View("LeagueTables", leagueTablesViewModel);
        }

        // GET: Results
        public ActionResult GroupGameResults(int id)
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var groupGameResultsViewModel = new GroupGameResultsViewModel();

            var eventFixtures = _context.EventFixtures
                .Include(a => a.Fixture)
                .Include(l => l.Fixture.League)
                .Include(b => b.Fixture.HomeTeam)
                .Include(b => b.Fixture.AwayTeam)
                .Where(p => p.EventId == id).ToList()
                .OrderBy(p => p.Fixture.FixtureDateTime).ToList();

            var myEvent = Helper.Cache.GetCachedEvent(id);

            groupGameResultsViewModel.EventFixtures = eventFixtures;
            ViewBag.EventId = id;
            ViewBag.EventName = myEvent.EventName;

            return View(groupGameResultsViewModel);
        }

        // GET: Results
        public ActionResult KOResults(short eventId)
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var koFixtureController = new KoFixturesController();
            var koFixturePredictionViewModel = koFixtureController.GetKoFixturePredictionViewModel(eventId);
            koFixturePredictionViewModel.ReadOnly = true;

            return View("KoFixturePredictions", koFixturePredictionViewModel);
        }
    }
}