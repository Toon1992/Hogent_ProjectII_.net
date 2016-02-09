using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DidactischeLeermiddelen.Startup))]
namespace DidactischeLeermiddelen
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
