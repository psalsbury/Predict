using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Predict.Models;
using System.Web.Mvc;

namespace Predict.Controllers
{
    public class EventGenerationsController : Controller
    {

        private readonly ApplicationDbContext _context;

        public EventGenerationsController()
        {
            _context = new ApplicationDbContext();
        }
        // GET: EventGenerations
        public ActionResult EventGenerationsIndex(short leagueEventGenerationId, short leagueId)
        {
            var eventGenerations = _context.EventGenerations
                            .Include(a => a.Event)
                            .Include(b => b.LeagueEventGeneration)
                            .Where(a => a.LeagueEventGenerationId == leagueEventGenerationId).ToList();

            ViewBag.LeagueEventGenerationId = leagueEventGenerationId;
            ViewBag.LeagueId = leagueId;
            return View(eventGenerations);
        }
    }
}