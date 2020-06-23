using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using Microsoft.Ajax.Utilities;
using Predict.Models;

namespace Predict.Controllers.Api
{
    public class PoolPlayersController : ApiController
    {
        private readonly ApplicationDbContext _context;

        public PoolPlayersController()
        {
            _context = new ApplicationDbContext();
        }

        // POST: api/PoolPlayers/5
        [HttpPost]
        [Route("api/PoolPlayers/delete/{poolId}/{playerId}")]
        public IHttpActionResult Delete(int poolId, string playerId)
        {
            var poolPlayer = _context.PoolPlayers.SingleOrDefault(c => c.PoolId == poolId && c.PlayerId == playerId);

            if (poolPlayer == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            _context.PoolPlayers.Remove(poolPlayer);
            _context.SaveChanges();

            return Ok();
        }

        [HttpGet]
        public IEnumerable<PoolPlayer> GetPoolPlayers()
        {
            return _context.PoolPlayers.ToList();
        }

        [HttpGet]
        [Route("api/PoolPlayers/{poolId}/{playerId}")]
        public PoolPlayer GetPoolPlayer(int poolId, string playerId)
        {
            var poolPlayer = _context.PoolPlayers.SingleOrDefault(p => p.PoolId == poolId && p.PlayerId == playerId);
            if (poolPlayer == null) throw new HttpResponseException(HttpStatusCode.NotFound);

            return poolPlayer;
        }

        [HttpPost]
        [Route("api/PoolPlayers/{poolId}/{playerId}")]
        public IHttpActionResult Authorize(int poolId, string playerId)
        {
            var poolPlayer = _context.PoolPlayers.SingleOrDefault(c => c.PoolId == poolId && c.PlayerId == playerId);

            if (poolPlayer == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            poolPlayer.AdminApprovedDateTime = DateTime.Now;
            _context.SaveChanges();

            return Ok();
        }

        [HttpPost]
        [Route("api/PoolPlayers/AddNewPoolPlayer/{poolId}/{playerId}/{joinCode}")]
        public IHttpActionResult AddNewPoolPlayer(int poolId, string playerId, string joinCode)
        {
            var pool = _context.Pools.SingleOrDefault(p => p.Id == poolId);
            if (pool == null)
                return BadRequest("Player does not exist");

            if (!pool.JoinCode.IsNullOrWhiteSpace() && pool.JoinCode != joinCode)
                return BadRequest("Join Code is Not Valid");

            var poolPlayer = _context.PoolPlayers.SingleOrDefault(c => c.PoolId == poolId && c.PlayerId == playerId);

            if (poolPlayer != null) return BadRequest("Player already belongs to this pool");

            poolPlayer = new PoolPlayer
            {
                PlayerId = playerId,
                PoolId = poolId,
                CreatedDateTime = DateTime.Now,
                ModifiedDateTime = DateTime.Now
            };
            _context.PoolPlayers.Add(poolPlayer);
            _context.SaveChanges();

            return Ok();
        }
    }
}