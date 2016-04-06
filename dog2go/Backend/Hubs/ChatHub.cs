using System;
using System.Threading.Tasks;
using System.Web;
using dog2go.Backend.Model;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace dog2go.Backend.Hubs
{
    [HubName("chatHub")]
    public class ChatHub : Hub
    {
        public void SendTo(string name, string message)
        {
            // Call the broadcastMessage method to update clients.
            Clients.All.broadcastMessage(name, message);   
        }

        public async Task JoinRoom(string roomName)
        {
            await Groups.Add(Context.ConnectionId, roomName);
            Clients.Group(roomName).addChatMessage(Context.User.Identity.Name + " joined.");
            //Clients.OthersInGroup(groupName).addChatMessage(name, message);
        }

        public Task LeaveRoom(string roomName)
        {
            return Groups.Remove(Context.ConnectionId, roomName);
        }

        public void SendMessage(string message)
        {
            Clients.All.broadcastMessage("System:", message);
        }

        public void SendMessageTo(string message, User recipientUser)
        {
            Clients.Group(recipientUser.GroupName).addChatMessage(message);
        }


        /*public void SendMessageFrom(User sender, string message)
        {
            Clients.Others.addChatMessage(sender.Nickname, message);
        }*/

        public void SendMessageFrom(string sender, string message)
        {
            Clients.Others.addChatMessage(sender, message);
        }

        public string ReceiveMessage(User user)
        {
            return "";
        }
    }
}