using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web.Mvc;
using Predict.Models;
using Predict.ViewModels;
using System.Collections.Generic;

namespace Predict.Controllers
{
    public class LeagueEventGenerationsController : Controller
    {

        private readonly ApplicationDbContext _context;

        public LeagueEventGenerationsController()
        {
            _context = new ApplicationDbContext();
        }
        // GET: LeagueEventGenerations
        [HttpGet]
        public ActionResult LeagueEventGenerationsIndex(short id)
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            if (!User.IsInRole("Admin")) return RedirectToAction("Index", "Home");

            var leagueEventGenerationsViewModel = new LeagueEventGenerationsViewModel
            {
                LeagueEventGenerations = _context.LeagueEventGenerations
                                            .Include(a => a.League)
                                            .Where(a => a.LeagueId == id).ToList()
                , League = _context.Leagues.Where(a => a.Id == id).FirstOrDefault()
            };

            var rapidApiV3LeagueSeasonIdArray = leagueEventGenerationsViewModel.LeagueEventGenerations.Where(a => a.League.RapidApiV3LeagueSeasonId != null).Select(a => a.League.RapidApiV3LeagueSeasonId).ToArray();

            var teamsArray = leagueEventGenerationsViewModel.LeagueEventGenerations.Where(a => a.TeamId != null).Select(a => a.TeamId).ToArray();

            leagueEventGenerationsViewModel.RapidApiV3LeagueSeasons = _context.RapidApiV3LeagueSeasons
                .Include(q => q.RapidApiV3League)
                .Include(c => c.RapidApiV3League.RapidApiV3Country)
                .Where(a => rapidApiV3LeagueSeasonIdArray.Contains(a.Id))
                .ToList();

            leagueEventGenerationsViewModel.Teams = _context.Teams.Where(s => teamsArray.Contains(s.Id)).ToList();

            return View(leagueEventGenerationsViewModel);
        }

        [HttpGet]
        public ActionResult EditLeagueEventGeneration(short id, short leagueId)
        {

            var leagueEventGenerationsEditViewModel = new LeagueEventGenerationsEditViewModel()
            {
                LeagueEventGeneration = _context.LeagueEventGenerations
                            .Include(a => a.League)
                            .Where(a => a.Id == id).FirstOrDefault()
                ,
                Teams = _context.Fixtures.Where(a => a.LeagueId == leagueId).Select(a => a.HomeTeam).Distinct().OrderBy(a => a.TeamName).ToList()
            };

            if (leagueEventGenerationsEditViewModel.LeagueEventGeneration == null)
            {
                var leagueEventGeneration = new LeagueEventGeneration
                {
                    LeagueId = leagueId
                    , League = _context.Leagues.Where(a => a.Id == leagueId).FirstOrDefault()
                };
                leagueEventGenerationsEditViewModel.LeagueEventGeneration = leagueEventGeneration;
            }

            leagueEventGenerationsEditViewModel.GenerationTypes = getGenerationTypesForDropDown();

            return View(leagueEventGenerationsEditViewModel);
        }

        private List<GenerationType> getGenerationTypesForDropDown()
        {
            return new List<GenerationType>
            {
                 new GenerationType { Id=1, GenerationTypeName="Weekly"}
                ,new GenerationType { Id=2, GenerationTypeName="2 a Month"}
                ,new GenerationType { Id=3, GenerationTypeName="Monthly"}
                ,new GenerationType { Id=11,GenerationTypeName="League Season"}
                ,new GenerationType { Id=21,GenerationTypeName="Team Season"}
            };
        }

        [HttpPost]
        public ActionResult EditLeagueEventGeneration(LeagueEventGenerationsEditViewModel leagueEventGenerationsEditViewModel)
        {
            if(leagueEventGenerationsEditViewModel.LeagueEventGeneration.Id==0)
            {
                leagueEventGenerationsEditViewModel.LeagueEventGeneration.CreatedDateTime = DateTime.UtcNow;
            }
            leagueEventGenerationsEditViewModel.LeagueEventGeneration.ModifiedDateTime = DateTime.UtcNow;

            if (!ModelState.IsValid)
            {
                leagueEventGenerationsEditViewModel.GenerationTypes = getGenerationTypesForDropDown();
                leagueEventGenerationsEditViewModel.Teams = _context.Fixtures.Where(a => a.LeagueId == leagueEventGenerationsEditViewModel.LeagueEventGeneration.LeagueId).Select(a => a.HomeTeam).Distinct().OrderBy(a => a.TeamName).ToList();
                return View(leagueEventGenerationsEditViewModel);
            }
            leagueEventGenerationsEditViewModel.LeagueEventGeneration.League = null; // Remove included League as dont want it saving as well
            _context.LeagueEventGenerations.AddOrUpdate(leagueEventGenerationsEditViewModel.LeagueEventGeneration);
            _context.SaveChanges();

            return RedirectToAction("LeagueEventGenerationsIndex", new { id = leagueEventGenerationsEditViewModel.LeagueEventGeneration.LeagueId });
        }
    }
}