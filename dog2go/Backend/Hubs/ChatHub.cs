using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace dog2go.Backend.Hubs
{
    [Authorize]
    [HubName("chatHub")]
    public class ChatHub : GenericHub
    {
        public void SendMessage(string message, int tableId)
        {
            Clients.Group(tableId.ToString()).broadcastMessage(Context.User.Identity.Name, message, tableId);
        }

        public void SendSystemMessage(string message, int tableId)
        {
            if (message.IsNullOrWhiteSpace()) return;
            Clients.Group(tableId.ToString()).broadcastSystemMessage(message, tableId);
        }
    }
}