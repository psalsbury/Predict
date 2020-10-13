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
                new { controller = "FixturePredictions", action = "FixturePredictions" } // Parameter default
            );

            routes.MapRoute(
                "EnterKOFixturePredictionsGrouped", // Route name
                "KoFixturePredictions/KoFixturePredictionsGrouped/{eventId}", // URL with parameters
                new { controller = "KoFixturePredictions", action = "KoFixturePredictionsGrouped" } // Parameter default
            );

            routes.MapRoute(
                "BonusQuestionPredictions", // Route name
                "BonusQuestionPredictions/BonusQuestionPredictions/{eventId}", // URL with parameters
                new { controller = "BonusQuestionPredictions", action = "BonusQuestionPredictions" } // Parameter default
            );

            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}