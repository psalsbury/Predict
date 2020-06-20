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
        public ActionResult LeagueTableResults(int eventId)
        {
            var leagueTablesViewModel = new Predict.ViewModels.LeagueTablesViewModel();
            leagueTablesViewModel.LeagueTables = Predict.Helper.LeagueTableHelper.FetchLeagueTablesFromResults(eventId);
            leagueTablesViewModel.Results = true;
            return View("LeagueTables", leagueTablesViewModel);
        }

        // GET: Results
        public ActionResult GroupGameResults(int eventId)
        {
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
        public ActionResult KOResults(short eventId)
        {
            var koFixtureController = new KoFixturesController();
            var koFixturePredictionViewModel = koFixtureController.GetKoFixturePredictionViewModel(eventId);
            koFixturePredictionViewModel.ReadOnly = true;

            return View("KoFixturePredictions", koFixturePredictionViewModel);
        }
    }
}