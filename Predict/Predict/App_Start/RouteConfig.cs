using System.Web.Mvc;
using System.Web.Routing;

namespace Predict
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "EnterFixturePredictions", // Route name
                "FixturePredictions/FixturePredictions/{eventId}", // URL with parameters
                new {controller = "FixturePredictions", action = "FixturePredictions"} // Parameter default
            );

            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new {controller = "Home", action = "Index", id = UrlParameter.Optional}
            );
        }
    }
}