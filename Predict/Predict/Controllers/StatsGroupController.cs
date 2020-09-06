using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using Predict.Models;
using Predict.ViewModels;

namespace Predict.Controllers
{
    public class StatsGroupController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StatsGroupController()
        {
            _context = new ApplicationDbContext();
        }

        // GET: StatsGroup
        public ActionResult Index(short eventId)
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var eventFixtures = _context.EventFixtures
                .Include(b => b.Fixture.HomeTeam)
                .Include(b => b.Fixture.AwayTeam)
                .Where(p => p.EventId == eventId).ToList();

            return View(eventFixtures);
        }

        // GET: StatsFixture
        public ActionResult StatsFixture(int id)
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var statsFixtureViewModel = new StatsFixtureViewModel
            {
                Fixture = _context.Fixtures
                    .Include(t => t.HomeTeam)
                    .Include(t => t.AwayTeam)
                    .SingleOrDefault(f => f.Id == id),

                StatFixturePredictions = _context.Database.SqlQuery<StatFixturePrediction>(
                    "spGetStatsFixture @intFixtureId"
                    , new SqlParameter("@intFixtureId", id)
                ).ToList()
            };

            return View(statsFixtureViewModel);
        }
    }
}