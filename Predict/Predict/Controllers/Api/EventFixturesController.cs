using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Antlr.Runtime.Tree;
using Microsoft.AspNet.Identity;
using Predict.Models;

namespace Predict.Controllers.Api
{
    public class EventFixturesController : ApiController
    {
        private readonly ApplicationDbContext _context;

        public EventFixturesController()
        {
            _context = new ApplicationDbContext();
        }

        [HttpPost]
        [Route("api/EventFixtures/delete/{eventId}/{fixtureId}")]
        public IHttpActionResult Delete(short eventId, int fixtureId)
        {
            var allOk = CheckBasics(eventId, fixtureId);

            if (!allOk)
                return BadRequest("Invalid parameters");

            var eventFixture =
                _context.EventFixtures.FirstOrDefault(a => a.EventId == eventId && a.FixtureId == fixtureId);

            if (eventFixture != null)
            {
                _context.EventFixtures.Remove(eventFixture);

                // Remove all pending predictions for this fixture that has been removed
                _context.FixturePredictions.RemoveRange(
                    _context.FixturePredictions.Where(a => a.EventId == eventId && a.FixtureId == fixtureId));
                _context.SaveChanges();
            }

            Helper.Cache.UpdateEventStartEnd(_context, eventId);

            return Ok();
        }

        [HttpPost]
        [Route("api/EventFixtures/add/{eventId}/{fixtureId}")]
        public IHttpActionResult Add(short eventId, int fixtureId)
        {
            var allOk = CheckBasics(eventId, fixtureId);

            if (!allOk)
                return BadRequest("Invalid parameters");

            // Add fixture to the event
            var eventFixture = new EventFixture
            {
                FixtureId = fixtureId,
                CreatedDateTime = DateTime.UtcNow,
                ModifiedDateTime = DateTime.UtcNow,
                EventId = eventId
            };
            _context.EventFixtures.AddOrUpdate(eventFixture);
            _context.SaveChanges();

            Helper.Cache.UpdateEventStartEnd(_context, eventId);

            var fixtureDateTime = _context.Fixtures.Where(a => a.Id == fixtureId).FirstOrDefault().FixtureDateTime;

            // If a fixture has been added for today, then check what time the system needs to get the result
            if (fixtureDateTime.Date == DateTime.UtcNow.Date)
                RapidApi.RapidApiHelper.SetNextResultCheckDateTime(true, true);

            return Ok();
        }

        private bool CheckBasics(short eventId, int fixtureId)
        {
            var playerId = User.Identity.GetUserId();

            if (!User.Identity.IsAuthenticated)
                return false;

            var myEvent = _context.Events.FirstOrDefault(p => p.Id == eventId);
            if (myEvent == null)
                return false;

            var fixture = _context.Fixtures.FirstOrDefault(p => p.Id == fixtureId);
            if (fixture == null)
                return false;

            if (!(myEvent.CreatedByPlayerId == playerId || User.IsInRole("Admin")))
                return false;

            return true;
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
