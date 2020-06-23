using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Predict.Models;

namespace Predict.Controllers
{
    public class PlayersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PlayersController()
        {
            _context = new ApplicationDbContext();
        }

        // GET: Players
        public ActionResult Index()
        {
            var players = _context.Players.Include(p => p.AspNetUser).ToList();
            return View(players);
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
            base.Dispose(disposing);
        }
    }
}