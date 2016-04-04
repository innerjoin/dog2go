using System;
using System.Drawing;
using System.Web.Services.Description;
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
            var hubConfiguration = new HubConfiguration();
            hubConfiguration.EnableDetailedErrors = true;
            app.MapSignalR(hubConfiguration);
#else
            Console.WriteLine("Hello this is Release Mode!");
            // Any connection or hub wire up and configuration should go here
            //app.MapSignalR();
#endif
            ConfigureAuth(app);

            
        }
    }
}
