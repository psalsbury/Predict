using System;
using System.Linq;
using System.Web.Http;
using Predict.Models;
using System.Data.Entity.Migrations;
using Microsoft.AspNet.Identity;

namespace Predict.Controllers.Api
{
    public class JokesController : ApiController
    {

        private readonly ApplicationDbContext _context;

        public JokesController()
        {
            _context = new ApplicationDbContext();
        }

        [HttpPost]
        [Route("api/Jokes/delete/{Id}")]
        public IHttpActionResult Delete(int Id)
        {

            if (!User.Identity.IsAuthenticated)
                return BadRequest();

            var playerId = User.Identity.GetUserId();

            var joke = _context.Jokes.SingleOrDefault(p => p.Id == Id);
            if (joke == null)
                return BadRequest("Joke does not exist");

            if (joke.PlayerId != playerId)
                return BadRequest("Invalid user");

            var jokeRatings = _context.JokeRatings.Where(a => a.JokeId == joke.Id).ToList();

            _context.JokeRatings.RemoveRange(jokeRatings);
            _context.Jokes.Remove(joke);
            _context.SaveChanges();

            return Ok();
        }

    }
}
