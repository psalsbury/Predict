using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Ajax.Utilities;
using Predict.Models;

namespace Predict.Controllers.Api
{
    public class EventPlayersController : ApiController
    {
        private readonly ApplicationDbContext _context;

        public EventPlayersController()
        {
            _context = new ApplicationDbContext();
        }

        // POST: api/EventPoolPlayers/5
        [HttpPost]
        [Route("api/EventPlayers/delete/{EventId}/{playerId}")]
        public IHttpActionResult Delete(int eventId, string playerId)
        {
            var eventPlayer = _context.EventPlayers.SingleOrDefault(c => c.EventId == eventId && c.PlayerId == playerId);

            if (eventPlayer == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            _context.EventPlayers.Remove(eventPlayer);
            _context.SaveChanges();

            return Ok();
        }

        [HttpPost]
        [Route("api/EventPlayers/AddNewEventPlayer/{eventId}/{playerId}")]
        public IHttpActionResult AddNewPoolPlayer(short eventId, string playerId)
        {
            var myEvent = _context.Events.SingleOrDefault(p => p.Id == eventId);
            if (myEvent == null)
                return BadRequest("Event does not exist");

            var eventPlayer = _context.EventPlayers.SingleOrDefault(c => c.EventId == eventId && c.PlayerId == playerId);

            if (eventPlayer != null) return BadRequest("Player already belongs to this event");

            eventPlayer = new EventPlayer
            {
                PlayerId = playerId,
                EventId = eventId,
                CreatedDateTime = DateTime.UtcNow,
                ModifiedDateTime = DateTime.UtcNow
            };
            _context.EventPlayers.Add(eventPlayer);
            _context.SaveChanges();

            return Ok();
        }

    }
}
