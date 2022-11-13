using System;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Predict.Models;
using System.Data.Entity;
using System.Configuration;

namespace Predict.Controllers.Api
{
    public class EventPoolPlayerController : ApiController
    {

        private readonly ApplicationDbContext _context;

        public EventPoolPlayerController()
        {
            _context = new ApplicationDbContext();
        }

        private bool CheckBasics(short eventId, int poolId)
        {
            var currentPlayerId = User.Identity.GetUserId();

            if (!User.Identity.IsAuthenticated)
                return false;

            var myEvent = _context.Events.FirstOrDefault(p => p.Id == eventId);
            if (myEvent == null)
                return false;

            var isAdmin = _context.Pools.Any(a => a.Id == poolId & a.AdminPlayerId == currentPlayerId);
            if(!isAdmin)
                return false;

            return true;
        }

        [HttpPost]
        [Route("api/EventPlayers/delete/{eventId}/{poolId}/{playerId}")]
        public IHttpActionResult Delete(short eventId, int poolId, string playerId)
        {
            var allOk = CheckBasics(eventId,poolId);

            if (!allOk)
                return BadRequest("Invalid parameters");

            var myEventPoolPlayer = _context.EventPoolPlayers.Where(a => a.EventId == eventId && a.PoolId == poolId && a.PlayerId == playerId ).FirstOrDefault();

            // Remove player from this comp/pool
            myEventPoolPlayer.Enabled = false;
            myEventPoolPlayer.ModifiedDateTime = DateTime.UtcNow;

            _context.EventPoolPlayers.AddOrUpdate(myEventPoolPlayer);
            _context.SaveChanges();

            return Ok();
        }

    }
}
