using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Predict.Models;
using Predict.ViewModels;

namespace Predict.Controllers
{
    public class StatsKoController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly int _eventId;

        public StatsKoController()
        {
            _context = new ApplicationDbContext();
            _eventId = Helper.Cache.GetEventId();
        }


        // GET: StatsKO
        public ActionResult Index()
        {
            var statsKoViewModel = new StatsKoViewModel
            {

                StatsKoRoundOfs = _context.Database.SqlQuery<StatsKoRoundOf>("spGetStatsKo @intEventId"
                    , new SqlParameter("@intEventId", _eventId)
                ).ToList()
            };

            return View(statsKoViewModel);
        }
    }
}