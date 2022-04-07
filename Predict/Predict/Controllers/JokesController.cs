using System;
using Predict.ViewModels;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Predict.Models;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Data.Entity.Migrations;


namespace Predict.Controllers
{
    public class JokesController : Controller
    {

        private readonly ApplicationDbContext _context;

        public JokesController()
        {
            _context = new ApplicationDbContext();
        }

        public ActionResult JokesTable()
        {

            var tableViewModels = _context.Database.SqlQuery<JokeTable>("spGetJokesLeagues").ToList();
            return View(tableViewModels);

        }
        [HttpGet]
        public ActionResult AddJoke()
        {
            var joke = new Joke();
            joke.PlayerId = User.Identity.GetUserId();
            return View(joke);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddJoke(Joke joke)
        {

            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var playerId = User.Identity.GetUserId();

            if (joke.PlayerId!= playerId)
                ModelState.AddModelError("Password", "Passwords do not match");

            if (!ModelState.IsValid)
            {
                return View("AddJoke", joke);
            }

            var player = _context.Players.FirstOrDefault(a => a.Id == playerId);
            var email = new IdentityMessage
            {
                Body = player.PlayerName + " has submitted a " + (joke.Id == 0 ? "change to " : "") + "Joke <br><br> "
                    + "Joke Text = " + joke.JokeText
                    + "<br><br>"
                    + "Joke Punchline = " + joke.JokePunchline,
                Subject = "New Joke Submitted by  " + player.PlayerName,
                Destination = "pete@salsbury.co.uk"
            };
            Helper.Cache.SendEmail(email);
            joke.ModifiedDateTime = DateTime.Now.ToUniversalTime();
            if (joke.Id==0)
            {
                joke.PlayerId = playerId;
                joke.CreatedDateTime = DateTime.Now.ToUniversalTime();
                _context.Jokes.Add(joke);
            }

            _context.Jokes.AddOrUpdate(joke);
            _context.SaveChanges();

            if (joke.Id == 0)
            {
                Predict.Helper.SessionHelper.UpdatePlayerJokeCount(_context, Session, playerId, true);
            }

            return RedirectToAction("DisplayJokeList");
        }

        [HttpGet]
        public ActionResult EditJoke(int id)
        {

            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var joke = _context.Jokes.FirstOrDefault(a => a.Id == id);

            if (joke==null)
                return RedirectToAction("Login", "Account");

            return View("AddJoke",joke);

        }

        [HttpGet]
        public ActionResult DisplayJokeList()
        {
            var playerId = User.Identity.GetUserId();
            var tableViewModels = _context.Database.SqlQuery<JokeTable>(
                  "spGetJokesLeagues @intPlayerId"
                  , new SqlParameter("@intPlayerId", playerId)).ToList();

            return View(tableViewModels);
        }

        // GET: Jokes
        [HttpGet]
        public ActionResult ShowSelectedJoke(int id)
            
        {
            var joke = _context.Jokes.Where(a => a.Id == id).FirstOrDefault();
            var playerId = User.Identity.GetUserId();

            var existingRating = _context.JokeRatings
                .FirstOrDefault(r => r.PlayerId == playerId && r.JokeId == joke.Id);
            if(existingRating!=null)
                ViewBag.PlayerJokeRating = existingRating.PlayerJokeRating;

            if (playerId == joke.PlayerId)
                ViewBag.OwnJoke = true;

            return View("ShowJoke", joke);
        }

        // GET: Jokes
        [HttpGet]
        public ActionResult ShowJoke()
        {

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.PlayerJokeRating = 0;
            var playerId = User.Identity.GetUserId();

            // Get all ratings that have been saved for this player already
            var jokeRatings = _context.JokeRatings.Where(r => r.PlayerId == playerId).Select(r => r.JokeId).ToList();

            // Get a joke that has not yet been rated by this player
            var joke = _context.Jokes.Where(j => j.PlayerId != playerId && !jokeRatings.Contains(j.Id)).OrderBy(r => Guid.NewGuid())
                    .Take(1).FirstOrDefault();

            if (joke == null)
            {
                // If there are no jokes that havent been rated by this player..
                var rand = new Random();

                var jokesToChoseFrom = _context.Jokes.Any((j => j.PlayerId != playerId));
                if (jokesToChoseFrom)
                {
                    joke = _context.Jokes.Where(j => j.PlayerId != playerId).OrderBy(r => Guid.NewGuid())
                        .Take(1).FirstOrDefault();

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