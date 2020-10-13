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
        public ActionResult Index(short eventId)
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var statsKoViewModel = new StatsKoViewModel
            {
                StatsKoRoundOfs = _context.Database.SqlQuery<StatsKoRoundOf>("spGetStatsKo @intEventId"
                    , new SqlParameter("@intEventId", eventId)
                ).ToList()
            };

            return View(statsKoViewModel);
        }
    }
}