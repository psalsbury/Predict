using System.Linq;
using System.Web.Mvc;
using Predict.Models;

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