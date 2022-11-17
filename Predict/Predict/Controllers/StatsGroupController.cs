using Predict.Models;
using Predict.ViewModels;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;

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
        public ActionResult Index(short eventId, int poolId = 0)
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            var eventFixtures = _context.EventFixtures
                .Include(b => b.Fixture.HomeTeam)
                .Include(b => b.Fixture.AwayTeam)
                .Where(p => p.EventId == eventId)
                .OrderBy(a => a.Fixture.FixtureDateTime).ToList();

            var poolName = "";

            if (poolId != 0)
            {
                var pool = _context.Pools.FirstOrDefault(a => a.Id == poolId);
                if(pool!=null)
                    poolName = pool.PoolName;
            }


            var showKoStats = _context.KoFixtures.Any(a => a.EventId == eventId);

            var myEvent = Helper.Cache.GetCachedEvent(eventId);

            ViewBag.ShowKoStats = showKoStats;
            ViewBag.EventId = eventId;
            ViewBag.PoolId = poolId;
            ViewBag.PoolName = poolName;
            ViewBag.International = myEvent.International;
            return View(eventFixtures);
        }

        // GET: StatsFixture
        public ActionResult StatsFixture(int id, short eventId, int poolId)
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            var myEvent = Helper.Cache.GetCachedEvent(eventId);

            var statsFixtureViewModel = new StatsFixtureViewModel
            {
                Fixture = _context.Fixtures
                    .Include(t => t.HomeTeam)
                    .Include(t => t.AwayTeam)
                    .SingleOrDefault(f => f.Id == id),

                StatFixturePredictions = _context.Database.SqlQuery<StatFixturePrediction>(
                    "spGetStatsFixture @intFixtureId, @intEventId, @intPoolId"
                    , new SqlParameter("@intFixtureId", id)
                    , new SqlParameter("@intEventId", eventId)
                    , new SqlParameter("@intPoolId", poolId)
                ).ToList()
            };
            statsFixtureViewModel.EventId = eventId;
            statsFixtureViewModel.International = myEvent.International;

            var poolName = "";

            if (poolId != 0)
            {
                var pool = _context.Pools.FirstOrDefault(a => a.Id == poolId);
                if (pool != null)
                    poolName = pool.PoolName;
            }

            ViewBag.PoolId = poolId;
            ViewBag.PoolName = poolName;

            return View(statsFixtureViewModel);
        }
    }
}