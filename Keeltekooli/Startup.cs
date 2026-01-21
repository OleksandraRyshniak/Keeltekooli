using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Keeltekooli.Startup))]
namespace Keeltekooli
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
