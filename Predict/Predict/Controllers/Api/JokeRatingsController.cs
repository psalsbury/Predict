using System;
using System.Linq;
using System.Web.Http;
using Predict.Models;
using System.Data.Entity.Migrations;
using Microsoft.AspNet.Identity;

namespace Predict.Controllers.Api
{
    public class JokeRatingsController : ApiController
    {
        private readonly ApplicationDbContext _context;

        public JokeRatingsController()
        {
            _context = new ApplicationDbContext();
        }

        // POST: api/JokeRatings

        [HttpPost]
        [Route("api/JokeRatings/PostJokeRating/{playerId}/{jokeId}/{playerJokeRating}")]
        public IHttpActionResult PostJokeRating(string playerId, int jokeId, Int16 playerJokeRating)
        {

            if (!User.Identity.IsAuthenticated)
                return BadRequest();

            var jokeExists = _context.Jokes.Any(j => j.Id == jokeId);
            if (!jokeExists)
                return BadRequest("Joke does not exist");

            if (playerId != User.Identity.GetUserId())
                return BadRequest("Invalid user");

            var jokeRating = _context.JokeRatings.SingleOrDefault(p => p.JokeId == jokeId && p.PlayerId == playerId);
            if (jokeRating == null)
            {
                jokeRating = new JokeRating
                {
                    PlayerId = playerId,
                    CreatedDateTime = DateTime.UtcNow,
                    JokeId = jokeId,
                    PlayerJokeRating = playerJokeRating
                };
            }
            else
            {
                jokeRating.PlayerJokeRating = playerJokeRating;
            }

            _context.JokeRatings.AddOrUpdate(jokeRating);
            _context.SaveChanges();

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

        private bool JokeRatingExists(int jokeId, string playerId)
        {
            return _context.JokeRatings.Count(e => e.JokeId == jokeId && e.PlayerId == playerId) > 0;
        }
    }
}