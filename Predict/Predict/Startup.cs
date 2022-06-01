using System;
using Microsoft.Owin;
using Owin;
using Predict;
using Predict.RapidApi;

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
            RapidApiHelper.SetNextResultCheckDateTime(false, false);
        }
    }
}