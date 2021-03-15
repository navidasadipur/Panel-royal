using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SpadStorePanel.Web.Startup))]
namespace SpadStorePanel.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
