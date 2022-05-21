using System;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Predict.Models;
using System.Data.Entity;
using System.Configuration;
using System.Web;

namespace Predict.Controllers.Api
{
    public class EventPlayersController : ApiController
    {
        private readonly ApplicationDbContext _context;

        public EventPlayersController()
        {
            _context = new ApplicationDbContext();
        }

        [HttpPost]
        [Route("api/EventPlayers/delete/{eventId}")]
        public IHttpActionResult Delete(short eventId)
        {
            var allOk = CheckBasics(eventId);

            if (!allOk)
                return BadRequest("Invalid parameters");

            var playerId = User.Identity.GetUserId();

            var myEventPlayer = _context.EventPlayers.Where(a => a.EventId == eventId && a.PlayerId == playerId).FirstOrDefault();

            // User has selected NOT to be in this event and the record exists
            myEventPlayer.Enabled = false;
            myEventPlayer.ModifiedDateTime = DateTime.UtcNow;

            // disable all EventPoolPlayers entries to pools for this event
            var eventPoolPlayers = _context.EventPoolPlayers
                .Where(f => f.PlayerId == playerId && f.EventId == eventId).ToList();

            eventPoolPlayers.ForEach(a => a.Enabled = false);
            eventPoolPlayers.ForEach(a => a.ModifiedDateTime = DateTime.UtcNow);

            // remove any existing fixture predictions as player is no longer playing this event
            var fixturePredictions = _context.FixturePredictions
                .Where(a => a.EventId == eventId)
                .Where(a => a.PlayerId == playerId).ToList();
            _context.FixturePredictions.RemoveRange(fixturePredictions);

            // remove any ko fixture predictions as player is no longer playing this event
            var koFixturePredictions = _context.KoFixturePredictions
                .Include(a => a.KoFixture)
                .Where(a => a.KoFixture.EventId == eventId)
                .Where(a => a.PlayerId == playerId).ToList();
            _context.KoFixturePredictions.RemoveRange(koFixturePredictions);

            // remove any bonus question predictions as player is no longer playing this event
            var bonusPredictions = _context.BonusQuestionPredictions
                .Include(a => a.BonusQuestion)
                .Where(a => a.BonusQuestion.EventId == eventId)
                .Where(a => a.PlayerId == playerId).ToList();
            _context.BonusQuestionPredictions.RemoveRange(bonusPredictions);

            _context.EventPlayers.AddOrUpdate(myEventPlayer);
            _context.SaveChanges();

            return Ok();
        }

        [HttpPost]
        [Route("api/EventPlayers/join/{eventId}")]
        public IHttpActionResult Join(short eventId)
        {
            var allOk = CheckBasics(eventId);

            if (!allOk)
                return BadRequest("Invalid parameters");

            var playerId = User.Identity.GetUserId();
            var myEventPlayer = _context.EventPlayers.Where(a => a.EventId == eventId && a.PlayerId == playerId).FirstOrDefault();

            if (myEventPlayer == null)
            {
                // User has selected to be in this event and the record does not exist
                myEventPlayer = new EventPlayer
                {
                    EventId = eventId,
                    PlayerId = playerId,
                    CreatedDateTime = DateTime.UtcNow,
                    ModifiedDateTime = DateTime.UtcNow,
                    Enabled = true
                };
            }
            else
            {
                // User has selected to be in this event and the record exists
                myEventPlayer.Enabled = true;
                myEventPlayer.ModifiedDateTime = DateTime.UtcNow;

            }
            _context.EventPlayers.AddOrUpdate(myEventPlayer);

            _context.SaveChanges();

            CheckAndUpdatePoolsIfPlayerIsAdmin(eventId, playerId);
            AddPlayerToPoolsThatAreCompetingInThisEvent(eventId, playerId);
            
            return Ok();
        }

