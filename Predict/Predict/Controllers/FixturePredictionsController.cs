using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;using Microsoft.AspNet.Identity;
using Predict.Models;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using Predict.ViewModels;

namespace Predict.Controllers
{
    public class FixturePredictionsController : Controller
    {
        private ApplicationDbContext _context;
        public FixturePredictionsController()
        {
            _context = new ApplicationDbContext();
        }

        // GET: FixturePredictions
        // Used to enter predictions
        //[Route("FixturePredictions/FixturePredictions/{eventId}")]
        public ActionResult FixturePredictions(int eventId)
        {
            var loggedInUserId = User.Identity.GetUserId();
            var fixturePredictionsViewModel = GetFixturePredictionsViewModel(loggedInUserId, loggedInUserId, eventId);
            return View(fixturePredictionsViewModel);
        }

        // GET: FixturePredictions
        // Used to view predictions
        [ActionName("ViewFixturePredictions")]
        public ActionResult FixturePredictions(int eventId, string userId)
        {
            var loggedInUserId = User.Identity.GetUserId();
            var fixturePredictionsViewModel = GetFixturePredictionsViewModel(loggedInUserId, userId, eventId);
            return View("FixturePredictions",fixturePredictionsViewModel);
        }

        public FixturePredictionsViewModel GetFixturePredictionsViewModel(string loggedInUserId,string userId, int eventId)
        {
           
            var fixturePredictions = new List<FixturePrediction>();
            var fixturePredictionsViewModel = new FixturePredictionsViewModel();
            var player = (Player)System.Web.HttpContext.Current.Session["Player"];

            if (userId == null)
                userId = loggedInUserId;

            fixturePredictionsViewModel.UserId = userId;

            var isPremiumPlayer = !(loggedInUserId != userId && !player.PremiumPlayer);
            fixturePredictionsViewModel.IsPremiumPlayer = isPremiumPlayer;

            if (loggedInUserId != userId)
                fixturePredictionsViewModel.ReadOnly = true;

            // Get the existing predictions
            var predictionsExist = _context.FixturePredictions.Any(p => p.PlayerId == userId && p.Fixture.EventId == eventId);
            if (predictionsExist)
            {
                // Get the existing predictions
                fixturePredictions = _context.FixturePredictions
                                .Include(b => b.Fixture)
                                .Include(b => b.Fixture.HomeTeam)
                                .Include(b => b.Fixture.AwayTeam)
                                .Where(p => p.PlayerId == userId)
                                .Where(p => p.Fixture.EventId == eventId)
                                .OrderByDescending(b => b.Fixture.FixtureDateTime)
                                .ToList();
            }

            // Get the list of fixtures
            var fixtures = _context.Fixtures
                .Include(t => t.HomeTeam)
                .Include(t => t.AwayTeam)
                .Where(p => p.EventId == eventId)
                .ToList();

            var eventTeams = _context.EventTeams.Where(p => p.EventId == eventId).ToList();

            // generate all the missing ones
            foreach (var fixture in fixtures)
            {
                var fixturePrediction = fixturePredictions.FirstOrDefault(f => f.Fixture.Id == fixture.Id);
                if (fixturePrediction == null)
                {
                    fixturePrediction = new FixturePrediction
                    {
                        Fixture = fixture,
                        FixtureId = fixture.Id,
                        PlayerId = userId
                    };
                    fixturePredictions.Add(fixturePrediction);
                }
            }
            fixturePredictionsViewModel.FixturePredictions = fixturePredictions;
            fixturePredictionsViewModel.EventTeams = eventTeams;
            fixturePredictionsViewModel.UserId = userId;

            return fixturePredictionsViewModel;
        }

        [HttpPost]
        public ActionResult Save(FixturePredictionsViewModel fixturePredictionsViewModel)
        {

            string userId = User.Identity.GetUserId();
            var eventId = fixturePredictionsViewModel.EventId;
            bool predictionChanged = true;

            // Get existing predictions and update or delete
            var fixturePredictionsInDb = _context.FixturePredictions
                .Include(b => b.Fixture)
                .Include(b => b.Fixture.HomeTeam)
                .Include(b => b.Fixture.AwayTeam)
                .Where(p => p.PlayerId == userId)
                .Where(p => p.Fixture.EventId == eventId)
                .ToList();

            var fixtures = _context.Fixtures
                .Where(f => f.EventId == eventId);

            // loop through the existing predictions from the database
            foreach (FixturePrediction fixturePredictionInDb in fixturePredictionsInDb)
            {
                // Find fixture in view model
                var fixturePredictionSubmitted = fixturePredictionsViewModel.FixturePredictions.Find(m => m.FixtureId == fixturePredictionInDb.FixtureId);
                if (fixturePredictionSubmitted != null && !fixturePredictionInDb.Fixture.FixtureDatePassed)
                {
                    if (fixturePredictionSubmitted.HomePrediction == null ||
                        fixturePredictionSubmitted.AwayPrediction == null)
                    {
                        _context.FixturePredictions.Remove(fixturePredictionInDb);
                    }
                }
            }

            // Now loop through all those submitted, find the one from the db and update, or create a new one
            foreach (FixturePrediction fixturePredictionSubmitted in fixturePredictionsViewModel.FixturePredictions)
            {
                var fixture = fixtures.FirstOrDefault(f => f.Id == fixturePredictionSubmitted.FixtureId);
                if (fixture!=null && !fixture.FixtureDatePassed)
                {

                    if (fixturePredictionSubmitted.HomePrediction != null &&
                        fixturePredictionSubmitted.AwayPrediction != null)
                    {
                        predictionChanged = true;
                        var fixturePredictionInDb =
                            fixturePredictionsInDb.Find(m => m.FixtureId == fixturePredictionSubmitted.FixtureId);


                        if (fixturePredictionInDb == null)
                        {
                            fixturePredictionInDb = new FixturePrediction
                            {
                                PlayerId = userId,
                                FixtureId = fixturePredictionSubmitted.FixtureId,
                                CreatedDateTime = DateTime.Now,
                                HomePrediction = fixturePredictionSubmitted.HomePrediction,
                                AwayPrediction = fixturePredictionSubmitted.AwayPrediction,
                                ModifiedDateTime = DateTime.Now
                            };
                        }
                        else
                        {
                            
                            if (fixturePredictionSubmitted.HomePrediction == fixturePredictionInDb.HomePrediction &&
                                fixturePredictionSubmitted.AwayPrediction == fixturePredictionInDb.AwayPrediction)
                                predictionChanged = false;

                        }

                        if (predictionChanged == true)
                        {
                            fixturePredictionInDb.HomePrediction = fixturePredictionSubmitted.HomePrediction;
                            fixturePredictionInDb.AwayPrediction = fixturePredictionSubmitted.AwayPrediction;
                            fixturePredictionInDb.ModifiedDateTime = DateTime.Now;
                            _context.FixturePredictions.AddOrUpdate(fixturePredictionInDb);
                        }
                    }
                }
            }

            _context.SaveChanges();
            Predict.Helper.SessionHelper.RefreshFixturePredictions(Session, userId, eventId);

            return RedirectToAction("Index", "Home");

        }
    }
}