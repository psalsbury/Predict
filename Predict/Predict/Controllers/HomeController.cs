using Microsoft.AspNet.Identity;
using Predict.Helper;
using Predict.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;


namespace Predict.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController()
        {
            _context = new ApplicationDbContext();
        }

        public ActionResult _SideBar()
        {
            return View();
        }
        public ActionResult Index()
        {
            EventPlayer eventPlayer = null;
            if (!User.Identity.IsAuthenticated)
                return View(eventPlayer);

            var playerId = User.Identity.GetUserId();

            SessionHelper.SetUserSessionVariables(Session, playerId, false);

            var eventId = System.Convert.ToInt16(Request["EventId"]);

            if (Helper.Cache.GetCachedEvent(eventId) == null)
            {
                Helper.Cache.SetEventCache(eventId);
            }

            if (eventId == 0)
            {
                eventPlayer = SessionHelper.GetOrderedEventsForPlayers(Session).FirstOrDefault();
                if (eventPlayer != null)
                {
                    eventId = eventPlayer.EventId;
                }
            }
            else
            {
                eventPlayer =
                    ((List<EventPlayer>)Session["EventPlayers"]).FirstOrDefault(e => e.EventId == eventId);

                if(eventPlayer==null)
                {
                    eventPlayer = _context.EventPlayers
                        .Include(a => a.Event)
                        .Where(a => a.EventId == eventId && a.PlayerId == playerId).FirstOrDefault();
                    SessionHelper.AddEventPlayerToSessionVariable(eventPlayer, Session);
                    SessionHelper.UpdateSessionForHomePage(_context, Session, eventPlayer, false, true);
                }
            }

            if (eventPlayer != null)
            {
                // If the user has just added themselves to a pool, ensure the home page is refreshed
                var cacheItem = "ForceUpdate*" + User.Identity.GetUserId() + "*" + eventPlayer.EventId;
                var forcePoolRefresh = Helper.Cache.GetCachedItem(cacheItem) != null;
                if (forcePoolRefresh)
                {
                    Helper.Cache.RemoveCachedItem(cacheItem);
                }
                // Also want to set forcePoolRefresh to true if the cached event has been modified
                if (eventPlayer.Event.ModifiedDateTime < Helper.Cache.GetCachedEvent(eventPlayer.EventId).ModifiedDateTime)
                {
                    forcePoolRefresh = true;
                }
                var nbrPoolsInEvent = _context.EventPools.Count(a => a.EventId == eventId && a.Enabled == true);
                ViewBag.nbrPoolsInEvent = nbrPoolsInEvent;

                // Used forcePoolRefresh as forceRefresh as fixtures could be added to a comp and that needs refreshing on users home page
                SessionHelper.UpdateSessionForHomePage(_context, Session, eventPlayer, forcePoolRefresh, forcePoolRefresh);
                ViewBag.Title = eventPlayer.Event.EventName;
                if (eventPlayer.Event.EventFinished)
                {
                    ViewBag.TitleColour = "Tomato";
                }

            }

            return View(eventPlayer);
        }

        public ActionResult ComingSoon()
        {
            return View();
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
            var from = User.Identity.Name + " " + Request["email"];

            var emailMesesage = new IdentityMessage
            {
                Body = message,
                Destination = "pete@salsbury.co.uk",
                Subject = string.Format("Query from {0}", from ?? "Unknown")
            };

            Cache.SendEmail(emailMesesage);
            return RedirectToAction("Index", "Home");
        }

        public ActionResult FeatureRequest()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendFeatureRequest()
        {
            var message = Request["message"];
            var from = User.Identity.Name + " " + Request["email"];

            var emailMesesage = new IdentityMessage
            {
                Body = message,
                Destination = "pete@salsbury.co.uk",
                Subject = string.Format("Feature Request from {0}", from ?? "Unknown")
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