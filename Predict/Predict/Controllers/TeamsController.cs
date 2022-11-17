using Predict.Models;
using System.Linq;
using System.Web.Mvc;

namespace Predict.Controllers
{
    public class TeamsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TeamsController()
        {
            _context = new ApplicationDbContext();
        }

        //GET : Teams
        public ActionResult Index()
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            var teams = _context.Teams.ToList();
            return View(teams);
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
            base.Dispose(disposing);
        }
    }
}