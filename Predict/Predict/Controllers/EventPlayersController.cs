using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
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

        [ValidateAntiForgeryToken]
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
                    // User has selected NOT to be in this event and the record exists
                    if (myEventPlayer.Enabled == true)
                    {
                        myEventPlayer.Enabled = false;
                        myEventPlayer.ModifiedDateTime = DateTime.Today;
                        _context.EventPlayers.AddOrUpdate(myEventPlayer);
                    }
                }
                else if (playingIn != "" && myEventPlayer != null)
                {
                    // User has selected to be in this event and the record exists
                    if (myEventPlayer.Enabled == false)
                    {
                        myEventPlayer.Enabled = true;
                        myEventPlayer.ModifiedDateTime = DateTime.Today;
                        _context.EventPlayers.AddOrUpdate(myEventPlayer);

                    }
                }
                else if (playingIn != "" && myEventPlayer == null)
                {
                    // User has selected to be in this event and the record does not exist
                    myEventPlayer = new EventPlayer
                    {
                        EventId = myEvent.Id,
                        PlayerId = playerId,
                        CreatedDateTime = DateTime.Today,
                        ModifiedDateTime = DateTime.Today,
                        Enabled = true

                    };
                    _context.EventPlayers.Add(myEventPlayer);                                       
                }
                _context.SaveChanges();
            }
            
            Helper.SessionHelper.UpdateEventPlayersSessionVariable(_context, Session,playerId,true);
            Helper.SessionHelper.SetUserSessionVariables(Session, playerId,true);
            return RedirectToAction("Index", "Home");
        }
    }

}
