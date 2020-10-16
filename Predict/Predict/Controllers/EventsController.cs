using Microsoft.AspNet.Identity;
using Predict.Models;
using System;
using System.Configuration;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web.Mvc;

namespace Predict.Controllers
{
    public class EventsController : Controller
    {

        private readonly ApplicationDbContext _context;

        public EventsController()
        {
            _context = new ApplicationDbContext();
        }

        private bool CheckUserIsValid()
        {
            if (!User.Identity.IsAuthenticated || !User.IsInRole("Admin")) return false;
            return true;
        }

        // GET: Events
        public ActionResult EventsIndex()
        {
            if (!CheckUserIsValid())
            {
                return RedirectToAction("Index", "Home");
            }

            var events = _context.Events.ToList();
            return View(events);
        }

        public ActionResult AddEvent()
        {
            if (!CheckUserIsValid()) return RedirectToAction("Index", "Home");

            var myEvent = new Event();
            return View("EditEvent", myEvent);
        }

        public ActionResult EditEvent(int id)
        {
            if (!CheckUserIsValid()) return RedirectToAction("Index", "Home");

            var myEvent = _context.Events.FirstOrDefault(e => e.Id == id);
            return View(myEvent);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveEvent(Event passedInEvent)
        {
            if (!CheckUserIsValid()) return RedirectToAction("Index", "Home");

            var playerId = User.Identity.GetUserId();
            var globalPoolId = (System.Convert.ToInt32(ConfigurationManager.AppSettings["GlobalPoolId"]));
            var myEvent = _context.Events.FirstOrDefault(e => e.Id == passedInEvent.Id);
            if (myEvent == null)
            {
                myEvent = new Event
                {
                    CreatedDateTime = DateTime.UtcNow
                    , DefaultPoolId = globalPoolId
                    , CreatedByPlayerId = playerId
                };
            }

            myEvent.ModifiedDateTime = DateTime.UtcNow;
            myEvent.EventName = passedInEvent.EventName;
            myEvent.EventDescription = passedInEvent.EventDescription;
            _context.Events.AddOrUpdate(myEvent);
            _context.SaveChanges();

            // Ensure that the default pool is associated to the to the event
            var eventPool =
                _context.EventPools.FirstOrDefault(a => a.EventId == myEvent.Id && a.PoolId == globalPoolId);
            if (eventPool == null)
            {
                eventPool = new EventPool
                {
                    EventId = myEvent.Id,
                    PoolId = globalPoolId,
                    CreatedDateTime = DateTime.UtcNow,
                    ModifiedDateTime = DateTime.UtcNow,
                    Enabled = true

                };
                _context.EventPools.AddOrUpdate(eventPool);
                _context.SaveChanges();
            }

            // update the application cache for events
            Helper.Cache.SetEventCache();

            return RedirectToAction("EventsIndex", "Events");
        }

    }
}