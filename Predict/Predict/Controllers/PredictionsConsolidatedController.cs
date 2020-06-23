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
        public ActionResult ViewPredictions(string playerId, int poolId)
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

            predictionsConsolidated.Pool = _context.Pools.FirstOrDefault(p => p.Id == poolId);
            if (predictionsConsolidated.Pool == null)
            {
                throw new Exception("Invalid Pool");
            }

            var nbrKoPredictionsToEnter = (int) Session["nbrKoFixtures*" + predictionsConsolidated.Pool.EventId];
            var nbrBonusQuestionsToEnter = (int)Session["nbrBonusQuestions*" + predictionsConsolidated.Pool.EventId]; ;

            var fixturePredictionsController = new FixturePredictionsController();
            var loggedInUserId = User.Identity.GetUserId();
            var fixturePredictionsViewModel = fixturePredictionsController.GetFixturePredictionsViewModel(loggedInUserId,playerId, predictionsConsolidated.Pool.EventId);
            fixturePredictionsViewModel.ReadOnly = true;
            predictionsConsolidated.FixturePredictionsViewModel = fixturePredictionsViewModel;

            if (nbrKoPredictionsToEnter > 0)
            {
                var koFixturePredictionsController = new KoFixturePredictionsController();
                var kOFixturePredictionsViewModel = koFixturePredictionsController.GetKoFixturePredictionViewModel(loggedInUserId, playerId, true, predictionsConsolidated.Pool.EventId);
                kOFixturePredictionsViewModel.ReadOnly = true;
                predictionsConsolidated.KoFixturePredictionViewModel = kOFixturePredictionsViewModel;

                var leagueTablesController = new LeagueTablesController();
                var leagueTablesViewModel = leagueTablesController.GetLeagueTablesViewModel(loggedInUserId, playerId, predictionsConsolidated.Pool.EventId);
                leagueTablesViewModel.Results = false;
                predictionsConsolidated.LeagueTablesViewModel = leagueTablesViewModel;
            }


            if (nbrBonusQuestionsToEnter > 0)
            {
                var bonusQuestionPredictionsController = new BonusQuestionPredictionsController();
                var bonusQuestionPredictionsViewModel = bonusQuestionPredictionsController.GetBonusQuestionPredictionsViewModel(loggedInUserId, playerId, predictionsConsolidated.Pool.EventId);
                bonusQuestionPredictionsViewModel.ReadOnly = true;
                predictionsConsolidated.BonusQuestionPredictionsViewModel = bonusQuestionPredictionsViewModel;
            }

            return View(predictionsConsolidated);
        }
    }
}