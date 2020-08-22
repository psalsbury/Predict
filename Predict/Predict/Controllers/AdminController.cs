using System;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using Predict.Models;

namespace Predict.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController()
        {
            _context = new ApplicationDbContext();
        }

        // GET: Admin
        public ActionResult UpdateScoring()
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            if (!User.IsInRole("Admin")) return HttpNotFound();
            var eventId = Convert.ToInt16(Request["EventId"]);
            if (eventId == 0) return HttpNotFound();

            var myEvent = _context.Events.FirstOrDefault(a => a.Id == eventId);
            if (myEvent == null) return HttpNotFound();
            return View(myEvent);
        }

        // GET: Admin
        public ActionResult AdminHome()
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            if (!User.IsInRole("Admin")) return HttpNotFound();

            var eventId = Convert.ToInt16(Request["EventId"]);
            if (eventId == 0) return HttpNotFound();

            var myEvent = _context.Events.FirstOrDefault(a => a.Id == eventId);
            return View(myEvent);
        }

        [HttpPost]
        public ActionResult SaveUpdateScoring(Event myEvent)
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            // Run stored procedure to update all scoring
            var today = DateTime.Today;
            var eventIdParam = new SqlParameter("@intEventId", myEvent.Id);
            var todayParam = new SqlParameter("@dteDate", today);
            _context.Database.ExecuteSqlCommand("EXEC spProcessScores @intEventId, @dteDate", eventIdParam, todayParam);

            // Update the cache for this event
            Helper.Cache.SetEventCache(myEvent.Id);

            return RedirectToAction("Index", "Home");
        }
    }
}