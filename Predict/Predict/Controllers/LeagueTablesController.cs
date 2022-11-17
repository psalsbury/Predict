using Predict.Helper;
using Predict.Models;
using Predict.ViewModels;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace Predict.Controllers
{
    public class LeagueTablesController : Controller
    {
        private ApplicationDbContext _context;

        public LeagueTablesController()
        {
            _context = new ApplicationDbContext();
        }

        public LeagueTablesViewModel GetLeagueTablesViewModel(string loggedInUserId, string userId, short eventId)
        {

            var player = (Player)System.Web.HttpContext.Current.Session["Player"];

            if (userId == null)
                userId = loggedInUserId;

            var leagueTablesViewModel = new LeagueTablesViewModel
            {
                LeagueTables = LeagueTableHelper.FetchLeagueTablesByUserId(eventId, userId)
            };
            var isPremiumPlayer = !(loggedInUserId != userId && !player.PremiumPlayer);
            leagueTablesViewModel.IsPremiumPlayer = isPremiumPlayer;
            return leagueTablesViewModel;
        }

        // GET: EventFixtures
        public ActionResult LeagueTables(short id)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            var userId = User.Identity.GetUserId();
            var myEvent = Helper.Cache.GetCachedEvent(id);

            var leagueTablesViewModel = GetLeagueTablesViewModel(userId,userId,id);
            ViewBag.EventId = id;
            ViewBag.International = myEvent.International;
            leagueTablesViewModel.OtherUserViewing = false;

            return View(leagueTablesViewModel);
        }
    }
}