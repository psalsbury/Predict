using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Predict.Models;
using Predict.ViewModels;

namespace Predict.Controllers
{
    public class ResultsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ResultsController()
        {
            _context = new ApplicationDbContext();
        }

        // GET: LeagueTableResults
        public ActionResult LeagueTableResults()
        {
            var leagueTablesViewModel = new Predict.ViewModels.LeagueTablesViewModel();
            var eventId = Helper.Cache.GetEventId();
            leagueTablesViewModel.LeagueTables = Predict.Helper.LeagueTableHelper.FetchLeagueTablesFromResults(eventId);
            leagueTablesViewModel.Results = true;
            return View("LeagueTables", leagueTablesViewModel);
        }

        // GET: Results
        public ActionResult GroupGameResults()
        {
            var eventId = Helper.Cache.GetEventId();
            var groupGameResultsViewModel = new GroupGameResultsViewModel();

            var fixtures = _context.Fixtures.Include(b => b.HomeTeam)
                .Include(b => b.AwayTeam)
                .Where(p => p.EventId == eventId).ToList()
                .OrderBy(p=> p.FixtureDateTime).ToList();

            groupGameResultsViewModel.Fixtures = fixtures;
            groupGameResultsViewModel.EventTeams = _context.EventTeams.Where(p => p.EventId == eventId).ToList(); ;

            return View(groupGameResultsViewModel);
        }

        // GET: Results
        public ActionResult KOResults()
        {
            var koFixtureController = new KoFixturesController();
            var koFixturePredictionViewModel = koFixtureController.GetKoFixturePredictionViewModel();
            koFixturePredictionViewModel.ReadOnly = true;

            return View("KoFixturePredictions", koFixturePredictionViewModel);
        }
    }
}