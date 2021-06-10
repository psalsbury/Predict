using Microsoft.AspNet.Identity;
using Predict.Helper;
using Predict.Models;
using Predict.ViewModels;
using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Web.Mvc;

namespace Predict.Controllers
{
    public class KoFixturePredictionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KoFixturePredictionsController()
        {
            _context = new ApplicationDbContext();
        }

        public ActionResult ReadOnly(KoFixturePredictionViewModel koFixturePredictionViewModel)
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            return RedirectToAction("Index", "Home");
        }

        [ValidateAntiForgeryToken]
        public ActionResult Save(KoFixturePredictionViewModel koFixturePredictionViewModel)
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var userId = User.Identity.GetUserId();
            var eventId = koFixturePredictionViewModel.EventId;
            var KoPredictionsEntered = 0;

            if (Cache.HasEventStarted(eventId))
                throw new Exception(
                    "Knock out predictions are not allowed to be changed after the competition has started");

            var koFixturePredictions = _context.KoFixturePredictions
                .Where(p => p.PlayerId == userId)
                .Where(p => p.KoFixture.EventId == eventId)
                .ToList();

            var koFixtures = _context.KoFixtures
                .Where(f => f.EventId == eventId)
                .ToList();

            var koWinningTeamPrediction = _context.KoWinningTeamPredictions
                .Include(b => b.Team)
                .FirstOrDefault(e => e.EventId == eventId && e.PlayerId == userId);

            var round = (int)koFixtures.Max(a => a.RoundOf);

            // Save the winning team information
            var stringWinningTeamId = Request["1_1"];
            int? winningTeamId = stringWinningTeamId == "" ? 0 : Convert.ToInt32(stringWinningTeamId);

            if (winningTeamId == 0 && koWinningTeamPrediction != null)
            {
                // remove the winning team row
                _context.KoWinningTeamPredictions.Remove(koWinningTeamPrediction);
            }
            else if (winningTeamId > 0 && koWinningTeamPrediction == null)
            {
                koWinningTeamPrediction = new KoWinningTeamPrediction
                {
                    CreatedDateTime = DateTime.UtcNow,
                    ModifiedDateTime = DateTime.UtcNow,
                    PlayerId = userId,
                    EventId = eventId,
                    TeamId = winningTeamId
                };
                _context.KoWinningTeamPredictions.Add(koWinningTeamPrediction);
            }
            else if (winningTeamId > 0 && koWinningTeamPrediction != null &&
                     koWinningTeamPrediction.TeamId != winningTeamId)
            {
                koWinningTeamPrediction.ModifiedDateTime = DateTime.UtcNow;
                koWinningTeamPrediction.TeamId = winningTeamId;
                _context.KoWinningTeamPredictions.AddOrUpdate(koWinningTeamPrediction);
            }

            if(winningTeamId!=0)
                KoPredictionsEntered += 1;

            // Loop round the rounds to save the rest of the KO predictions
            while (round > 1)
            {
                for (var a = 1; a <= round / 2; a++)
                {
                    var koFixture = koFixtures.Single(k => k.RoundOf == round && k.Position == a);
                    var stringTeam1Id = Request[round + "_" + (a * 2 - 1)];
                    var stringTeam2Id = Request[round + "_" + a * 2];

                    int? team1Id = stringTeam1Id == "" ? 0 : Convert.ToInt32(stringTeam1Id);
                    int? team2Id = stringTeam2Id == "" ? 0 : Convert.ToInt32(stringTeam2Id);
                    if (team1Id == 0)
                    {
                        team1Id = null;
                    }
                    else
                    {
                        KoPredictionsEntered += 1;
                    }

                    if (team2Id == 0)
                    {
                        team2Id = null;
                    }
                    else
                    {
                        KoPredictionsEntered += 1;
                    }

                    var koFixturePrediction = koFixturePredictions.FirstOrDefault(k => k.KoFixtureId == koFixture.Id);
                    if (koFixturePrediction == null)
                    {
                        if (team1Id > 0 || team2Id > 0)
                            _context.KoFixturePredictions.Add(
                                ReturnNewKoFixturePrediction(team1Id, team2Id, koFixture.Id, userId));
                    }
                    else
                    {
                        if (team1Id == 0 && team2Id == 0)
                        {
                            _context.KoFixturePredictions.Remove(koFixturePrediction);
                        }
                        else
                        {
                            if (team1Id > 0 || team2Id > 0)
                            {
                                koFixturePrediction.ModifiedDateTime = DateTime.UtcNow;
                                koFixturePrediction.Team1Id = team1Id;
                                koFixturePrediction.Team2Id = team2Id;
                                _context.KoFixturePredictions.AddOrUpdate(koFixturePrediction);
                            }
                        }
                    }
                }

                round = round / 2;
            }

            var eventPoolPlayers = _context.EventPoolPlayers.Where(a => a.PlayerId == userId && a.EventId == eventId)
                .ToList();

            foreach (var eventPoolPlayer in eventPoolPlayers)
            {
                eventPoolPlayer.KoPredictionsEntered = KoPredictionsEntered;
                eventPoolPlayer.ModifiedDateTime = DateTime.UtcNow;
                _context.EventPoolPlayers.AddOrUpdate(eventPoolPlayer);
            }

            _context.SaveChanges();
            SessionHelper.RefreshKoPredictions(Session, userId, eventId);
            SessionHelper.RefreshWinningTeamPredictions(Session, userId, eventId);
            return RedirectToAction("Index", "Home", new { EventId = koFixturePredictionViewModel.EventId });
        }

        private static KoFixturePrediction ReturnNewKoFixturePrediction(int? team1Id, int? team2Id, int koFixtureId,
            string userId)
        {

            var koFixturePrediction = new KoFixturePrediction
            {
                CreatedDateTime = DateTime.UtcNow,
                ModifiedDateTime = DateTime.UtcNow,
                PlayerId = userId,
                Team1Id = null,
                Team2Id = null,
                KoFixtureId = koFixtureId
            };
            if (team1Id != null) koFixturePrediction.Team1Id = team1Id;
            if (team2Id != null) koFixturePrediction.Team2Id = team2Id;

            return koFixturePrediction;
        }

        // GET: KOFixturePredictions
        [Route("KoFixturePredictions/{userId}")]
        public ActionResult KoFixturePredictions(string userId, short eventId)
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var loggedInUserId = User.Identity.GetUserId();
            var koFixturePredictionViewModel = GetKoFixturePredictionViewModel(loggedInUserId, userId, false, eventId);

            return View(koFixturePredictionViewModel);
        }


        // GET: KOFixturePredictionsGrouped
        // [Route("KoFixturePredictionsGrouped/{eventId}")]
        public ActionResult KoFixturePredictionsGrouped(short eventId)
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var loggedInUserId = User.Identity.GetUserId();
            var koFixturePredictionViewModel = GetKoFixturePredictionViewModel(loggedInUserId, loggedInUserId, false, eventId);

            if (Cache.HasEventStarted(eventId))
            {
                koFixturePredictionViewModel.ReadOnly = true;
                return View("KoFixturePredictions",koFixturePredictionViewModel);
            }
            else
            {
                return View(koFixturePredictionViewModel);
            }

        }

        public KoFixturePredictionViewModel GetKoFixturePredictionViewModel(string loggedInUserId, string userId,
            bool readOnly, short eventId)
        {

            var isInLockDown = Cache.HasEventStarted(eventId);
            var player = (Player)System.Web.HttpContext.Current.Session["Player"];


            if (userId == null)
                userId = loggedInUserId;

            var koFixturePredictionViewModel = new KoFixturePredictionViewModel
            {
                EventId = eventId,
                KoFixturePredictions = _context.KoFixturePredictions
                    .Include(b => b.KoFixture)
                    .Include(b => b.Team1)
                    .Include(b => b.Team2)
                    .Where(p => p.PlayerId == userId)
                    .Where(p => p.KoFixture.EventId == eventId)
                    .ToList(),
                KoWinningTeam = _context.KoWinningTeamPredictions
                    .Include(b => b.Team)
                    .FirstOrDefault(e => e.EventId == eventId && e.PlayerId == userId)
            };
            koFixturePredictionViewModel.EventTeams = LeagueTableHelper.GetEventTeams(_context, eventId);

            var isPremiumPlayer = !(loggedInUserId != userId && !player.PremiumPlayer);
            koFixturePredictionViewModel.IsPremiumPlayer = isPremiumPlayer;

            if (!readOnly)
            {
                koFixturePredictionViewModel.FirstStageAutoFill =
                    LeagueTableHelper.FetchFirstRoundTeamsForAutoFill(eventId, userId);
                koFixturePredictionViewModel.RankedTeamsForAutoFill =
                    LeagueTableHelper.FetchAllTeamsInOrder(eventId, userId);
            }

            int maxRoundOf = _context.KoFixtures.Max(p => p.RoundOf);
            koFixturePredictionViewModel.MaxRows = maxRoundOf * 2 - 1;

            var maxCols = 1;
            do
            {
                maxRoundOf = maxRoundOf / 2;
                maxCols++;
            } while (maxRoundOf > 1);

            koFixturePredictionViewModel.MaxCols = maxCols;

            if ((loggedInUserId != userId) | isInLockDown)
                koFixturePredictionViewModel.ReadOnly = true;

            return koFixturePredictionViewModel;
        }
    }
}