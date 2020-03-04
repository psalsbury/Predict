using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Predict.Models;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Predict.ViewModels;

namespace Predict.Controllers
{
    public class PoolPlayersController : Controller
    {
        private ApplicationDbContext _context;

        public PoolPlayersController()
        {
            _context = new ApplicationDbContext();
        }

        // GET: PoolPlayers
        [System.Web.Mvc.Route("PoolAdmin/{poolId}")]
        public ActionResult PoolAdmin(int id)
        {
            var loggedInUserId = User.Identity.GetUserId();
            var isPoolAdmin = _context.Pools.Any(o => o.Id == id && o.AdminPlayerId == loggedInUserId);

            if (!isPoolAdmin)
            {
                throw new Exception("Only pool admin is allowed to edit the pool");
            }

            var poolPlayers = _context.PoolPlayers .Include(p => p.Player)
                                        .Where(p => p.PoolId == id).ToList();

            var pool = _context.Pools.SingleOrDefault(p => p.Id == id);
            var poolPlayerViewModel = new PoolPlayerViewModel
            {
                Pool = pool,
                PoolPlayers = poolPlayers
            };

            return View(poolPlayerViewModel);
        }
    }
}