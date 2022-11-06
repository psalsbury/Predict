using System;
using System.Linq;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Predict.Models;

namespace Predict.Controllers.Api
{
    public class EventsController : ApiController
    {
        private readonly ApplicationDbContext _context;
        public EventsController()
        {
            _context = new ApplicationDbContext();
        }

        [HttpPost]
        [Route("api/Events/delete/{eventId}")]
        public IHttpActionResult Delete(short eventId)
        {

            if (!User.Identity.IsAuthenticated)
                return BadRequest();

            var myEvent = _context.Events.FirstOrDefault(e => e.Id == eventId);

            // Check that Event exists
            if(myEvent==null)
                return BadRequest();

            // Check that its the user who created it that is trying to delete
            if(myEvent.CreatedByPlayerId != User.Identity.GetUserId() & !User.IsInRole("Admin"))
                return BadRequest();

            _context.FixturePredictions.RemoveRange(
                _context.FixturePredictions.Where(a => a.EventId == eventId));

            _context.EventFixtures.RemoveRange(
                _context.EventFixtures.Where(a => a.EventId == eventId));

            _context.EventPoolPlayers.RemoveRange(
                _context.EventPoolPlayers.Where(a => a.EventId == eventId));

            _context.EventPools.RemoveRange(
                _context.EventPools.Where(a => a.EventId == eventId));

            _context.EventPlayers.RemoveRange(
                _context.EventPlayers.Where(a => a.EventId == eventId));

            _context.EventPoolPlayerPositionHistory.RemoveRange(
                _context.EventPoolPlayerPositionHistory.Where(a => a.EventId == eventId));

            _context.KoFixtures.RemoveRange(
                _context.KoFixtures.Where(a => a.EventId == eventId));

            _context.KoFixturePredictions.RemoveRange(
                _context.KoFixturePredictions.Where(a => a.KoFixture.EventId == eventId));

            _context.KoWinningTeamPredictions.RemoveRange(
                _context.KoWinningTeamPredictions.Where(a => a.EventId == eventId));

            _context.EventKos.RemoveRange(
                _context.EventKos.Where(a => a.EventId == eventId));

            _context.BonusQuestionPredictions.RemoveRange(
                 _context.BonusQuestionPredictions.Where(a => a.BonusQuestion.EventId == eventId));

            _context.BonusQuestions.RemoveRange(
                _context.BonusQuestions.Where(a => a.EventId == eventId));

            _context.Events.Remove(myEvent);

            _context.SaveChanges();

            Helper.Cache.SetEventCache();

            //Leaving EventGenerations so the audit trail of the generation is maintained.

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
