using Microsoft.AspNet.Identity;
using Predict.Models;
using Predict.RapidApi;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Runtime.Caching;
using Predict.Helper;
using System.Runtime.InteropServices;
using System.Web.Mvc;

namespace Predict.Controllers
{
    public class CompsDashboardController : Controller
    {

        private readonly ApplicationDbContext _context;

        public CompsDashboardController()
        {
            _context = new ApplicationDbContext();
        }

        // GET: CompsDashboard
        public ActionResult CompsDashboardIndex()
        {
            // home screen to show 
            // 1) Join a new comp (shows comps that are unfinished)
            // 2) Show comps youve participated in (number of completed comps)
            // 3) Create a new comp
            // 4) Manage your comps
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var dteNow = DateTime.UtcNow;
            var playerId = User.Identity.GetUserId();
            var events = (List<Event>)Helper.Cache.GetCachedItem("Events");
            var eventPlayers = (List<EventPlayer>)Session["EventPlayers"];
            var eventIdsPlaying = eventPlayers.Where(a => a.Event.EventFinished==false).Select(a => a.EventId).ToList();

            var nbrParticipatingIn = eventPlayers.Where(a => a.Event.EventFinished == false).Count();
            var nbrUpcoming = events.Where(a => a.EventFinished == false && !eventIdsPlaying.Contains(a.Id)).Count();

            var nbrCompletedEvents = _context.EventPlayers.Where(a => a.Event.EndDateTime < dteNow && a.PlayerId==playerId).Count();
            var nbrOwnedEvents = _context.Events.Where(a => a.CreatedByPlayerId==playerId).Count();

            // find if the user has any comps without fixtures
            var eventsNoFixtures = from a in _context.Events
                        join b in _context.EventFixtures
                            on a.Id equals b.EventId into c
                        from b in c.DefaultIfEmpty()
                        where b == null
                        & a.CreatedByPlayerId == playerId
                        select a;

            var eventNoFixtures = eventsNoFixtures.FirstOrDefault();
            if (eventNoFixtures != null)
            {
                ViewBag.compsWithoutFixtures = eventNoFixtures.EventName;
            }

            // find if the user has any comps that ther are not playing
            var eventsNotPlaying = from a in _context.Events
                        join b in _context.EventPlayers
                            on a.Id equals b.EventId into c
                        from b in c.DefaultIfEmpty()
                        where b == null
                        & a.CreatedByPlayerId == playerId
                        & (a.StartDateTime > dteNow | a.EndDateTime > dteNow)
                        select a;

            var eventNotPlaying = eventsNotPlaying.FirstOrDefault();
            if (eventNotPlaying != null)
            {
                ViewBag.eventsNotPlaying = eventNotPlaying.EventName;
            }

            ViewBag.nbrUpcoming = nbrUpcoming;
            ViewBag.nbrParticipatingIn = nbrParticipatingIn;
            ViewBag.nbrCompletedEvents = nbrCompletedEvents;
            ViewBag.nbrOwnedEvents = nbrOwnedEvents;
            return View();
        }
    }
}