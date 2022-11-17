using AutoMapper;
using Predict.Models;
using Predict.ViewModels;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace Predict.Controllers
{
    public class FixturesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FixturesController()
        {
            _context = new ApplicationDbContext();
        }

        [HttpGet ]
        // GET: Fixtures
        public ActionResult Index(short leagueId)
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            var fixtures = _context.Fixtures
                .Include(b => b.HomeTeam)
                .Include(b => b.AwayTeam)
                .Where(a => a.LeagueId == leagueId)
                .OrderBy(a => a.FixtureDateTime)
                .ToList();

            ViewBag.LeagueId = leagueId;
            return View(fixtures);
        }

        [HttpGet]
        public ActionResult Create(short leagueId)
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            // If not an admin of the site, then do not allow the creation of a fixture
            if (!User.IsInRole("Admin")) return RedirectToAction("Index", "Home");

            var fixtureViewModel = new FixtureViewModel
            {
                Teams = (from a in _context.Teams
                         select a).ToList()
            };
            fixtureViewModel.LeagueId = leagueId;

            return View("EditFixture", fixtureViewModel);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            if (!User.IsInRole("Admin")) return RedirectToAction("Index", "Home");

            var fixture = _context.Fixtures
                .Include(t => t.HomeTeam)
                .Include(t => t.AwayTeam)
                .SingleOrDefault(f => f.Id == id);

            if(fixture==null)
            {
                return RedirectToAction("Index", "Home");
            }

            var fixtureViewModel = new FixtureViewModel
            {
                Teams = (from a in _context.Teams
                         select a).ToList()
            };

            Mapper.Map(fixture, fixtureViewModel);
            return View("EditFixture", fixtureViewModel);
        }
        [HttpPost]
        public ActionResult UpdateFixturesFromApi()
        {
            var leagueId = System.Convert.ToInt16(Request["leagueId"]);
            var league = _context.Leagues
                .Where(a => a.Id == leagueId).FirstOrDefault();

            var rapidApiV3LeagueSeason = _context.RapidApiV3LeagueSeasons.Where(a => a.Id == league.RapidApiV3LeagueSeasonId).FirstOrDefault();
            var earliestTime = DateTime.UtcNow;

            RapidApi.RapidApiHelper.V3FixturesByLeague(rapidApiV3LeagueSeason, earliestTime, leagueId);
            return RedirectToAction("Index", "Fixtures", new { @leagueId = leagueId });

        }

        [HttpPost]
        public ActionResult Save(FixtureViewModel fixtureViewModel)
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            var fixture = new Fixture();
            if (fixtureViewModel.Id != 0) fixture = _context.Fixtures.SingleOrDefault(f => f.Id == fixtureViewModel.Id);

            Mapper.Map(fixtureViewModel, fixture);

            fixture.ModifiedDateTime = DateTime.UtcNow;
            fixture.ResultProcessed = false;

            if (fixtureViewModel.Id == 0)
            {
                fixture.CreatedDateTime = DateTime.UtcNow;
                _context.Fixtures.Add(fixture);
            }

            _context.SaveChanges();
            return RedirectToAction("Index", "Fixtures", new { @leagueId = fixture.LeagueId });
        }

        [HttpPost]
        public ActionResult Delete(FixtureViewModel fixtureViewModel)
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            var fixture = _context.Fixtures.FirstOrDefault(a => a.Id== fixtureViewModel.Id);
            if(fixture==null)
            {
                throw new InvalidOperationException("fixture does not exist");
            }

            var events = _context.EventFixtures.Where(a => a.FixtureId == fixture.Id).Select(a => a.EventId).ToList();
            _context.FixturePredictions.RemoveRange(_context.FixturePredictions.Where(a => a.FixtureId == fixture.Id));
            _context.EventFixtures.RemoveRange(_context.EventFixtures.Where(a => a.FixtureId == fixture.Id));
            _context.FixtureOddsByResults.RemoveRange(_context.FixtureOddsByResults.Where(a => a.RapidApiFixtureId == fixture.RapidApiFixtureId));
            _context.FixtureOddsByScores.RemoveRange(_context.FixtureOddsByScores.Where(a => a.RapidApiFixtureId == fixture.RapidApiFixtureId));
            _context.Fixtures.Remove(fixture);
            _context.SaveChanges();

            foreach (var eventId in events)
            {
                Helper.Cache.UpdateEventStartEnd(_context, eventId);
            }

            return RedirectToAction("Index", "Fixtures", new { @leagueId = fixtureViewModel.LeagueId });
        }

    }
}