using System.Linq;
using System.Web.Mvc;
using Predict.Models;
using System.Data.Entity;
using System.Data.SqlClient;
using Predict.ViewModels;

namespace Predict.Controllers
{
    public class StatsGroupController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly int _eventId;

        public StatsGroupController()
        {
            _context = new ApplicationDbContext();
            _eventId = Helper.Cache.GetEventId();
        }
        // GET: StatsGroup
        public ActionResult Index()
        {            
            var fixtures = _context.Fixtures.Include(b => b.HomeTeam)
                .Include(b => b.AwayTeam)
                .Where(p => p.EventId == _eventId).ToList();

            return View(fixtures);
        }


        public ActionResult StatsFixture(int id)
        {
            var statsFixtureViewModel = new StatsFixtureViewModel
            {
                Fixture = _context.Fixtures
                .Include(t => t.HomeTeam)
                .Include(t => t.AwayTeam)
                .SingleOrDefault(f => f.Id == id),

                StatFixturePredictions = _context.Database.SqlQuery<StatFixturePrediction>("spGetStatsFixture @intFixtureId"
                , new SqlParameter("@intFixtureId", id)
            ).ToList()
            };

            return View(statsFixtureViewModel);
        }
    }
}