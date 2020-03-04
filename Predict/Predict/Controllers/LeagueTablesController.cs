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
        
        public LeagueTablesViewModel GetLeagueTablesViewModel(string loggedInUserId, string userId)
        {
            var eventId = Predict.Helper.Cache.GetEventId();
            var player = (Player)System.Web.HttpContext.Current.Session["Player"];

            if (userId == null)
                userId = loggedInUserId;

            if (loggedInUserId != userId && !player.PremiumPlayer)
            {
                throw new Exception("Only Premium Players are allowed to view other predictions");
            }

            var leagueTablesViewModel = new LeagueTablesViewModel
            {
                LeagueTables = Helper.LeagueTableHelper.FetchLeagueTablesByUserId(eventId, userId)
            };
            return leagueTablesViewModel;
        }

    }
}