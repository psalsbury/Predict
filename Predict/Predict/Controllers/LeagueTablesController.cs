using System.Web.Mvc;
using Predict.Helper;
using Predict.Models;
using Predict.ViewModels;

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

            var player = (Player) System.Web.HttpContext.Current.Session["Player"];
            
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
    }
}