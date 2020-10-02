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
        public ActionResult AdminHome()
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            if (!User.IsInRole("Admin")) return HttpNotFound();

            return View();
        }

        [HttpPost]
        public ActionResult SaveUpdateScoring()
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            Helper.Cache.UpdateScoring(_context);

            return RedirectToAction("AdminHome", "Admin");
        }

        [HttpPost]
        public ActionResult PremierLeagueUpdate()
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            RapidApi.RapidApiHelper.UpdatePremierLeague();

            return RedirectToAction("AdminHome", "Admin");
        }        
    }
}