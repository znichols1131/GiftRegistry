using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GiftRegistry.WebMVC.Startup))]
namespace GiftRegistry.WebMVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
