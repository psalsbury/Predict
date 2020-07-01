using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using PagedList;
using Predict.Models;
using Predict.ViewModels;

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

        // GET: Table

        // 3 action sREsults, bestPools, bestTeams, and one for the others. The others will all be the same view i.e. 1 object per player

        //[Route("FindPlayer/{poolId}/{playerId}")]
        [Route("Table/FindPlayer/{poolId}/{playerId}")]
        public ActionResult FindPlayer(int poolId, string playerId)
        {
            //poolId = 1;
            //var playerId = "1c6a9081-e07f-4142-beae-1dc806ae31ee";
            var tableViewModels = GetTableViewModel(poolId);
            var posn = tableViewModels.FindIndex(p => p.PlayerId == playerId);
            var pageNumber = posn / PAGE_SIZE + 1;
            ViewBag.FindPlayer = playerId;

            return View("Index", tableViewModels.ToPagedList(pageNumber, PAGE_SIZE));
        }

        [Route("index /{tableTypeId ?}/{poolId?}/{page?}/{playerId?}")]
        public ActionResult Index(int poolId, int? page)
        {
            // tableTypeId 1 = Normal or global pool
            // tableTypeId 2 = Pools Best, based on global league scoring
            // tableTypeId 3 = Teams Best, based on global league scoring

            var pageNumber = page ?? 1;
            var pageSize = PAGE_SIZE;

            var pool = _context.Pools.FirstOrDefault(p => p.Id == poolId);
            if (pool == null)
                return HttpNotFound();

            ViewBag.PoolName = pool.PoolName;

            var tableViewModels = GetTableViewModel(poolId);

            ViewBag.PoolId = poolId;
            ViewBag.EventId = pool.EventId;

            return View(tableViewModels.ToPagedList(pageNumber, pageSize));
        }

        private List<TableViewModel> GetTableViewModel(int poolId)
        {
            var tableViewModels = _context.Database.SqlQuery<TableViewModel>(
                "spGetTable @intPoolId"
                , new SqlParameter("@intPoolId", poolId)).ToList();
            return tableViewModels;
        }
    }
}