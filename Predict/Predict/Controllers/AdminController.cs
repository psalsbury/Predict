using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Predict.Models;

namespace Predict.Controllers
{
    public class AdminController : Controller
    {
        private ApplicationDbContext _context;
        public AdminController()
        {
            _context = new ApplicationDbContext();
        }
        // GET: Admin
        public ActionResult UpdateScoring()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SaveUpdateScoring()
        {
            // Run stored procedure to update all scoring
            var eventId = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["EventId"]);
            var today = DateTime.Today;
            var eventIdParam = new SqlParameter("@intEventId", eventId);
            var todayParam = new SqlParameter("@dteDate", today);
            _context.Database.ExecuteSqlCommand("EXEC spProcessScores @intEventId, @dteDate", eventIdParam, todayParam);

            return RedirectToAction("Index", "Home");
        }
    }
}