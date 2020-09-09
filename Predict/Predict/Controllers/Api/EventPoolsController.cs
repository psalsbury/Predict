using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Predict.Controllers.Api
{
    public class EventPoolsController : Controller
    {
        // GET: EventPools
        public ActionResult EventPoolsIndex()
        {
            return View();
        }
    }
}