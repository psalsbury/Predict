using Microsoft.Ajax.Utilities;
using Predict.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Http;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Logging;
using Predict.Helper;

namespace Predict.Controllers.Api
{
    public class PoolPlayersController : ApiController
    {
        private readonly ApplicationDbContext _context;

        public PoolPlayersController()
        {
            _context = new ApplicationDbContext();
        }

        [HttpPost]
        [Route("api/PoolPlayers/delete/{poolId}/{playerId}")]
        public IHttpActionResult Delete(int poolId, string playerId)
        {
            var poolPlayer = _context.PoolPlayers.SingleOrDefault(c => c.PoolId == poolId && c.PlayerId == playerId);

            if (poolPlayer == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);
            
            var eventPoolPlayers = _context.EventPoolPlayers.Include(a => a.Event)
                .Where(a => a.Event.StartDateTime >= DateTime.UtcNow)
                .Where(a => a.PlayerId == playerId)
                .Where(a => a.PoolId == poolId).ToList();

            poolPlayer.Enabled = false;
            _context.PoolPlayers.AddOrUpdate(poolPlayer);

            // Remove player from any events that have not yet started
            foreach (var eventPoolPlayer in eventPoolPlayers)
            {
                eventPoolPlayer.Enabled = false;
                eventPoolPlayer.ModifiedDateTime = DateTime.UtcNow;
                _context.EventPoolPlayers.AddOrUpdate(eventPoolPlayer);
            }

            _context.SaveChanges();

            return Ok();
        }


        [HttpPost]
        [Route("api/PoolPlayers/AddNewPoolPlayer/{poolId}/{playerId}/{joinCode}")]
        public IHttpActionResult AddNewPoolPlayer(int poolId, string playerId, string joinCode)
        {
            var pool = _context.Pools.SingleOrDefault(p => p.Id == poolId);
            if (pool == null)
                return BadRequest("Pool does not exist");

            if (!pool.JoinCode.IsNullOrWhiteSpace() && pool.JoinCode != joinCode)
                return BadRequest("Join Code is Not Valid");

            var poolPlayer = _context.PoolPlayers.SingleOrDefault(c => c.PoolId == poolId && c.PlayerId == playerId);
            if (poolPlayer == null)
            {

                poolPlayer = new PoolPlayer
                {
                    PlayerId = playerId,
                    PoolId = poolId,
                    CreatedDateTime = DateTime.UtcNow,
                    ModifiedDateTime = DateTime.UtcNow,
                    Enabled = true
                };
            }
            else
            {
                poolPlayer.Enabled = true;
                poolPlayer.ModifiedDateTime = DateTime.UtcNow;
            }
            _context.PoolPlayers.AddOrUpdate(poolPlayer);

            // Add player into EventPoolPlayers for events that are still outstanding
            
            // First get any records that already exist (they should be disabled)
            var eventPoolPlayers = _context.EventPoolPlayers
                .Include(a => a.Event)
                .Where(a => a.PlayerId == playerId)
                .Where(a => a.PoolId == poolId)
                .Where(a => a.Event.StartDateTime > DateTime.UtcNow).ToList();

            foreach (var eventPoolPlayer in eventPoolPlayers)
            {
                eventPoolPlayer.Enabled = true;
                eventPoolPlayer.ModifiedDateTime = DateTime.UtcNow;
            }

            // now get new ones
            var eventPools = _context.EventPools
                .Include(a => a.Event)
                .Where(a => a.PoolId == poolId)
                .Where(a => a.Event.StartDateTime > DateTime.UtcNow)
                .Where(a => a.Enabled==true).ToList();

            foreach (var eventPool in eventPools)
            {
                if (!eventPoolPlayers.Any(a => a.EventId == eventPool.EventId))
                {
                    var eventPoolPlayer = new EventPoolPlayer
                    {
                        PlayerId = playerId, EventId = eventPool.EventId, CreatedDateTime = DateTime.UtcNow,
                        ModifiedDateTime = DateTime.UtcNow, PoolId = eventPool.PoolId
                    };
                    _context.EventPoolPlayers.Add(eventPoolPlayer);
                }
            }
            _context.SaveChanges();

            return Ok();
        }
    }
}