using Microsoft.AspNet.Identity;
using Predict.Models;
using Predict.ViewModels;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Configuration;

namespace Predict.Controllers
{
    public class PoolPlayersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PoolPlayersController()
        {
            _context = new ApplicationDbContext();
        }

        // GET: EventPoolPlayers
        [Route("PoolAdmin/{poolId}")]
        public ActionResult PoolAdmin(int id)
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            var loggedInUserId = User.Identity.GetUserId();
            var isPoolAdmin = _context.Pools.Any(o => o.Id == id && o.AdminPlayerId == loggedInUserId);

            if (!isPoolAdmin) throw new Exception("Only pool admin is allowed to edit the pool");

            var poolPlayers = _context.PoolPlayers
                .Include(p => p.Player)
                .Include(u => u.Player.AspNetUser)
                .Where(p => p.PoolId == id && p.Enabled==true).ToList();

            var globalPoolId =
                Convert.ToInt32(ConfigurationManager.AppSettings["GlobalPoolId"]);

            var pool = _context.Pools.SingleOrDefault(p => p.Id == id);
            var poolPlayerViewModel = new PoolPlayerViewModel
            {
                Pool = pool,
                PoolPlayers = poolPlayers,
                GlobalPoolId = globalPoolId
            };

            return View(poolPlayerViewModel);
        }
    }
}