using Microsoft.Owin;
using Owin;
using Predict;

[assembly: OwinStartup(typeof(Startup))]

namespace Predict
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Startup Configuration - Start");
            Helper.Cache.SetEventCache();
            Helper.Cache.SetNextResultCheckDateTime(false);
        }
    }
}