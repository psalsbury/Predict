using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using Microsoft.Ajax.Utilities;
using Predict.Helper;
using Predict.Models;
using Predict.ViewModels;

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
        public ActionResult Index(int eventId)
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var fixtures = _context.KoFixtures.Where(p => p.EventId == eventId).ToList();
            return View(fixtures);
        }

        public ActionResult Save(KoFixtureViewModel koFixtureViewModel)
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var koFixture = new KoFixture();
            if (koFixtureViewModel.Id != 0)
                koFixture = _context.KoFixtures.SingleOrDefault(f => f.Id == koFixtureViewModel.Id);

            Mapper.Map(koFixtureViewModel, koFixture);
            if (koFixtureViewModel.Id == 0)
            {
                koFixture.CreatedDateTime = DateTime.Now;
                _context.KoFixtures.Add(koFixture);
            }

            koFixture.ModifiedDateTime = DateTime.Now;
            _context.SaveChanges();

            return RedirectToAction("Index", "KoFixtures");
        }

        public ActionResult Create(short eventId)
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            // If not an admin of the site, then do not allow the creation of a fixture
            if (!User.IsInRole("Admin")) return HttpNotFound();
            var koFixtureViewModel = PrepareViewModel(eventId);

            return View("EditKoFixture", koFixtureViewModel);
        }


        public ActionResult SaveKoResults(KoFixturePredictionViewModel koFixturePredictionViewModel)
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var eventId = koFixturePredictionViewModel.EventId;

            var koFixtures = _context.KoFixtures
                .Where(f => f.EventId == eventId)
                .ToList();

            var round = (int) koFixtures.Max(a => a.RoundOf);

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
                    koFixture.ModifiedDateTime = DateTime.Now;

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
                return RedirectToAction("Login", "Account");

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

            var eventTeams = Helper.LeagueTableHelper.GetEventTeams(_context, eventId);

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

            koFixturePredictionViewModel.EventTeams = eventTeams;

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
                return RedirectToAction("Login", "Account");

            if (!User.IsInRole("Admin")) return HttpNotFound();

            var koFixtureViewModel = PrepareViewModel(eventId);
            var koFixtureFromDb = _context.KoFixtures.SingleOrDefault(f => f.Id == id);

            Mapper.Map(koFixtureFromDb, koFixtureViewModel);

            return View("EditKoFixture", koFixtureViewModel);
        }

        public ActionResult DeleteKoFixture(int id)
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var koFixture = _context.KoFixtures.SingleOrDefault(f => f.Id == id);
            if (!User.IsInRole("Admin") | (koFixture == null)) return HttpNotFound();
            _context.KoFixtures.Remove(koFixture);
            _context.SaveChanges();

            return View("Index");
        }

        private KoFixtureViewModel PrepareViewModel(short eventId)
        {

            var koFixtureViewModel = new KoFixtureViewModel
            {
                RoundOfs = new List<short>(),
                EventTeams = Helper.LeagueTableHelper.GetEventTeams(_context,eventId)
                
            };
            for (var power = 0; power <= 4; power++)
                koFixtureViewModel.RoundOfs.Add((short) Math.Pow(2, power));

            var leagueNames = new List<string>();
            foreach (EventTeam eventTeam in koFixtureViewModel.EventTeams)
            {
                var leagueName = eventTeam.LeagueName;
                if (!leagueNames.Exists(a => a == leagueName))
                {
                    leagueNames.Add(leagueName);
                }
            }
            koFixtureViewModel.Leagues = leagueNames;
            koFixtureViewModel.EventId = eventId;

            return koFixtureViewModel;
        }
    }
}