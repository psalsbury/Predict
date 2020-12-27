using AutoMapper;
using Predict.App_Start;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Predict.Models;
using Predict.ScheduledTasks;


namespace Predict
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            Mapper.Initialize(c => c.AddProfile<MappingProfile>()); // THIS IS NEEDED FOR AUTOMAPPER TO WORK //
            GlobalConfiguration.Configure(WebApiConfig.Register);
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            InitialiseDb.CreateRolesAndUsers();
            ExecuteTaskServiceCallScheduler.StartAsync().GetAwaiter().GetResult();

        }

    }

}