using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Predict.Models;
using Predict.ViewModels;

namespace Predict.Controllers
{
    public class PredictionsConsolidatedController : Controller
    {
        private ApplicationDbContext _context;
        public PredictionsConsolidatedController()
        {
            _context = new ApplicationDbContext();
        }
        // GET: PredictionsConsolidated
        [Route("{playerId: string}")]
        public ActionResult ViewPredictions(string playerId)
        {
            var predictionsConsolidated = new PredictionsConsolidatedViewModel
            {
                PlayerId = playerId
            };

            predictionsConsolidated.Player = _context.Players.FirstOrDefault(p => p.Id == playerId);
            if (predictionsConsolidated.Player==null)
            {
                throw new Exception("Invalid Player");
            }

            var fixturePredictionsController = new FixturePredictionsController();
            var loggedInUserId = User.Identity.GetUserId();
            var fixturePredictionsViewModel = fixturePredictionsController.GetFixturePredictionsViewModel(loggedInUserId,playerId);
            fixturePredictionsViewModel.ReadOnly = true;

            var koFixturePredictionsController = new KoFixturePredictionsController();
            var kOFixturePredictionsViewModel = koFixturePredictionsController.GetKoFixturePredictionViewModel(loggedInUserId,playerId,true);
            kOFixturePredictionsViewModel.ReadOnly = true;

            var bonusQuestionPredictionsController = new BonusQuestionPredictionsController();
            var bonusQuestionPredictionsViewModel = bonusQuestionPredictionsController.GetBonusQuestionPredictionsViewModel(loggedInUserId, playerId);
            bonusQuestionPredictionsViewModel.ReadOnly = true;

            var leagueTablesController = new LeagueTablesController();
            var leagueTablesViewModel = leagueTablesController.GetLeagueTablesViewModel(loggedInUserId, playerId);
            leagueTablesViewModel.Results = false;

            predictionsConsolidated.KoFixturePredictionViewModel = kOFixturePredictionsViewModel;
            predictionsConsolidated.FixturePredictionsViewModel = fixturePredictionsViewModel;
            predictionsConsolidated.LeagueTablesViewModel = leagueTablesViewModel;
            predictionsConsolidated.BonusQuestionPredictionsViewModel = bonusQuestionPredictionsViewModel;
            return View(predictionsConsolidated);
        }
    }
}