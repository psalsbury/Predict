using Microsoft.AspNet.Identity;
using Predict.Helper;
using Predict.Models;
using Predict.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web.Mvc;

namespace Predict.Controllers
{
    public class FixturePredictionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FixturePredictionsController()
        {
            _context = new ApplicationDbContext();
        }

        // GET: FixturePredictions
        // Used to enter predictions
        //[Route("FixturePredictions/FixturePredictions/{eventId}")]
        public ActionResult FixturePredictions(short eventId)
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var loggedInUserId = User.Identity.GetUserId();
            var fixturePredictionsViewModel = GetFixturePredictionsViewModel(loggedInUserId, loggedInUserId, eventId);
            return View(fixturePredictionsViewModel);
        }

        // GET: FixturePredictions
        // Used to view predictions
        [ActionName("ViewFixturePredictions")]
        public ActionResult FixturePredictions(short eventId, string userId)
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var loggedInUserId = User.Identity.GetUserId();
            var fixturePredictionsViewModel = GetFixturePredictionsViewModel(loggedInUserId, userId, eventId);
            return View("FixturePredictions", fixturePredictionsViewModel);
        }

        public FixturePredictionsViewModel GetFixturePredictionsViewModel(string loggedInUserId, string userId, short eventId)
        {

            var fixturePredictions = new List<FixturePrediction>();
            var fixturePredictionsViewModel = new FixturePredictionsViewModel();
            var player = (Player)System.Web.HttpContext.Current.Session["Player"];

            var myEvent = Helper.Cache.GetCachedEvent(eventId);

            if (userId == null)
                userId = loggedInUserId;

            fixturePredictionsViewModel.OtherUserViewing = (userId != loggedInUserId);
            fixturePredictionsViewModel.UserId = userId;
            fixturePredictionsViewModel.EventStartDateTime = myEvent.StartDateTime;
            fixturePredictionsViewModel.EventEndDateTime = myEvent.EndDateTime;

            var isPremiumPlayer = !(loggedInUserId != userId && !player.PremiumPlayer);
            fixturePredictionsViewModel.IsPremiumPlayer = isPremiumPlayer;

            if (!fixturePredictionsViewModel.OtherUserViewing)
            {

                fixturePredictionsViewModel.FreezeAllPredictions = _context.EventPoolPlayers.Include(a => a.Event)
                .Include(p => p.Pool)
                .Where(p => p.Event.StartDateTime <= DateTime.UtcNow)
                .Where(b => b.Event.EndDateTime >= DateTime.UtcNow)
                .Where((c => c.PlayerId == userId))
                .Where(d => d.EventId == eventId)
                .Any(a => a.Pool.FreezePredictions == true);
            }

            // Get the existing predictions
            var predictionsExist =
                _context.FixturePredictions.Any(p => p.PlayerId == userId && p.EventId == eventId);
            if (predictionsExist)
                fixturePredictions = _context.FixturePredictions
                    .Include(b => b.EventFixture)
                    .Include(b => b.EventFixture.Fixture)
                    .Include(b => b.EventFixture.Fixture.League)
                    .Include(b => b.EventFixture.Fixture.HomeTeam)
                    .Include(b => b.EventFixture.Fixture.AwayTeam)
                    .Where(p => p.PlayerId == userId)
                    .Where(p => p.EventId == eventId)
                    .ToList();

            // Get the list of fixtures
            var eventFixtures = _context.EventFixtures
                .Include(t => t.Fixture.HomeTeam)
                .Include(t => t.Fixture.AwayTeam)
                .Include(t => t.Fixture)
                .Include(t => t.Fixture.League)
                .Where(p => p.EventId == eventId)
                .OrderBy(b => b.Fixture.FixtureDateTime)
                .ToList();

            // generate all the missing ones
            foreach (var eventFixture in eventFixtures)
            {
                var fixturePrediction = fixturePredictions.FirstOrDefault(f => f.EventFixture.Fixture.Id == eventFixture.Fixture.Id);
                if (fixturePrediction == null)
                {
                    fixturePrediction = new FixturePrediction
                    {
                        EventFixture = eventFixture,
                        FixtureId = eventFixture.Fixture.Id,
                        PlayerId = userId
                    };
                    fixturePredictions.Add(fixturePrediction);
                }
            }

            // Get list of fixtures that have at least one result odds entry
            var listOfIds = (from m in eventFixtures where m.Fixture.RapidApiFixtureId != null select m.Fixture.RapidApiFixtureId);
            var listOfRapidApiFixturesWithResultOdds = (from m in _context.FixtureOddsByResults where listOfIds.Contains(m.RapidApiFixtureId) select m.RapidApiFixtureId).Distinct().ToList();
            var listOfRapidApiFixturesWithScoreOdds = (from m in _context.FixtureOddsByScores where listOfIds.Contains(m.RapidApiFixtureId) select m.RapidApiFixtureId).Distinct().ToList();

            fixturePredictionsViewModel.RapidApiFixtureIdsWithResultOdds = listOfRapidApiFixturesWithResultOdds;
            fixturePredictionsViewModel.RapidApiFixtureIdsWithScoreOdds = listOfRapidApiFixturesWithScoreOdds;

            // If there are any fixtures in the future, set the property so the lucky dip/clear buttons are available
            fixturePredictionsViewModel.AnyFixturesInTheFuture =
                fixturePredictions.Any(a => a.EventFixture.Fixture.FixtureDateTime > DateTime.UtcNow);

            fixturePredictionsViewModel.FixturePredictions =
                fixturePredictions.OrderBy(a => a.EventFixture.Fixture.FixtureDateTime).ToList();

            fixturePredictionsViewModel.UserId = userId;
            fixturePredictionsViewModel.EventId = eventId;
            fixturePredictionsViewModel.EventName = Helper.Cache.GetCachedEvent(eventId).EventName;

            return fixturePredictionsViewModel;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleError]
        public ActionResult Save(FixturePredictionsViewModel fixturePredictionsViewModel)
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var userId = User.Identity.GetUserId();
            var eventId = fixturePredictionsViewModel.EventId;
            var predictionChanged = true;

            // Get existing predictions and update or delete. Only get those where the dat has not yet passed
            var fixturePredictionsInDb = _context.FixturePredictions
                .Include(b => b.EventFixture)
                .Include(b => b.EventFixture.Fixture)
                .Include(b => b.EventFixture.Fixture.HomeTeam)
                .Include(b => b.EventFixture.Fixture.AwayTeam)
                .Where(p => p.PlayerId == userId)
                .Where(p => p.EventId == eventId)
                .ToList();

            var eventFixtures = _context.EventFixtures
                .Include(b => b.Fixture)
                .Where(f => f.EventId == eventId);

            // loop through the existing predictions from the database
            foreach (var fixturePredictionInDb in fixturePredictionsInDb)
            {
                // Find fixture in view model
                var fixturePredictionSubmitted =
                    fixturePredictionsViewModel.FixturePredictions.Find(m =>
                        m.FixtureId == fixturePredictionInDb.FixtureId);
                if (fixturePredictionSubmitted != null && !fixturePredictionInDb.EventFixture.Fixture.FixtureDatePassed)
                    if (fixturePredictionSubmitted.HomePrediction == null ||
                        fixturePredictionSubmitted.AwayPrediction == null)
                        _context.FixturePredictions.Remove(fixturePredictionInDb);
            }

            // Now loop through all those submitted, find the one from the db and update, or create a new one
            foreach (var fixturePredictionSubmitted in fixturePredictionsViewModel.FixturePredictions)
            {
                var eventFixture = eventFixtures.FirstOrDefault(f => f.Fixture.Id == fixturePredictionSubmitted.FixtureId);
                if (eventFixture != null && !eventFixture.Fixture.FixtureDatePassed)
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
                                EventId = eventId,
                                FixtureId = fixturePredictionSubmitted.FixtureId,
                                CreatedDateTime = DateTime.UtcNow,
                                HomePrediction = fixturePredictionSubmitted.HomePrediction,
                                AwayPrediction = fixturePredictionSubmitted.AwayPrediction,
                                ModifiedDateTime = DateTime.UtcNow
                            };
                        }
                        else
                        {
                            if (fixturePredictionSubmitted.HomePrediction == fixturePredictionInDb.HomePrediction &&
                                fixturePredictionSubmitted.AwayPrediction == fixturePredictionInDb.AwayPrediction)
                                predictionChanged = false;
                        }

                        if (predictionChanged)
                        {
                            fixturePredictionInDb.HomePrediction = fixturePredictionSubmitted.HomePrediction;
                            fixturePredictionInDb.AwayPrediction = fixturePredictionSubmitted.AwayPrediction;
                            fixturePredictionInDb.ModifiedDateTime = DateTime.UtcNow;
                            _context.FixturePredictions.AddOrUpdate(fixturePredictionInDb);
                        }
                    }
            }

            var predictionsEntered = fixturePredictionsViewModel.FixturePredictions.Count(a => a.AwayPrediction!=null && a.HomePrediction !=null);
            var eventPlayer = _context.EventPlayers.FirstOrDefault(a => a.PlayerId == userId && a.EventId == eventId);
            if (eventPlayer != null)
            {
                eventPlayer.FixturePredictionsEntered = predictionsEntered;
                eventPlayer.ModifiedDateTime = DateTime.UtcNow;
                _context.EventPlayers.AddOrUpdate(eventPlayer);
            }

            _context.SaveChanges();
            SessionHelper.RefreshFixturePredictions(Session, userId, eventId);

            return RedirectToAction("Index", "Home", new { EventId = fixturePredictionsViewModel.EventId });
        }
    }
}