        private bool CheckAndUpdatePoolsIfPlayerIsAdmin(short eventId, string playerId)
        {
            // If this member is the owner of a pool, then join the event to the pool
            var globalPoolId = Convert.ToInt32(ConfigurationManager.AppSettings["GlobalPoolId"]);
            var pools = _context.Pools.Where(a => a.AdminPlayerId == playerId && a.Id != globalPoolId).ToList();
            var changeMade = false;
            foreach (var pool in pools)
            {
                changeMade = true;
                var eventPool = _context.EventPools.Where(a => a.PoolId == pool.Id && a.EventId == eventId).FirstOrDefault();
                if (eventPool != null && eventPool.Enabled == false)
                {
                    eventPool.Enabled = true;
                    eventPool.ModifiedDateTime = DateTime.UtcNow;
                }
                else if (eventPool == null)
                {
                    eventPool = new EventPool
                    {
                        EventId = eventId,
                        PoolId = pool.Id,
                        Enabled = true,
                        ModifiedDateTime = DateTime.UtcNow,
                        CreatedDateTime = DateTime.UtcNow
                    };
                }
                _context.EventPools.AddOrUpdate(eventPool);

                var thisPoolPoolPlayers = _context.PoolPlayers.Where(a => a.PoolId == pool.Id);
                foreach (var thisPoolPlayer in thisPoolPoolPlayers)
                {
                    // check each player of this pool and if they are participating in this comp then ensure that the player/pool/event is linked

                    var playerPlayingThisEvent = _context.EventPlayers.Where(a => a.EventId == eventId && a.PlayerId == thisPoolPlayer.PlayerId).Any();

                    if (!playerPlayingThisEvent)
                        continue;

                    var eventPoolPlayer = _context.EventPoolPlayers.Where(a => a.PoolId == pool.Id && a.EventId == eventId && a.PlayerId == thisPoolPlayer.PlayerId).FirstOrDefault();
                    if (eventPoolPlayer != null && eventPoolPlayer.Enabled == false)
                    {
                        eventPoolPlayer.Enabled = true;
                        eventPoolPlayer.ModifiedDateTime = DateTime.UtcNow;
                    }
                    else if (eventPoolPlayer == null)
                    {
                        // Add the player to the event/pool
                        eventPoolPlayer = new EventPoolPlayer
                        {
                            PlayerId = thisPoolPlayer.PlayerId,
                            EventId = eventId,
                            PoolId = pool.Id,
                            Enabled = true,
                            CreatedDateTime = DateTime.UtcNow,
                            ModifiedDateTime = DateTime.UtcNow,
                            AdminApprovedDateTime = DateTime.UtcNow,
                            PoolPosition = 0,
                            CorrectScore = 0,
                            CorrectResult = 0,
                            WinMargin = 0,
                            KoScore = 0,
                            BonusScore = 0,
                            TotalScore = 0
                        };
                        _context.EventPoolPlayers.Add(eventPoolPlayer);
                    }
                }

            }
            if (changeMade == true)
                _context.SaveChanges();

            return changeMade;
        }

        private bool AddPlayerToPoolsThatAreCompetingInThisEvent(short eventId, string playerId)
        {

            // Make sure the player is associated to all pools that are associated to the player that are associated to this event
            var eventPools = _context.EventPools.Where(a => a.EventId == eventId)
                .Where(a => a.Enabled == true).ToList();

            var poolPlayers = _context.PoolPlayers.Where(a => a.PlayerId == playerId)
                .Where(a => a.Enabled == true).ToList();

            var changesMade = false;

            foreach (var eventPool in eventPools)
            {
                var exists = poolPlayers.Exists(a => a.PoolId == eventPool.PoolId && a.Enabled == true);
                if (exists)
                {
                    changesMade = true;
                    // If the player is also associated the the pool then associate the player/pool to the event
                    var myEventPoolPlayer = _context.EventPoolPlayers.FirstOrDefault(f =>
                        f.EventId == eventId && f.PlayerId == playerId && f.PoolId == eventPool.PoolId);
                    if (myEventPoolPlayer == null)
                    {
                        myEventPoolPlayer = new EventPoolPlayer()
                        {
                            CreatedDateTime = DateTime.UtcNow,
                            ModifiedDateTime = DateTime.UtcNow,
                            EventId = eventId,
                            PlayerId = playerId,
                            PoolId = eventPool.PoolId,
                            AdminApprovedDateTime = DateTime.UtcNow,
                            PoolPosition = 0,
                            Enabled = true
                        };
                    }
                    else
                    {
                        myEventPoolPlayer.Enabled = true;
                        myEventPoolPlayer.ModifiedDateTime = DateTime.UtcNow;
                    }

                    _context.EventPoolPlayers.AddOrUpdate(myEventPoolPlayer);
                }
            }
            if(changesMade)
                _context.SaveChanges();

            return changesMade;
        }

        private bool CheckBasics(short eventId)
        {
            var playerId = User.Identity.GetUserId();

            if (!User.Identity.IsAuthenticated)
                return false;

            var myEvent = _context.Events.FirstOrDefault(p => p.Id == eventId);
            if (myEvent == null)
                return false;

            return true;
        }
    }
}
