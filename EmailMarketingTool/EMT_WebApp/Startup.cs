using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EMT_WebApp.Startup))]
namespace EMT_WebApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
