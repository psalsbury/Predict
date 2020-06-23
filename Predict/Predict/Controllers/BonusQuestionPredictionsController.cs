using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Predict.Models;
using Predict.ViewModels;

namespace Predict.Controllers
{
    public class BonusQuestionPredictionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BonusQuestionPredictionsController()
        {
            _context = new ApplicationDbContext();
        }

        // GET: BonusQuestionPredictions
        public ActionResult BonusQuestionPredictions(short eventId)
        {
            var loggedInUserId = User.Identity.GetUserId();
            var bonusQuestionPredictionsViewModel =
                GetBonusQuestionPredictionsViewModel(loggedInUserId, loggedInUserId, eventId);
            bonusQuestionPredictionsViewModel.ReadOnly = false;
            return View(bonusQuestionPredictionsViewModel);
        }

        public BonusQuestionPredictionsViewModel GetBonusQuestionPredictionsViewModel(string loggedInUserId,
            string playerId, short eventId)
        {
            var bonusQuestionPredictionsViewModel = new BonusQuestionPredictionsViewModel();
            var player = (Player) System.Web.HttpContext.Current.Session["Player"];

            bonusQuestionPredictionsViewModel.PlayerId = playerId;

            var isPremiumPlayer = !(loggedInUserId != playerId && !player.PremiumPlayer);
            bonusQuestionPredictionsViewModel.IsPremiumPlayer = isPremiumPlayer;

            var bonusQuestions = _context.BonusQuestions.Where(e => e.EventId == eventId);

            // Get the existing bonus question predictions
            var bonusQuestionPredictions = _context.BonusQuestionPredictions
                .Include(b => b.BonusQuestion)
                .Where(p => p.PlayerId == playerId)
                .Where(p => p.BonusQuestion.EventId == eventId)
                .OrderBy(b => b.BonusQuestion.ToBeAnsweredByDateTime)
                .ToList();


            foreach (var bonusQuestion in bonusQuestions)
            {
                var bonusQuestionPrediction =
                    bonusQuestionPredictions.FirstOrDefault(f => f.BonusQuestionId == bonusQuestion.Id);
                if (bonusQuestionPrediction == null)
                {
                    bonusQuestionPrediction = new BonusQuestionPrediction
                    {
                        BonusQuestion = bonusQuestion,
                        CreatedDateTime = DateTime.Now,
                        ModifiedDateTime = DateTime.Now,
                        PlayerId = playerId,
                        BonusQuestionId = bonusQuestion.Id
                    };
                    bonusQuestionPredictions.Add(bonusQuestionPrediction);
                }
            }

            bonusQuestionPredictionsViewModel.BonusQuestionPredictions = bonusQuestionPredictions;
            return bonusQuestionPredictionsViewModel;
        }

        [HttpPost]
        public ActionResult Save(BonusQuestionPredictionsViewModel bonusQuestionPredictionsViewModel)
        {
            var eventId = bonusQuestionPredictionsViewModel.eventId;
            var loggedInUserId = User.Identity.GetUserId();

            var bonusQuestionPredictionsInDb = _context.BonusQuestionPredictions
                .Where(p => p.PlayerId == loggedInUserId)
                .Where(p => p.BonusQuestion.EventId == eventId)
                .OrderBy(b => b.BonusQuestion.ToBeAnsweredByDateTime)
                .ToList();

            foreach (var bonusQuestionPrediction in bonusQuestionPredictionsViewModel
                .BonusQuestionPredictions)
            {
                var bonusQuestionPredictionInDb =
                    bonusQuestionPredictionsInDb.FirstOrDefault(b => b.Id == bonusQuestionPrediction.Id);
                if (bonusQuestionPredictionInDb == null)
                {
                    // A new prediction
                    bonusQuestionPredictionInDb = new BonusQuestionPrediction
                    {
                        BonusQuestionId = bonusQuestionPrediction.BonusQuestionId, PlayerId = loggedInUserId,
                        PredictedAnswer = bonusQuestionPrediction.PredictedAnswer, CreatedDateTime = DateTime.Now,
                        ModifiedDateTime = DateTime.Now
                    };
                    _context.BonusQuestionPredictions.Add(bonusQuestionPredictionInDb);
                }
                else
                {
                    // An existing prediction
                    if (bonusQuestionPrediction.PredictedAnswer.IsNullOrWhiteSpace())
                    {
                        _context.BonusQuestionPredictions.Remove(bonusQuestionPredictionInDb);
                    }
                    else
                    {
                        bonusQuestionPredictionInDb.PredictedAnswer = bonusQuestionPrediction.PredictedAnswer;
                        bonusQuestionPredictionInDb.ModifiedDateTime = DateTime.Now;
                        _context.BonusQuestionPredictions.AddOrUpdate(bonusQuestionPredictionInDb);
                    }
                }
            }

            _context.SaveChanges();
            return RedirectToAction("Index", "Home");
        }
    }
}