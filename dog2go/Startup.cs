using System;
using dog2go.Backend.Hubs;
using dog2go.Backend.Repos;
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
            GlobalHost.DependencyResolver.Register(typeof(GameHub), () => new GameHub(UserRepository.Instance));
            app.MapSignalR(new HubConfiguration { EnableDetailedErrors = true });
#else
            Console.WriteLine("Hello this is Release Mode!");
            GlobalHost.DependencyResolver.Register(typeof(GameHub), () => new GameHub(UserRepository.Instance));
            app.MapSignalR();
#endif
            ConfigureAuth(app);

            
        }
    }
}
