using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Predict.Models;
using Predict.ViewModels;
using System.Data.Entity;
using System.Data.Entity.Migrations;

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
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Save(KoFixturePredictionViewModel koFixturePredictionViewModel)
        {
            string userId = User.Identity.GetUserId();
            var eventId = Predict.Helper.Cache.GetEventId();

            if (Predict.Helper.Cache.IsInLockDown())
            {
                throw new Exception("Knock out predictions are not allowed to be changed after the competition has started");
            }

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
            int? winningTeamId = stringWinningTeamId == "" ? 0 : System.Convert.ToInt32(stringWinningTeamId);

            if (winningTeamId == 0 && koWinningTeamPrediction != null)
            {
                // remove the winning team row
                _context.KoWinningTeamPredictions.Remove(koWinningTeamPrediction);
            }
            else if(winningTeamId > 0 && koWinningTeamPrediction == null)
            {
                koWinningTeamPrediction = new KoWinningTeamPrediction
                {
                    CreatedDateTime = DateTime.Now,
                    ModifiedDateTime = DateTime.Now,
                    PlayerId = userId,
                    EventId = eventId,
                    TeamId = winningTeamId
                };
                _context.KoWinningTeamPredictions.Add(koWinningTeamPrediction);
            }
            else if (winningTeamId > 0 && koWinningTeamPrediction != null && koWinningTeamPrediction.TeamId != winningTeamId)
            {
                koWinningTeamPrediction.ModifiedDateTime = DateTime.Now;
                koWinningTeamPrediction.TeamId = winningTeamId;
                _context.KoWinningTeamPredictions.AddOrUpdate(koWinningTeamPrediction);
            }

            // Loop round the rounds to save the rest of the KO predictions
            while (round > 1)
            {

                for (int a = 1; a <= round/2; a++)
                {
                    var koFixture = koFixtures.Single(k => k.RoundOf == round && k.Position == a);
                    var stringTeam1Id = Request[round.ToString() + "_" + ((a * 2) - 1).ToString()];
                    var stringTeam2Id = Request[round.ToString() + "_" + (a * 2).ToString()];

                    int? team1Id = stringTeam1Id=="" ? 0 : System.Convert.ToInt32(stringTeam1Id);
                    int? team2Id = stringTeam2Id == "" ? 0 : System.Convert.ToInt32(stringTeam2Id);
                    if (team1Id == 0)
                        team1Id = null;

                    if (team2Id == 0)
                        team2Id = null;

                    var koFixturePrediction = koFixturePredictions.FirstOrDefault(k => k.KoFixtureId == koFixture.Id);
                    if (koFixturePrediction == null)
                    {
                        if (team1Id >0 || team2Id > 0)
                        {
                            _context.KoFixturePredictions.Add(ReturnNewKoFixturePrediction(team1Id,team2Id,koFixture.Id,userId));
                        }
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
                                koFixturePrediction.ModifiedDateTime = DateTime.Now;
                                koFixturePrediction.Team1Id = team1Id;
                                koFixturePrediction.Team2Id = team2Id;
                                _context.KoFixturePredictions.AddOrUpdate(koFixturePrediction);
                            }
                        }
                    }

                }
                round = round / 2;
               
            }

            _context.SaveChanges();
            Predict.Helper.SessionHelper.RefreshKoPredictions(Session, userId);
            return RedirectToAction("Index", "Home");
        }

        private static KoFixturePrediction ReturnNewKoFixturePrediction(int? team1Id, int? team2Id, int koFixtureId, string userId)
        {
            var koFixturePrediction = new KoFixturePrediction
            {
                CreatedDateTime = DateTime.Now,
                ModifiedDateTime = DateTime.Now,
                PlayerId = userId,
                Team1Id = null,
                Team2Id = null,
                KoFixtureId = koFixtureId
            };
            if (team1Id != null)
            {
                koFixturePrediction.Team1Id = team1Id;
            }
            if (team2Id != null)
            {
                koFixturePrediction.Team2Id = team2Id;
            }

            return koFixturePrediction;

        }

        // GET: KOFixturePredictions
        [System.Web.Mvc.Route("KoFixturePredictions/{userId}")]
        public ActionResult KoFixturePredictions(string userId)
        {
            var loggedInUserId = User.Identity.GetUserId();
            var koFixturePredictionViewModel = GetKoFixturePredictionViewModel(loggedInUserId, userId, false);

            return View(koFixturePredictionViewModel);
        }

        // GET: KOFixturePredictions
        [System.Web.Mvc.Route("KoFixturePredictions/{userId}")]
        public ActionResult KoFixturePredictionsGrouped(string userId)
        {
            var loggedInUserId = User.Identity.GetUserId();
            var koFixturePredictionViewModel = GetKoFixturePredictionViewModel(loggedInUserId, userId,false);

            return View(koFixturePredictionViewModel);
        }

        public KoFixturePredictionViewModel GetKoFixturePredictionViewModel(string loggedInUserId,string userId, bool readOnly)
        {
            
            var eventId = Predict.Helper.Cache.GetEventId();
            var isInLockDown = Predict.Helper.Cache.IsInLockDown();
            var player = (Player) System.Web.HttpContext.Current.Session["Player"];            

            if (userId == null)
                userId = loggedInUserId;

            if (loggedInUserId != userId && !player.PremiumPlayer)
            {
                throw new Exception("Only Premium Players are allowed to view other predictions");
            }

            var koFixturePredictionViewModel = new KoFixturePredictionViewModel
            {
                KoFixturePredictions = _context.KoFixturePredictions
                .Include(b => b.KoFixture)
                .Where(p => p.PlayerId == userId)
                .Where(p => p.KoFixture.EventId == eventId)
                .ToList(),
                Teams = (from a in _context.Teams
                         join c in _context.EventTeams on a.Id equals c.TeamId
                         where c.EventId == eventId
                         select a).ToList(),
                KoWinningTeam = _context.KoWinningTeamPredictions
                .Include(b => b.Team)
                .FirstOrDefault(e => e.EventId == eventId && e.PlayerId == userId)
            };

            if (!readOnly)
            {
                koFixturePredictionViewModel.FirstStageAutoFill =
                    Helper.LeagueTableHelper.FetchFirstRoundTeamsForAutoFill(eventId, userId);
                koFixturePredictionViewModel.RankedTeamsForAutoFill =
                    Helper.LeagueTableHelper.FetchAllTeamsInOrder(eventId, userId);
            }

            int maxRoundOf = _context.KoFixtures.Max(p => p.RoundOf);
            koFixturePredictionViewModel.MaxRows = (maxRoundOf * 2) - 1;

            int maxCols = 1;
            do
            {
                maxRoundOf = maxRoundOf / 2;
                maxCols++;
            } while (maxRoundOf > 1);
            koFixturePredictionViewModel.MaxCols = maxCols;

            if (loggedInUserId != userId | isInLockDown)
                koFixturePredictionViewModel.ReadOnly = true;

            return koFixturePredictionViewModel;
        }
    }
}