using Microsoft.AspNet.Identity;
using Predict.Models;
using Predict.ViewModels;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Predict.Controllers
{
    public class PredictionsConsolidatedController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PredictionsConsolidatedController()
        {
            _context = new ApplicationDbContext();
        }

        // GET: PredictionsConsolidated
        public ActionResult ViewPredictions(string playerId, int poolId, short eventId)
        {

            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var predictionsConsolidated = new PredictionsConsolidatedViewModel
            {
                PlayerId = playerId
                , EventId = eventId
            };

            predictionsConsolidated.Player = _context.Players.FirstOrDefault(p => p.Id == playerId);
            if (predictionsConsolidated.Player == null) throw new Exception("Invalid Player");

            predictionsConsolidated.Pool = _context.Pools.FirstOrDefault(p => p.Id == poolId);
            if (predictionsConsolidated.Pool == null) throw new Exception("Invalid Pool");

            var nbrKoPredictionsToEnter = (int)Session["nbrKoFixtures*" + eventId];
            var nbrBonusQuestionsToEnter = (int)Session["nbrBonusQuestions*" + eventId];

            var fixturePredictionsController = new FixturePredictionsController();
            var loggedInUserId = User.Identity.GetUserId();
            var fixturePredictionsViewModel =
                fixturePredictionsController.GetFixturePredictionsViewModel(loggedInUserId, playerId, eventId);

            fixturePredictionsViewModel.OtherUserViewing = true;
            fixturePredictionsViewModel.Pool = predictionsConsolidated.Pool;
            predictionsConsolidated.FixturePredictionsViewModel = fixturePredictionsViewModel;

            if (nbrKoPredictionsToEnter > 0)
            {
                var koFixturePredictionsController = new KoFixturePredictionsController();
                var kOFixturePredictionsViewModel =
                    koFixturePredictionsController.GetKoFixturePredictionViewModel(loggedInUserId, playerId, true, eventId);
                kOFixturePredictionsViewModel.ReadOnly = true;
                predictionsConsolidated.KoFixturePredictionViewModel = kOFixturePredictionsViewModel;

                var leagueTablesController = new LeagueTablesController();
                var leagueTablesViewModel =
                    leagueTablesController.GetLeagueTablesViewModel(loggedInUserId, playerId, eventId);
                leagueTablesViewModel.Results = false;
                leagueTablesViewModel.OtherUserViewing = true;
                predictionsConsolidated.LeagueTablesViewModel = leagueTablesViewModel;
            }

            if (nbrBonusQuestionsToEnter > 0)
            {
                var bonusQuestionPredictionsController = new BonusQuestionPredictionsController();
                var bonusQuestionPredictionsViewModel =
                    bonusQuestionPredictionsController.GetBonusQuestionPredictionsViewModel(loggedInUserId, playerId, eventId);
                bonusQuestionPredictionsViewModel.ReadOnly = true;
                predictionsConsolidated.BonusQuestionPredictionsViewModel = bonusQuestionPredictionsViewModel;
            }

            return View(predictionsConsolidated);
        }
    }
}