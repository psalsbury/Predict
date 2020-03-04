using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Predict.Models;

namespace Predict.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {            
                Predict.Helper.SessionHelper.SetUserSessionVariables(Session, User.Identity.GetUserId());
            }
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Rules()
        {
            ViewBag.Message = "Rules.";
            ViewBag.EventName = Predict.Helper.Cache.GetEventName();

            return View();
        }
    }
}