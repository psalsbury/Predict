using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Predict.Models;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using Predict.ViewModels;

namespace Predict.Controllers
{
    public class EventFixturesController : Controller
    {

        private readonly ApplicationDbContext _context;


        public EventFixturesController()
        {
            _context = new ApplicationDbContext();
        }

        // GET: EventFixtures
        public ActionResult Index(short id)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var eventFixturesViewModel = new EventFixturesViewModel();

            var eventFixtures = _context.EventFixtures
                .Include(b => b.Fixture)
                .Include(b => b.Fixture.HomeTeam)
                .Include(b => b.Fixture.AwayTeam)
                .Include(b => b.Event)
                .Where(b => b.EventId == id)
                .ToList();

            var fixtures = _context.Fixtures
                .Include(b => b.HomeTeam)
                .Include(b => b.AwayTeam)
                .Include(b => b.League)
                .OrderBy(a => a.FixtureDateTime)
                .ToList();

            eventFixturesViewModel.Fixtures = fixtures;
            eventFixturesViewModel.EventFixtures = eventFixtures;
            eventFixturesViewModel.Event = Helper.Cache.GetCachedEvent(id);
            
            return View(eventFixturesViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(EventFixturesViewModel eventFixtureViewModel)
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var changesMade = false;

            var fixtures = _context.Fixtures.ToList();
            var eventFixtures = _context.EventFixtures.Where(a => a.EventId == eventFixtureViewModel.Event.Id).ToList();

            foreach (Fixture fixture in fixtures)
            {
                var myEventFixture = eventFixtures.FirstOrDefault(m => m.FixtureId == fixture.Id);

                var exists = Request["fixture_" + fixture.Id];
                if (exists == null && myEventFixture != null)
                {
                    // User has selected for this fixture NOT to be associated with this event,
                     
                    // delete any predictions there may be for this
                    var fixturePredictions = _context.FixturePredictions.Where(f => f.FixtureId == fixture.Id).ToList();
                    foreach (FixturePrediction fixturePrediction in fixturePredictions)
                    {
                        _context.FixturePredictions.Remove(fixturePrediction);
                    }
                    _context.EventFixtures.Remove(myEventFixture);
                    _context.SaveChanges();
                    changesMade = true;

                }
                else if (exists != null && myEventFixture == null)
                {
                    // User has selected to be in this event and the record does not exist
                    myEventFixture = new EventFixture()
                    {
                        EventId = eventFixtureViewModel.Event.Id,
                        FixtureId = fixture.Id,
                        CreatedDateTime = DateTime.UtcNow,
                        ModifiedDateTime = DateTime.UtcNow

                    };
                    _context.EventFixtures.Add(myEventFixture);
                    _context.SaveChanges();
                    changesMade = true;
                }
            }

            if (changesMade == true)
            {
                var eventParam = new SqlParameter("@intEventId", eventFixtureViewModel.Event.Id);
                _context.Database.ExecuteSqlCommand("EXEC spUpdateEventStartEnd @intEventId", eventParam);

                // update the application cache for events
                Helper.Cache.SetEventCache(eventFixtureViewModel.Event.Id);
            }

            return RedirectToAction("EventsIndex", "Events");

        }


    }
    }