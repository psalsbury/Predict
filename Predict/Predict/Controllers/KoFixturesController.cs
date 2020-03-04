using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
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
        public ActionResult Index()
        {
            
            var eventId = Helper.Cache.GetEventId();
            var fixtures = _context.KoFixtures.Where(p => p.EventId == eventId).ToList();

            return View(fixtures);
        }

        public ActionResult Save(KoFixtureViewModel koFixtureViewModel)
        {
            var koFixture = new KoFixture();
            if (koFixtureViewModel.Id != 0)
            {
                koFixture = _context.KoFixtures.SingleOrDefault(f => f.Id == koFixtureViewModel.Id);
            }
 
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

        public ActionResult Create()
        {
            // If not an admin of the site, then do not allow the creation of a fixture
            if (!User.IsInRole("Admin"))
            {
                return HttpNotFound();
            }
            var koFixtureViewModel = PrepareViewModel();
            koFixtureViewModel.EventId = Predict.Helper.Cache.GetEventId();

            return View("EditKoFixture", koFixtureViewModel);
        }


        public ActionResult SaveKoResults(KoFixturePredictionViewModel koFixturePredictionViewModel)
        {

            var eventId = Predict.Helper.Cache.GetEventId();

            var koFixtures = _context.KoFixtures
                .Where(f => f.EventId == eventId)
                .ToList();

            var round = (int)koFixtures.Max(a => a.RoundOf);

            var stringWinningTeamId = Request["1_1"];

            var koEvent = _context.EventKos.FirstOrDefault(f => f.EventId == eventId);
            if (koEvent != null)
            {
                int? winningTeamId = stringWinningTeamId == "" ? 0 : System.Convert.ToInt32(stringWinningTeamId);
                if (winningTeamId == 0)
                {
                    koEvent.WinningTeamId = null;
                }
                else
                {
                    koEvent.WinningTeamId = winningTeamId;
                }
                
                _context.EventKos.AddOrUpdate(koEvent);
            }


            while (round > 1)
            {

                for (int a = 1; a <= round / 2; a++)
                {
                    var koFixture = koFixtures.Single(k => k.RoundOf == round && k.Position == a);
                    var stringTeam1Id = Request[round.ToString() + "_" + ((a * 2) - 1).ToString()];
                    var stringTeam2Id = Request[round.ToString() + "_" + (a * 2).ToString()];

                    int? team1Id = stringTeam1Id == "" ? 0 : System.Convert.ToInt32(stringTeam1Id);
                    int? team2Id = stringTeam2Id == "" ? 0 : System.Convert.ToInt32(stringTeam2Id);
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

        public ActionResult EditKoResults()
        {
            // I WANT TO REUSE THE KO FIXTURE PREDICTION CONTROLLER, SO WILL SIMULATE PREDICTIONS
        
         return View("KoFixturePredictions", GetKoFixturePredictionViewModel());
       
        }

        public KoFixturePredictionViewModel GetKoFixturePredictionViewModel()
        {
            var eventId = Predict.Helper.Cache.GetEventId();

            var koFixturePredictionViewModel = new KoFixturePredictionViewModel
            {
                FirstStageAutoFill = new Dictionary<int, int>()
                ,
                RankedTeamsForAutoFill = new Dictionary<int, int>()
                ,
                Predictions = false
            };

            var koFixturePredictions = new List<KoFixturePrediction>();
            var koFixtures = _context.KoFixtures.Where(f => f.EventId == eventId);
            var koEvent = _context.EventKos.FirstOrDefault(f => f.EventId == eventId);

            var teams = (from a in _context.Teams
                         join c in _context.EventTeams on a.Id equals c.TeamId
                         where c.EventId == eventId
                         select a).ToList();

            if (koEvent?.WinningTeamId != null)
            {
                var koWinningTeamPrediction = new KoWinningTeamPrediction();
                koFixturePredictionViewModel.KoWinningTeam = koWinningTeamPrediction; koWinningTeamPrediction.TeamId = koEvent.WinningTeamId;
            }

            foreach (var koFixture in koFixtures)
            {
                var koFixturePrediction = new KoFixturePrediction
                {
                    Team1Id = koFixture.Team1Id
                    ,
                    Team2Id = koFixture.Team2Id
                    ,
                    KoFixtureId = koFixture.Id
                    ,
                    KoFixture = koFixture
                };
                koFixturePredictions.Add(koFixturePrediction);
            }

            koFixturePredictionViewModel.KoFixturePredictions = koFixturePredictions;
            koFixturePredictionViewModel.MaxCols = 0;
            koFixturePredictionViewModel.MaxRows = 0;

            koFixturePredictionViewModel.Teams = teams;

            int maxRoundOf = koFixtures.Max(p => p.RoundOf);
            koFixturePredictionViewModel.MaxRows = (maxRoundOf * 2) - 1;

            int maxCols = 1;
            do
            {
                maxRoundOf = maxRoundOf / 2;
                maxCols++;
            } while (maxRoundOf > 1);
            koFixturePredictionViewModel.MaxCols = maxCols;

            return koFixturePredictionViewModel;
        }

        public ActionResult EditKoFixture(int id)
        {

            if (!User.IsInRole("Admin"))
            {
                return HttpNotFound();
            }

            var koFixtureViewModel = PrepareViewModel();
            var koFixtureFromDb = _context.KoFixtures.SingleOrDefault(f => f.Id == id);

            Mapper.Map(koFixtureFromDb, koFixtureViewModel);

            return View("EditKoFixture", koFixtureViewModel);
        }

        public ActionResult DeleteKoFixture(int id)
        {

            var koFixture = _context.KoFixtures.SingleOrDefault(f => f.Id == id);
            if (!User.IsInRole("Admin") | koFixture == null)
            {
                return HttpNotFound();
            }
            _context.KoFixtures.Remove(koFixture);
            _context.SaveChanges();
            
            return View("Index");
        }

        private KoFixtureViewModel PrepareViewModel()
        {
            var eventId = Predict.Helper.Cache.GetEventId();
            var koFixtureViewModel = new KoFixtureViewModel
            {
                Teams = (from a in _context.Teams
                    join c in _context.EventTeams on a.Id equals c.TeamId
                    where c.EventId == eventId
                    select a).ToList(),
                RoundOfs = new List<short>()

            };
            for (int power = 0; power <= 4; power++)
                koFixtureViewModel.RoundOfs.Add((short)Math.Pow(2, power));

            koFixtureViewModel.Leagues = _context.EventTeams.Select(m => m.League).Distinct().ToList();

            return koFixtureViewModel;
        }


    }
}