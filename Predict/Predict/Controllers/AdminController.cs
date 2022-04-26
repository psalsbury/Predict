using System;
using Predict.Models;
using System.Web.Mvc;

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

            if (!User.IsInRole("Admin")) return RedirectToAction("Index", "Home");

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
        public ActionResult ForceDailyUpdate()
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            RapidApi.RapidApiHelper.ForceDailyRapidApiLeagueCheck();

            return RedirectToAction("AdminHome", "Admin");
        }

        [HttpPost]
        public ActionResult V3Leagues()
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var country = HttpContext.Request.Params.Get("Country");
            var year = System.Convert.ToInt32(HttpContext.Request.Params.Get("year"));

            RapidApi.RapidApiHelper.V3Leagues (country, year);

            return RedirectToAction("AdminHome", "Admin");
        }
    }

}
