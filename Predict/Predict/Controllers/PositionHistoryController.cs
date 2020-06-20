using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Predict.Models;

namespace Predict.Controllers
{
    public class PositionHistoryController : Controller
    {

        private ApplicationDbContext _context;

        public PositionHistoryController()
        {
            _context = new ApplicationDbContext();
        }
        // GET: PosnHistory
        public ActionResult Index(string playerId, int poolId)
        {
            var positionHistory = (from a in _context.PoolPlayerPositionHistory
                join c in _context.Pools on a.PoolId equals c.Id
                where a.PlayerId == playerId
                      && c.Id == poolId
                           select a).ToList();
            return View();
        }


    }
}