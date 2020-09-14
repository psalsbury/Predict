using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Predict.Models;

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
            if( !CheckUserIsValid()) return HttpNotFound();

            var events = _context.Events.ToList();
            return View(events);
        }

        public ActionResult AddEvent()
        {
            if (!CheckUserIsValid()) return HttpNotFound();

            var myEvent = new Event();
            return View("EditEvent",myEvent);
        }

        public ActionResult EditEvent(int id)
        {
            if (!CheckUserIsValid()) return HttpNotFound();

            var myEvent = _context.Events.FirstOrDefault(e => e.Id == id);
            return View(myEvent);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveEvent(Event passedInEvent)
        {
            if (!CheckUserIsValid()) return HttpNotFound();

            var playerId = User.Identity.GetUserId();
            var globalPoolId = (System.Convert.ToInt32(ConfigurationManager.AppSettings["GlobalPoolId"]));
            Event myEvent;
            if (passedInEvent.Id==0)
            {
                myEvent = new Event
                {
                    CreatedDateTime = DateTime.Now
                    ,DefaultPoolId = globalPoolId
                };
            }
            else
            {
                myEvent = _context.Events.FirstOrDefault(e => e.Id == passedInEvent.Id);
            }

            myEvent.ModifiedDateTime = DateTime.Now;
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
                    CreatedDateTime = DateTime.Now,
                    ModifiedDateTime = DateTime.Now
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