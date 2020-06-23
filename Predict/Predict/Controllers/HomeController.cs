using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Predict.Helper;

namespace Predict.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
                SessionHelper.SetUserSessionVariables(Session, User.Identity.GetUserId());
            return View();
        }

        public ActionResult ContactUs()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendMessage()
        {
            var message = Request["message"];
            var from = User.Identity.Name;

            var emailMesesage = new IdentityMessage
            {
                Body = message,
                Destination = "pete@salsbury.co.uk",
                Subject = string.Format("Query from {0}", from ?? "Unknown")
            };

            Cache.SendEmail(emailMesesage);
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Rules()
        {
            ViewBag.Message = "Rules.";
            ViewBag.EventName = "Prediction Competition";

            return View();
        }
    }
}