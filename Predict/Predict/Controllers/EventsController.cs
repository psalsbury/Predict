using System;
using System.Collections.Generic;
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

        [ValidateAntiForgeryToken]
        public ActionResult SaveEvent(Event passedInEvent)
        {
            if (!CheckUserIsValid()) return HttpNotFound();

            Event myEvent;
            if (passedInEvent.Id==0)
            {
                myEvent = new Event
                {
                    CreatedDateTime = DateTime.Now
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

            // If no default pool has been created, then add one
            if(myEvent.DefaultPoolId == 0)
            {
                var pool = new Pool
                {
                    PoolName = "Global Pool",
                    CreatedDateTime = DateTime.Now,
                    ModifiedDateTime = DateTime.Now,
                    CorrectScorePoints = 5,
                    CorrectResultPoints = 2,
                    WinMarginPoints = 1,
                    EmailNotifications = false,
                    DefaultPoolForEvent = true,
                    AdminPlayerId = User.Identity.GetUserId(),
                    EventId = myEvent.Id
                };
                _context.Pools.Add(pool);
                _context.SaveChanges();

                var poolPlayer = new PoolPlayer
                {
                    CreatedDateTime = DateTime.Now,
                    ModifiedDateTime = DateTime.Now,
                    PlayerId = User.Identity.GetUserId(),
                    PoolId = pool.Id,
                    AdminApprovedDateTime = DateTime.Now
                };
                _context.PoolPlayers.Add(poolPlayer);

                myEvent.DefaultPoolId = pool.Id;
                _context.SaveChanges();
            }

            return View(myEvent);
        }

    }
}