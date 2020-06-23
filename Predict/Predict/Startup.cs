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
        }
    }
}