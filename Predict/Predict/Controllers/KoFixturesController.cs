using AutoMapper;
using Predict.Models;
using Predict.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;


namespace Predict.Controllers
{
    public class KoFixturesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KoFixturesController()
        {
            _context = new ApplicationDbContext();
        }

        // GET: KOFixtures
        public ActionResult Index(int id)
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            ViewBag.EventId = id;

            var fixtures = _context.KoFixtures.Where(p => p.EventId == id).ToList();
            return View(fixtures);
        }

        public ActionResult Save(KoFixtureViewModel koFixtureViewModel)
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            var koFixture = new KoFixture();
            if (koFixtureViewModel.Id != 0)
                koFixture = _context.KoFixtures.SingleOrDefault(f => f.Id == koFixtureViewModel.Id);

            Mapper.Map(koFixtureViewModel, koFixture);
            if (koFixtureViewModel.Id == 0)
            {
                koFixture.CreatedDateTime = DateTime.UtcNow;
                _context.KoFixtures.Add(koFixture);
            }

            koFixture.ModifiedDateTime = DateTime.UtcNow;
            _context.SaveChanges();

            return RedirectToAction("Index", "KoFixtures", new {Id = koFixture.EventId});
        }

        public ActionResult Create(short eventId)
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            // If not an admin of the site, then do not allow the creation of a fixture
            if (!User.IsInRole("Admin")) return RedirectToAction("Index", "Home");
            var koFixtureViewModel = PrepareViewModel(eventId);

            return View("EditKoFixture", koFixtureViewModel);
        }


        public ActionResult SaveKoResults(KoFixturePredictionViewModel koFixturePredictionViewModel)
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            var eventId = koFixturePredictionViewModel.EventId;

            var koFixtures = _context.KoFixtures
                .Where(f => f.EventId == eventId)
                .ToList();

            var round = (int)koFixtures.Max(a => a.RoundOf);

            var stringWinningTeamId = Request["1_1"];

            var koEvent = _context.EventKos.FirstOrDefault(f => f.EventId == eventId);
            if (koEvent != null)
            {
                int? winningTeamId = stringWinningTeamId == "" ? 0 : Convert.ToInt32(stringWinningTeamId);
                if (winningTeamId == 0)
                    koEvent.WinningTeamId = null;
                else
                    koEvent.WinningTeamId = winningTeamId;

                _context.EventKos.AddOrUpdate(koEvent);
            }


            while (round > 1)
            {
                for (var a = 1; a <= round / 2; a++)
                {
                    var koFixture = koFixtures.Single(k => k.RoundOf == round && k.Position == a);
                    var stringTeam1Id = Request[round + "_" + (a * 2 - 1)];
                    var stringTeam2Id = Request[round + "_" + a * 2];

                    int? team1Id = stringTeam1Id == "" ? 0 : Convert.ToInt32(stringTeam1Id);
                    int? team2Id = stringTeam2Id == "" ? 0 : Convert.ToInt32(stringTeam2Id);
                    if (team1Id == 0)
                        team1Id = null;

                    if (team2Id == 0)
                        team2Id = null;

                    koFixture.Team1Id = team1Id;
                    koFixture.Team2Id = team2Id;
                    koFixture.ModifiedDateTime = DateTime.UtcNow;

                    _context.KoFixtures.AddOrUpdate(koFixture);
                }

                round = round / 2;
            }

            _context.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult EditKoResults(short eventId)
        {
            // I WANT TO REUSE THE KO FIXTURE PREDICTION CONTROLLER, SO WILL SIMULATE PREDICTIONS
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            return View("KoFixturePredictions", GetKoFixturePredictionViewModel(eventId));
        }

        public KoFixturePredictionViewModel GetKoFixturePredictionViewModel(short eventId)
        {

            var koFixturePredictionViewModel = new KoFixturePredictionViewModel
            {
                FirstStageAutoFill = new Dictionary<int, int>(),
                RankedTeamsForAutoFill = new Dictionary<int, int>(),
                Predictions = false,
                EventId = eventId
            };

            var koFixturePredictions = new List<KoFixturePrediction>();
            var koFixtures = _context.KoFixtures.Where(f => f.EventId == eventId);
            var koEvent = _context.EventKos.FirstOrDefault(f => f.EventId == eventId);

            var leagueSubLeagueTeams = _context.LeagueSubLeagueTeams
                            .Include(p => p.Team)
                            .Where(a => a.LeagueSubLeague.LeagueId == koEvent.EventId).ToList();

            if (koEvent?.WinningTeamId != null)
            {
                var koWinningTeamPrediction = new KoWinningTeamPrediction();
                koFixturePredictionViewModel.KoWinningTeam = koWinningTeamPrediction;
                koWinningTeamPrediction.TeamId = koEvent.WinningTeamId;
            }

            foreach (var koFixture in koFixtures)
            {
                var koFixturePrediction = new KoFixturePrediction
                {
                    Team1Id = koFixture.Team1Id,
                    Team2Id = koFixture.Team2Id,
                    KoFixtureId = koFixture.Id,
                    KoFixture = koFixture
                };
                koFixturePredictions.Add(koFixturePrediction);
            }

            koFixturePredictionViewModel.KoFixturePredictions = koFixturePredictions;
            koFixturePredictionViewModel.MaxCols = 0;
            koFixturePredictionViewModel.MaxRows = 0;

            koFixturePredictionViewModel.LeagueSubLeagueTeams = leagueSubLeagueTeams;

            int maxRoundOf = koFixtures.Max(p => p.RoundOf);
            koFixturePredictionViewModel.MaxRows = maxRoundOf * 2 - 1;

            var maxCols = 1;
            do
            {
                maxRoundOf = maxRoundOf / 2;
                maxCols++;
            } while (maxRoundOf > 1);

            koFixturePredictionViewModel.MaxCols = maxCols;

            return koFixturePredictionViewModel;
        }

        public ActionResult EditKoFixture(int id, short eventId)
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            if (!User.IsInRole("Admin")) return RedirectToAction("Index", "Home");

            var koFixtureViewModel = PrepareViewModel(eventId);
            var koFixtureFromDb = _context.KoFixtures.SingleOrDefault(f => f.Id == id);

            Mapper.Map(koFixtureFromDb, koFixtureViewModel);

            return View("EditKoFixture", koFixtureViewModel);
        }

        public ActionResult DeleteKoFixture(int id)
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            var koFixture = _context.KoFixtures.SingleOrDefault(f => f.Id == id);
            if (!User.IsInRole("Admin") | (koFixture == null)) return RedirectToAction("Index", "Home");
            _context.KoFixtures.Remove(koFixture);
            _context.SaveChanges();

            return View("Index");
        }

        private KoFixtureViewModel PrepareViewModel(short eventId)
        {
            var eventsKo = _context.EventKos.Where(a => a.EventId == eventId).FirstOrDefault();

            var koFixtureViewModel = new KoFixtureViewModel
            {
                RoundOfs = new List<short>()
            };
            for (var power = 0; power <= 4; power++)
                koFixtureViewModel.RoundOfs.Add((short)Math.Pow(2, power));

            var leagueSubLeagues = _context.LeagueSubLeagues.Where(a => a.LeagueId == eventsKo.LinkedLeagueId).ToList();
            var leagueSubLeagueTeams = _context.LeagueSubLeagueTeams
                                            .Include(p => p.Team)
                                            .Where(a => a.LeagueSubLeague.LeagueId == eventsKo.LinkedLeagueId).ToList();

            leagueSubLeagues.Add(new LeagueSubLeague() { Id = 0, SubLeagueName = "Calculated" });
            koFixtureViewModel.LeagueSubLeagues = leagueSubLeagues;
            koFixtureViewModel.LeagueSubLeaguesTeams = leagueSubLeagueTeams;
            koFixtureViewModel.LinkedLeagueId = (short)eventsKo.LinkedLeagueId;
            koFixtureViewModel.EventId = eventId;

            return koFixtureViewModel;
        }
    }
}