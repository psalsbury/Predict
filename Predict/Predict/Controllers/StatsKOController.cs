using Predict.Models;
using Predict.ViewModels;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace Predict.Controllers
{
    public class StatsKoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StatsKoController()
        {
            _context = new ApplicationDbContext();
        }


        // GET: StatsKO
        public ActionResult Index(short eventId, int poolId = 0)
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var statsKoViewModel = new StatsKoViewModel
            {
                StatsKoRoundOfs = _context.Database.SqlQuery<StatsKoRoundOf>("spGetStatsKo @intEventId, @intPoolId"
                    , new SqlParameter("@intEventId", eventId)
                    , new SqlParameter("@intPoolID", poolId)
                ).ToList()
            };

            var poolName = "";
            if (poolId != 0)
            {
                var pool = _context.Pools.FirstOrDefault(a => a.Id == poolId);
                if (pool != null)
                    poolName = pool.PoolName;
            }

            ViewBag.EventId = eventId;
            ViewBag.PoolId = poolId;
            ViewBag.PoolName = poolName;

            return View(statsKoViewModel);
        }
    }
}