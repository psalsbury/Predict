using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Predict.Models;
using System.Data.Entity;

namespace Predict.Controllers
{
    public class JokesController : Controller
    {

        private readonly ApplicationDbContext _context;

        public JokesController()
        {
            _context = new ApplicationDbContext();
        }
        // GET: Jokes
        public ActionResult ShowJoke()
        {

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            var playerId = User.Identity.GetUserId();

            // Get all ratings that have been saved for this player already
            var jokeRatings = _context.JokeRatings.Where(r => r.PlayerId == playerId).Select(r => r.JokeId).ToList();

            // Get a joke that has not yet been rated by this player
            var joke = _context.Jokes
                .FirstOrDefault(j => j.PlayerId != playerId
                                     && !jokeRatings.Contains(j.Id));
            if (joke == null)
            {
                var rand = new Random();
                int toSkip = rand.Next(0, _context.Jokes.Count());

                var jokesToChoseFrom = _context.Jokes.Any((j => j.PlayerId != playerId));
                if (jokesToChoseFrom)
                {
                    joke = _context.Jokes.Where(j => j.PlayerId != playerId).OrderBy(r => Guid.NewGuid()).Skip(toSkip)
                        .Take(1).First();

                    var existingRating = _context.JokeRatings
                        .First(r => r.PlayerId == playerId && r.JokeId == joke.Id).PlayerJokeRating;
                    ViewBag.PlayerJokeRating = existingRating;
                }
                else
                {
                    return View("NoJokes");
                }
            }

            return View(joke);
        }
    }
}