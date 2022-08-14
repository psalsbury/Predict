using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Predict.Models;
using Microsoft.AspNet.Identity;

namespace Predict.Controllers.Api
{
    public class PoolsController : ApiController
    {

        private readonly ApplicationDbContext _context;

        public PoolsController()
        {
            _context = new ApplicationDbContext();
        }

        [HttpGet]
        [Route("api/Pools/GetPoolInfo/{poolId}")]
        public string GetPoolInfo(int poolId)
        {

            var table = "";
            if (!User.Identity.IsAuthenticated)
                return "";

            var pool = _context.Pools.SingleOrDefault(p => p.Id == poolId);
            if (pool == null)
                return "";

            var poolInfoFromSpViewModels = _context.Database.SqlQuery<ViewModels.PoolInfoFromSpViewModel>(
                    "spGetPoolInfo @PoolId"
                     , new System.Data.SqlClient.SqlParameter("@PoolId", poolId)).ToList();

            if(poolInfoFromSpViewModels.Count>0)
            {
                var poolInfoFromSpViewModel = poolInfoFromSpViewModels.First();
                table = "<table class='table table-striped'>"
                    + "<tr><td>Number Of Players</td>" + "<td>" + poolInfoFromSpViewModel.NbrPlayers + "</td></tr>"
                    + "<tr><td>Admin Name</td>" + "<td>" + poolInfoFromSpViewModel.AdminDisplayName + "</td></tr>"
                    + "<tr><td>Most Recent Comp Name</td>" + "<td>" + poolInfoFromSpViewModel.MostRecentEventName + "</td></tr>"
                    + "<tr><td>Most Recent Comp Winner</td>" + "<td>" + poolInfoFromSpViewModel.MostRecentEventWinner + "</td></tr>"
                    + "<tr><td>Nbr Comps</td>" + "<td>" + poolInfoFromSpViewModel.NbrCompsEntered + "</td></tr>"
                    + "<tr><td>Correct Score Points</td>" + "<td>" + poolInfoFromSpViewModel.CorrectScorePoints + "</td></tr>"
                    + "<tr><td>Correct Result Points</td>" + "<td>" + poolInfoFromSpViewModel.CorrectResultPoints + "</td></tr>"
                    + "<tr><td>Win Margin Points</td>" + "<td>" + poolInfoFromSpViewModel.WinMarginPoints + "</td></tr>"
                    + "</table>";
            }

            return table;
        }

        [HttpPost]
        [Route("api/Pools/delete/{poolId}")]
        public IHttpActionResult Delete(int poolId)
        {
            if (!User.Identity.IsAuthenticated)
                return BadRequest();

            var pool = _context.Pools.SingleOrDefault(p => p.Id == poolId);
            if (pool == null)
                return BadRequest("Pool does not exist");

            if (pool.AdminPlayerId != User.Identity.GetUserId())
                return BadRequest("Invalid user");

            // EventPoolPlayerPositionHistory
            var eventPoolPlayerPositionHistory = _context.EventPoolPlayerPositionHistory.Where(a => a.PoolId == poolId).ToList();
            _context.EventPoolPlayerPositionHistory.RemoveRange(eventPoolPlayerPositionHistory);

            // Delete EventPoolPLayers (players that are linked to this event/pool)
            var eventPoolPlayers = _context.EventPoolPlayers.Where(a => a.PoolId == poolId).ToList();
            _context.EventPoolPlayers.RemoveRange(eventPoolPlayers);

            // Delete EventPools (events linked to the pool)
            var eventPools = _context.EventPools.Where(a => a.PoolId == poolId).ToList();
            _context.EventPools.RemoveRange(eventPools);

            // Delete PoolPLayers (players linked to the pool)
            var poolPlayers = _context.PoolPlayers.Where(a => a.PoolId == poolId).ToList();
            _context.PoolPlayers.RemoveRange(poolPlayers);

            // Delete Pool
            _context.Pools.Remove(pool);

            _context.SaveChanges();

            return Ok();
        }
    }
}