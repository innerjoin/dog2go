using System;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(dog2go.Startup))]
namespace dog2go
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
#if DEBUG
            Console.WriteLine("Hello this is Debug Mode!");
            app.MapSignalR(new HubConfiguration { EnableDetailedErrors = true });
#else
            Console.WriteLine("Hello this is Release Mode!");
            app.MapSignalR();
#endif
            ConfigureAuth(app);

            
        }
    }
}
