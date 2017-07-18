using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(businessProBms.Startup))]
namespace businessProBms
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
