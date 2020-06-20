using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using Predict.Models;
using Predict.ViewModels;
using Predict.Helper;

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
                LeagueTables = Helper.LeagueTableHelper.FetchLeagueTablesByUserId(eventId, userId)
            };
            var isPremiumPlayer = !(loggedInUserId != userId && !player.PremiumPlayer);
            leagueTablesViewModel.IsPremiumPlayer = isPremiumPlayer;
            return leagueTablesViewModel;
        }

    }
}