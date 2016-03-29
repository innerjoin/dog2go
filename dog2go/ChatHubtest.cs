using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;


namespace dog2go
{
    [HubName("chatHub")]
    class ChatHubtest : Hub
    {
        

        public void SendTo(string name, string message)
        {
            // Call the broadcastMessage method to update clients.
            Clients.All.broadcastMessage(name, message);
        }
    }
}

/*public interface IClient
{
    void NewMessage(string message);
}*/
