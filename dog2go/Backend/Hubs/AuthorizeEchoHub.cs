using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;

namespace dog2go.Backend.Hubs
{
    [Authorize]
    public class AuthorizeEchoHub : Hub
    {
        public override Task OnConnected()
        {
            return Clients.Caller.hubReceived("Welcome " + Context.User.Identity.Name + "!");
        }

        public void Echo(string value)
        {
            Clients.Caller.hubReceived(value);
        }
    }
}
