using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using Predict.Models;
using Predict.ViewModels;

namespace Predict.Controllers
{
    public class FixturesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FixturesController()
        {
            _context = new ApplicationDbContext();
        }

        // GET: Fixtures
        public ActionResult Index(short eventId)
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var fixtures = _context.Fixtures.Include(b => b.HomeTeam)
                .Include(b => b.AwayTeam)
                .Where(p => p.EventId == eventId).ToList();

            ViewBag.EventId = eventId;

            return View(fixtures);
        }

        public ActionResult Create(short eventId)
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            // If not an admin of the site, then do not allow the creation of a fixture
            if (!User.IsInRole("Admin")) return HttpNotFound();

            var fixtureViewModel = new FixtureViewModel
            {
                Teams = (from a in _context.Teams
                    join c in _context.EventTeams on a.Id equals c.TeamId
                    where c.EventId == eventId
                    select a).ToList()
            };
            fixtureViewModel.EventId = eventId;

            return View("EditFixture", fixtureViewModel);
        }

        public ActionResult Edit(int id)
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            if (!User.IsInRole("Admin")) return HttpNotFound();

            var fixture = _context.Fixtures
                .Include(t => t.HomeTeam)
                .Include(t => t.AwayTeam)
                .SingleOrDefault(f => f.Id == id);

            var fixtureViewModel = new FixtureViewModel
            {
                Teams = (from a in _context.Teams
                    join c in _context.EventTeams on a.Id equals c.TeamId
                    where c.EventId == fixture.EventId
                    select a).ToList()
            };

            Mapper.Map(fixture, fixtureViewModel);
            return View("EditFixture", fixtureViewModel);
        }

        public ActionResult Save(FixtureViewModel fixtureViewModel)
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var fixture = new Fixture();
            if (fixtureViewModel.Id != 0) fixture = _context.Fixtures.SingleOrDefault(f => f.Id == fixtureViewModel.Id);

            Mapper.Map(fixtureViewModel, fixture);

            fixture.ModifiedDateTime = DateTime.Now;

            if (fixtureViewModel.Id == 0)
            {
                fixture.CreatedDateTime = DateTime.Now;
                _context.Fixtures.Add(fixture);
            }

            _context.SaveChanges();

            return RedirectToAction("Index", "Fixtures", new {eventId = fixture.EventId});
        }
    }
}