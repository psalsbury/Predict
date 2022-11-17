using Microsoft.AspNet.Identity;
using Predict.Models;
using Predict.ViewModels;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Configuration;

namespace Predict.Controllers
{
    public class PredictionsConsolidatedController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PredictionsConsolidatedController()
        {
            _context = new ApplicationDbContext();
        }

        public ActionResult ViewPredictionSummary(short eventId)
        {

            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            var playerId = User.Identity.GetUserId();
            var globalPoolId = (System.Convert.ToInt32(ConfigurationManager.AppSettings["GlobalPoolId"]));

            ViewBag.Summary = "true";
            return View("ViewPredictions",GetViewModel(playerId, globalPoolId, eventId));
        }

        // GET: PredictionsConsolidated
        public ActionResult ViewPredictions(string playerId, int poolId, short eventId)
        {

            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            ViewBag.Summary = "false";

            return View(GetViewModel(playerId,poolId,eventId));
        }

        private PredictionsConsolidatedViewModel GetViewModel(string playerId, int poolId, short eventId)
        {
            var currentPlayerId = User.Identity.GetUserId();
            var otherUser = true;
            if (currentPlayerId == playerId)
                otherUser = false;

            var predictionsConsolidated = new PredictionsConsolidatedViewModel
            {
                PlayerId = playerId
                ,
                EventId = eventId
                ,
                PoolId = poolId
            };

            predictionsConsolidated.Player = _context.Players.FirstOrDefault(p => p.Id == playerId);
            if (predictionsConsolidated.Player == null) throw new Exception("Invalid Player");

            var nbrKoPredictionsToEnter = (int)Session["nbrKoFixtures*" + eventId];
            var nbrBonusQuestionsToEnter = (int)Session["nbrBonusQuestions*" + eventId];

            var fixturePredictionsController = new FixturePredictionsController();
            var loggedInUserId = User.Identity.GetUserId();
            var fixturePredictionsViewModel =
                fixturePredictionsController.GetFixturePredictionsViewModel(loggedInUserId, playerId, eventId);

            fixturePredictionsViewModel.OtherUserViewing = otherUser;
            fixturePredictionsViewModel.ReadOnly = true;
            predictionsConsolidated.FixturePredictionsViewModel = fixturePredictionsViewModel;

            fixturePredictionsViewModel.Pool = _context.Pools.FirstOrDefault(p => p.Id == poolId);
            if (fixturePredictionsViewModel.Pool == null) throw new Exception("Invalid Pool");

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
                leagueTablesViewModel.OtherUserViewing = otherUser;
                predictionsConsolidated.LeagueTablesViewModel = leagueTablesViewModel;
            }

            if (nbrBonusQuestionsToEnter > 0)
            {
                var bonusQuestionPredictionsController = new BonusQuestionPredictionsController();
                var bonusQuestionPredictionsViewModel =
                    bonusQuestionPredictionsController.GetBonusQuestionPredictionsViewModel(loggedInUserId, playerId, eventId);
                bonusQuestionPredictionsViewModel.ReadOnly = true; ;
                bonusQuestionPredictionsViewModel.OtherUserViewing = otherUser;
                predictionsConsolidated.BonusQuestionPredictionsViewModel = bonusQuestionPredictionsViewModel;
            }

            return predictionsConsolidated;

        }
    }
}