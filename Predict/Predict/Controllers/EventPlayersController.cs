using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Predict.Models;

namespace Predict.Controllers
{
    public class EventPlayersController : Controller
    {

        private readonly ApplicationDbContext _context;

        public EventPlayersController()
        {
            _context = new ApplicationDbContext();
        }

        // GET: EventPlayers
        public ActionResult Index()
        {
            var myEvents = (List<Event>) Predict.Helper.Cache.GetCachedItem("Events");

            return View(myEvents);
        }

        public ActionResult Save()
        {
            var playerId = User.Identity.GetUserId();
            var myEventPlayers = _context.EventPlayers.Where(a => a.PlayerId == playerId).ToList();
            var myEvents = (List<Event>)Predict.Helper.Cache.GetCachedItem("Events");
            foreach (Event myEvent in myEvents)
            {
                var myEventPlayer = myEventPlayers.FirstOrDefault(m => m.EventId == myEvent.Id);

                var playingIn = Request["event_" + myEvent.Id];
                if (playingIn == null && myEventPlayer != null)
                {
                    _context.EventPlayers.Remove(myEventPlayer);
                }
                else if (playingIn != "" && myEventPlayer == null)
                {    
                    myEventPlayer = new EventPlayer
                    {
                        EventId = myEvent.Id,
                        PlayerId = playerId,
                        CreatedDateTime = DateTime.Today,
                        ModifiedDateTime = DateTime.Today
                    };
                    _context.EventPlayers.Add(myEventPlayer);                                       
                }
                _context.SaveChanges();
            }
            
            Helper.SessionHelper.UpdateEventPlayersSessionVariable(_context, Session,playerId,true);
            return RedirectToAction("Index", "Home");
        }
    }

}
