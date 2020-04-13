using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Predict.Models;
using Predict.ViewModels;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.Ajax.Utilities;
using PagedList;

namespace Predict.Controllers
{
    public class TableController : Controller
    {
        private ApplicationDbContext _context;
        private const int PAGE_SIZE = 5;
        public TableController()
        {
            _context = new ApplicationDbContext();
        }

        // GET: Table

        // 3 action sREsults, bestPools, bestTeams, and one for the others. The others will all be the same view i.e. 1 object per player

        public ActionResult BestPools(int? page)
        {
            var pageNumber = page ?? 1;
            var pageSize = PAGE_SIZE;

            var eventId = Predict.Helper.Cache.GetEventId();
            var tableViewModels = _context.Database.SqlQuery<BestPoolsTableViewModel>("spGetBestPoolsTable @intEventId"
                , new SqlParameter("@intEventId", eventId)).ToList();

            ViewBag.TableName = "Best Pools";

            return View(tableViewModels.ToPagedList(pageNumber, pageSize));

        }

        public ActionResult BestTeams(int? page)
        {

            var pageNumber = page ?? 1;
            var pageSize = PAGE_SIZE;

            var eventId = Predict.Helper.Cache.GetEventId();
            var tableViewModels = _context.Database.SqlQuery<BestTeamsTableViewModel>("spGetBestTeamsTable @intEventId"
                , new SqlParameter("@intEventId", eventId)).ToList();

            ViewBag.TableName = "Best Teams";

            return View(tableViewModels.ToPagedList(pageNumber, pageSize));

        }

        // routing for poolid/player

        //need to finsih of the routing for when a player clicks on a link on the home page to find their position


        //[Route("FindPlayer/{poolId}/{playerId}")]
        [Route("Table/FindPlayer/{poolId}/{playerId}")]
        public ActionResult FindPlayer(int poolId, string playerId)
        {
            //poolId = 1;
            //var playerId = "1c6a9081-e07f-4142-beae-1dc806ae31ee";
            var tableViewModels = GetTableViewModel(1, (int)poolId);
            var posn = tableViewModels.FindIndex(p => p.PlayerId == playerId);
            var pageNumber = (posn / PAGE_SIZE)+1;
            ViewBag.FindPlayer = playerId;

            return View("Index",tableViewModels.ToPagedList(pageNumber, PAGE_SIZE));
        }

        [Route("index /{tableTypeId ?}/{poolId?}/{page?}/{playerId?}")]
        public ActionResult Index(int? tableTypeId, int? poolId, int? page)
        {
            // tableTypeId 1 = Normal or global pool
            // tableTypeId 2 = Pools Best, based on global league scoring
            // tableTypeId 3 = Teams Best, based on global league scoring

            var pageNumber = page ?? 1;
            var pageSize = PAGE_SIZE;

            tableTypeId = tableTypeId ?? 1;
            poolId = (poolId ?? Predict.Helper.Cache.GetGlobalPoolId());

            if (tableTypeId == 1)
            {

                var pool = _context.Pools.FirstOrDefault(p => p.Id == poolId);
                if (pool == null)
                    return HttpNotFound();

                ViewBag.PoolName = pool.PoolName;
            }
            else if (tableTypeId == 2)
            {
                ViewBag.PoolName = "Pools Best Players";
            }
            else if (tableTypeId == 3)
            {
                ViewBag.PoolName = "Teams Best Players";
            }


            var tableViewModels = GetTableViewModel((int)tableTypeId, (int)poolId);

            ViewBag.PoolId = poolId;
            ViewBag.TableTypeId = tableTypeId;

            return View(tableViewModels.ToPagedList(pageNumber, pageSize));
        }

        private List<TableViewModel> GetTableViewModel(int tableTypeId, int poolId)
        {

                var eventId = Predict.Helper.Cache.GetEventId();
                var tableViewModels = _context.Database.SqlQuery<TableViewModel>(
                    "spGetTable @intEventId, @intTableTypeId, @intPoolId "
                    , new SqlParameter("@intEventId", eventId)
                    , new SqlParameter("@intTableTypeId", tableTypeId)
                    , new SqlParameter("@intPoolId", poolId)).ToList();
                return tableViewModels;
            
        }
    }
}