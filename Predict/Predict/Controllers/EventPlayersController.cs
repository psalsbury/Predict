using System;
using System.Collections.Generic;
using System.Configuration;
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

        [HttpPost]
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
                        myEventPlayer.ModifiedDateTime = DateTime.Now;

                        // disable all entries to pools for this event
                        var poolPlayers = _context.PoolPlayers.Where(f => f.PlayerId == playerId && f.EventId == myEvent.Id).ToList();
                        poolPlayers.ForEach(a => a.Enabled = false);
                        poolPlayers.ForEach(a => a.ModifiedDateTime= DateTime.Now);
                        _context.EventPlayers.AddOrUpdate(myEventPlayer);
                    }

                }
                else if (playingIn != "" && myEventPlayer != null)
                {
                    // User has selected to be in this event and the record exists
                    if (myEventPlayer.Enabled == false)
                    {
                        myEventPlayer.Enabled = true;
                        myEventPlayer.ModifiedDateTime = DateTime.Now;
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
                        CreatedDateTime = DateTime.Now,
                        ModifiedDateTime = DateTime.Now,
                        Enabled = true

                    };
                    _context.EventPlayers.Add(myEventPlayer);                                       
                }

                if (playingIn != "")
                {
                    // Make sure the player has an enabled record for the default pool
                    var globalPoolId = (System.Convert.ToInt32(ConfigurationManager.AppSettings["GlobalPoolId"]));
                    var myPoolPlayer =_context.PoolPlayers.FirstOrDefault(f => f.EventId == myEvent.Id && f.PlayerId == playerId && f.PoolId== globalPoolId);
                    if (myPoolPlayer == null)
                    {
                        myPoolPlayer = new PoolPlayer()
                        {
                            CreatedDateTime = DateTime.Now
                            , ModifiedDateTime = DateTime.Now
                            , EventId = myEvent.Id
                            , PlayerId = playerId
                            , PoolId = globalPoolId
                            , AdminApprovedDateTime = DateTime.Now
                            , PoolPosition = 1
                            , Enabled =true
                        };
                    }
                    else
                    {
                        myPoolPlayer.Enabled = true;
                        myPoolPlayer.ModifiedDateTime = DateTime.Now;
                    }
                    _context.PoolPlayers.AddOrUpdate(myPoolPlayer);
                }
                _context.SaveChanges();
            }
            
            Helper.SessionHelper.UpdateEventPlayersSessionVariable(_context, Session,playerId,true);
            Helper.SessionHelper.SetUserSessionVariables(Session, playerId,true);
            return RedirectToAction("Index", "Home");
        }
    }

}
