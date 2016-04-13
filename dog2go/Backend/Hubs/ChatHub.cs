using System;
using System.Threading.Tasks;
using System.Web;
using dog2go.Backend.Interfaces;
using dog2go.Backend.Model;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using User = dog2go.Backend.Model.User;

namespace dog2go.Backend.Hubs
{
    [HubName("chatHub")]
    public class ChatHub : Hub
    {
        private IChatRepository _repository;

        public ChatHub(IChatRepository repository)
        {
            _repository = repository;
        }

        public void SendMessage(Message message)
        {
            Clients.All.broadcastMessage("System:", message);
        }
    }
}