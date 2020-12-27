using Predict.Models;
using System.Linq;
using System.Web.Mvc;

namespace Predict.Controllers
{
    public class PositionHistoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PositionHistoryController()
        {
            _context = new ApplicationDbContext();
        }

        [HttpGet]
        public ActionResult ShowHistory(short eventId, int poolId, string playerId)
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var positionHistory = (from a in _context.EventPoolPlayerPositionHistory
                                   join c in _context.Pools on a.PoolId equals c.Id
                                   join e in _context.Events on a.EventId equals e.Id
                                   where a.PlayerId == playerId
                                         && c.Id == poolId
                                         && a.EventId == eventId
                                   select a).ToList();

            var nbrPlayersInPool = _context.EventPoolPlayers.Count(a => a.EventId == eventId && a.PoolId == poolId);

            ViewBag.NumberPlayers = nbrPlayersInPool;
            ViewBag.EventId = eventId;
            return View(positionHistory);
        }
    }
}