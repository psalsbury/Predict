using Predict.Models;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

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
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

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