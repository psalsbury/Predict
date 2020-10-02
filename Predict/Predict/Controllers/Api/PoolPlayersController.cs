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

        // POST: api/EventPoolPlayers/5
        [HttpPost]
        [Route("api/EventPoolPlayers/delete/{poolId}/{playerId}")]
        public IHttpActionResult Delete(int poolId, string playerId)
        {
            var poolPlayer = _context.EventPoolPlayers.SingleOrDefault(c => c.PoolId == poolId && c.PlayerId == playerId);

            if (poolPlayer == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            _context.EventPoolPlayers.Remove(poolPlayer);
            _context.SaveChanges();

            return Ok();
        }

        [HttpGet]
        public IEnumerable<EventPoolPlayer> GetPoolPlayers()
        {
            return _context.EventPoolPlayers.ToList();
        }

        [HttpGet]
        [Route("api/EventPoolPlayers/{poolId}/{playerId}")]
        public EventPoolPlayer GetPoolPlayer(int poolId, string playerId)
        {
            var poolPlayer = _context.EventPoolPlayers.SingleOrDefault(p => p.PoolId == poolId && p.PlayerId == playerId);
            if (poolPlayer == null) throw new HttpResponseException(HttpStatusCode.NotFound);

            return poolPlayer;
        }

        [HttpPost]
        [Route("api/EventPoolPlayers/{poolId}/{playerId}")]
        public IHttpActionResult Authorize(int poolId, string playerId)
        {
            var poolPlayer = _context.EventPoolPlayers.SingleOrDefault(c => c.PoolId == poolId && c.PlayerId == playerId);

            if (poolPlayer == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            poolPlayer.AdminApprovedDateTime = DateTime.UtcNow;
            _context.SaveChanges();

            return Ok();
        }

        [HttpPost]
        [Route("api/EventPoolPlayers/AddNewPoolPlayer/{poolId}/{playerId}/{joinCode}")]
        public IHttpActionResult AddNewPoolPlayer(int poolId, string playerId, string joinCode)
        {
            var pool = _context.Pools.SingleOrDefault(p => p.Id == poolId);
            if (pool == null)
                return BadRequest("Player does not exist");

            if (!pool.JoinCode.IsNullOrWhiteSpace() && pool.JoinCode != joinCode)
                return BadRequest("Join Code is Not Valid");

            var poolPlayer = _context.EventPoolPlayers.SingleOrDefault(c => c.PoolId == poolId && c.PlayerId == playerId);

            if (poolPlayer != null) return BadRequest("Player already belongs to this pool");

            poolPlayer = new EventPoolPlayer
            {
                PlayerId = playerId,
                PoolId = poolId,
                CreatedDateTime = DateTime.UtcNow,
                ModifiedDateTime = DateTime.UtcNow
            };
            _context.EventPoolPlayers.Add(poolPlayer);
            _context.SaveChanges();

            return Ok();
        }
    }
}