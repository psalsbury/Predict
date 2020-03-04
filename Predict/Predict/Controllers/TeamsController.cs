using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using Predict.Models;

namespace Predict.Controllers
{
    public class TeamsController : Controller
    {
        private ApplicationDbContext _context;
        public TeamsController()
        {
            _context = new ApplicationDbContext();
        }
        
        //GET : Teams
        public ActionResult Index()
        {
            var teams = _context.Teams.ToList();
            return View(teams);
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
            base.Dispose(disposing);
        }
    }
}
