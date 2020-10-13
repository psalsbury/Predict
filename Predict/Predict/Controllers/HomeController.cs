using Microsoft.AspNet.Identity;
using Predict.Helper;
using Predict.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Predict.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController()
        {
            _context = new ApplicationDbContext();
        }
        public ActionResult Index()
        {
            EventPlayer eventPlayer = null;
            if (User.Identity.IsAuthenticated)
            {
                SessionHelper.SetUserSessionVariables(Session, User.Identity.GetUserId(), false);

                // Check if result is needed to be checked
                Helper.Cache.GetRapidApiResults();

                var eventId = System.Convert.ToInt16(Request["EventId"]);
                if (eventId == 0)
                {
                    eventPlayer = SessionHelper.GetOrderedEventsForPlayers(Session).FirstOrDefault();
                }
                else
                {
                    eventPlayer =
                        ((List<EventPlayer>)Session["EventPlayers"]).FirstOrDefault(e => e.EventId == eventId);
                }

                SessionHelper.UpdateSessionForHomePage(_context, Session, eventPlayer, false);

            }
            return View(eventPlayer);
        }

        public ActionResult ContactUs()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendMessage()
        {
            var message = Request["message"];
            var from = User.Identity.Name;

            var emailMesesage = new IdentityMessage
            {
                Body = message,
                Destination = "pete@salsbury.co.uk",
                Subject = string.Format("Query from {0}", from ?? "Unknown")
            };

            Cache.SendEmail(emailMesesage);
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Rules()
        {
            ViewBag.Message = "Rules.";
            ViewBag.EventName = "Prediction Competition";

            return View();
        }
    }
}