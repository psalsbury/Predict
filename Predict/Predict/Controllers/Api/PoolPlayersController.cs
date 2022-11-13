using Microsoft.Ajax.Utilities;
using Predict.Models;
using System;
using System.Data.Entity.Migrations;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web.Http;
using Microsoft.AspNet.Identity;

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

            if (!User.Identity.IsAuthenticated)
                return BadRequest();

            var pool = _context.Pools.SingleOrDefault(p => p.Id == poolId);
            if (pool == null)
                return BadRequest("Pool does not exist");

            if(playerId!=User.Identity.GetUserId() && pool.AdminPlayerId != User.Identity.GetUserId())
                return BadRequest("Invalid user");

            var poolPlayer = _context.PoolPlayers.SingleOrDefault(c => c.PoolId == poolId && c.PlayerId == playerId);

            if (poolPlayer == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);
            
            poolPlayer.Enabled = false;
            poolPlayer.ModifiedDateTime = DateTime.UtcNow;
            _context.PoolPlayers.AddOrUpdate(poolPlayer);

            var eventPoolPlayers = _context.EventPoolPlayers.Include(a => a.Event)
                .Where(a => a.Event.StartDateTime > DateTime.UtcNow)
                .Where(a => a.PlayerId == playerId)
                .Where(a => a.PoolId == poolId).ToList();

            // Remove player from any events that have not yet started
            foreach (var eventPoolPlayer in eventPoolPlayers)
            {
                eventPoolPlayer.Enabled = false;
                eventPoolPlayer.ModifiedDateTime = DateTime.UtcNow;
                _context.EventPoolPlayers.AddOrUpdate(eventPoolPlayer);

                // Set this so when user goes on the home page it updates the screen
                Helper.Cache.SetCachedItem("ForceUpdate*" + playerId + "*" + eventPoolPlayer.EventId, DateTime.Now.AddHours(1));
            }

            _context.SaveChanges();



            return Ok();
        }


        [HttpPost]
        [Route("api/PoolPlayers/AddNewPoolPlayer/{poolId}/{playerId}/{joinCode}")]
        public IHttpActionResult AddNewPoolPlayer(int poolId, string playerId, string joinCode)
        {

            if (!User.Identity.IsAuthenticated)
                return BadRequest();

            var pool = _context.Pools.SingleOrDefault(p => p.Id == poolId);
            if (pool == null)
                return BadRequest("Pool does not exist");

            if (playerId != User.Identity.GetUserId() && pool.AdminPlayerId != User.Identity.GetUserId())
                return BadRequest("Invalid user");

            var player = _context.Players.SingleOrDefault(p => p.Id == playerId);
            if (player == null)
                return BadRequest("Player does not exist");

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

                Helper.Cache.SetCachedItem("ForceUpdate*" + playerId + "*" + eventPoolPlayer.EventId, DateTime.Now.AddHours(1));
            }

            var eventPlayers = _context.EventPlayers
                .Where(a => a.PlayerId == playerId)
                .Where(a => a.Enabled == true)
                .ToList();

            // now get new ones
            var eventPools = _context.EventPools
                .Include(a => a.Event)
                .Where(a => a.PoolId == poolId)
                .Where(a => a.Event.StartDateTime > DateTime.UtcNow)
                .Where(a => a.Enabled==true).ToList();

            // loop through each event that this pool is in
            foreach (var eventPool in eventPools)
            {
                // if this player is playing this event, then add in the eventpoolplayer
                if(eventPlayers.Any(a => a.EventId == eventPool.EventId))
                {
                    var eventPoolPlayer = eventPoolPlayers.FirstOrDefault(a => a.EventId == eventPool.EventId);

                    if (eventPoolPlayer != null)
                    {
                        eventPoolPlayer.Enabled = true;
                        eventPoolPlayer.ModifiedDateTime = DateTime.UtcNow;
                        _context.EventPoolPlayers.AddOrUpdate(eventPoolPlayer);
                    }
                    else
                    {
                        eventPoolPlayer = new EventPoolPlayer
                        {
                            PlayerId = playerId
                            , EventId = eventPool.EventId
                            , CreatedDateTime = DateTime.UtcNow
                            , ModifiedDateTime = DateTime.UtcNow
                            , AdminApprovedDateTime = DateTime.UtcNow
                            , PoolId = eventPool.PoolId
                            , Enabled = true
                        };
                        _context.EventPoolPlayers.Add(eventPoolPlayer);
                    }
                    Helper.Cache.SetCachedItem("ForceUpdate*" + playerId + "*" + eventPoolPlayer.EventId, DateTime.Now.AddHours(1));
                }
            }
            _context.SaveChanges();

            // Ensure that the number of fixtures entered is correct
            var eventParam = new SqlParameter("@strPlayerId", playerId);
            _context.Database.ExecuteSqlCommand("EXEC spUpdateFixturesEntered @strPlayerId", eventParam);

            // Send email to pool admin when someone joins
            // May need to change this to be picked up by quartz 1 min job to send outstanding requests if errors
            if (pool.EmailNotifications)
            {
                var joiningPlayer = _context.Users.FirstOrDefault(a => a.Id == playerId);
                var adminPlayer = _context.Users.FirstOrDefault(a => a.Id == pool.AdminPlayerId);
                if (joiningPlayer != null && adminPlayer != null)
                {
                    Helper.Cache.SendNewPoolMemberEmail(player.PlayerName, joiningPlayer.Email, pool.PoolName, adminPlayer.Email);

                    poolPlayer.EmailSentToAdminDateTime = DateTime.UtcNow;
                    _context.PoolPlayers.AddOrUpdate(poolPlayer);
                    _context.SaveChanges();
                }
            }

            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}