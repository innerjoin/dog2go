using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using dog2go.Backend.Interfaces;
using dog2go.Backend.Model;
using dog2go.Backend.Repos;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using User = dog2go.Backend.Model.User;

namespace dog2go.Backend.Hubs
{
    [HubName("chatHub")]
    public class ChatHub : Hub
    {
        private readonly IChatRepository _repository;

        public ChatHub(IChatRepository repository)
        {
            _repository = repository;
        }

        public void SendMessage(Message message)
        {
            User sendUser = UserRepository.Instance.Get().Find(user => user.Nickname == message.User.Nickname);
            if (message.Group == null)
            {
                message.Group = sendUser.GroupName;
            }
            _repository.AddMessage(message);
            var gameHubContext = GlobalHost.ConnectionManager.GetHubContext<GameHub>();
            gameHubContext.Clients.Group(message.Group).broadcastMessage(message);
        }
    }
}