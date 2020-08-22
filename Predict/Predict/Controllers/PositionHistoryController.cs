using System.Linq;
using System.Web.Mvc;
using Predict.Models;

namespace Predict.Controllers
{
    public class PositionHistoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PositionHistoryController()
        {
            _context = new ApplicationDbContext();
        }

        // GET: PosnHistory
        public ActionResult Index(string playerId, int poolId)
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var positionHistory = (from a in _context.PoolPlayerPositionHistory
                join c in _context.Pools on a.PoolId equals c.Id
                where a.PlayerId == playerId
                      && c.Id == poolId
                select a).ToList();
            return View();
        }
    }
}