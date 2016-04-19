using System;
using dog2go.Backend.Connection;
using dog2go.Backend.Hubs;
using dog2go.Backend.Model;
using dog2go.Backend.Repos;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;

[assembly: OwinStartupAttribute(typeof(dog2go.Startup))]
namespace dog2go
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            GlobalHost.DependencyResolver.Register(typeof(ChatHub), () => new ChatHub(ChatMessageRepository.Instance));
            GlobalHost.DependencyResolver.Register(typeof(GameHub), () => new GameHub(GameRepository.Instance));
            app.UseCors(CorsOptions.AllowAll);
            ConfigureAuth(app);

            app.MapSignalR<AuthorizeEchoConnection>("/echo");
#if DEBUG
            Console.WriteLine("Hello this is Debug Mode!");
            app.MapSignalR(new HubConfiguration { EnableDetailedErrors = true});
#else
            Console.WriteLine("Hello this is Release Mode!");
            GlobalHost.DependencyResolver.Register(typeof(GameHub), () => new GameHub(UserRepository.Instance));
            app.MapSignalR();
#endif

        }
    }
}
