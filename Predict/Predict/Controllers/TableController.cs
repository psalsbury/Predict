using PagedList;
using Predict.Models;
using Predict.ViewModels;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace Predict.Controllers
{
    public class TableController : Controller
    {
        private const int PAGE_SIZE = 25;
        private readonly ApplicationDbContext _context;

        public TableController()
        {
            _context = new ApplicationDbContext();
        }

        [HttpGet]
        public ActionResult FindPlayer(short eventId, int poolId, string playerId)
        {
            var tableViewModels = GetTableViewModel(eventId, poolId);
            var position = tableViewModels.FindIndex(p => p.PlayerId == playerId);
            var pageNumber = position / PAGE_SIZE + 1;
            ViewBag.FindPlayer = playerId;

            return View("ShowTable", tableViewModels.ToPagedList(pageNumber, PAGE_SIZE));
        }

        [HttpGet]
        public ActionResult ShowTable(short eventId, int poolId, int? page)
        {
            var pageNumber = page ?? 1;
            var pageSize = PAGE_SIZE;

            var pool = _context.Pools.FirstOrDefault(p => p.Id == poolId);
            if (pool == null)
                return RedirectToAction("Index", "Home");

            ViewBag.PoolName = pool.PoolName;

            var tableViewModels = GetTableViewModel(eventId, poolId);

            var myEvent = Helper.Cache.GetCachedEvent(eventId);

            ViewBag.PoolId = poolId;
            ViewBag.EventId = eventId;
            ViewBag.PoolAdmin = pool.AdminPlayerId == User.Identity.GetUserId();

            ViewBag.nbrFixturePredictionsRequired = myEvent.Fixtures;
            ViewBag.nbrKOPredictionsRequired = myEvent.KoFixtures;
            ViewBag.nbrBonusPredictionsRequired = myEvent.BonusQuestions;

            return View(tableViewModels.ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        public ActionResult BestPools(short eventId, int? page)
        {
            var pageNumber = page ?? 1;
            var pageSize = PAGE_SIZE;

            ViewBag.PoolName = "Best Pools";
            var tableViewModels = GetBestPoolsTableViewModel(eventId);
            ViewBag.EventId = eventId;

            return View(tableViewModels.ToPagedList(pageNumber, pageSize));
        }

        private List<TableViewModel> GetTableViewModel(short eventId, int poolId)
        {
            var tableViewModels = _context.Database.SqlQuery<TableViewModel>(
                "spGetTable @intEventId, @intPoolId"
                , new SqlParameter("@intEventId", eventId)
                , new SqlParameter("@intPoolId", poolId)).ToList();
            return tableViewModels;
        }

        private List<BestPoolsTableViewModel> GetBestPoolsTableViewModel(short eventId)
        {
            var tableViewModels = _context.Database.SqlQuery<BestPoolsTableViewModel>(
                "spGetBestPoolsTable @intEventId"
                , new SqlParameter("@intEventId", eventId)).ToList();
            return tableViewModels;
        }
    }
}