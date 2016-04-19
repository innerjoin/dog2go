using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly ConnectionRepository<string> Connections = ConnectionRepository<string>.Instance;
        private readonly IChatRepository _repository;

        public ChatHub(IChatRepository repository)
        {
            _repository = repository;
        }

        public override Task OnConnected()
        {
            string name = Context.User.Identity.Name;
            Connections.Add(name, Context.ConnectionId);
            JoinGroup("session_group");
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            string name = Context.User.Identity.Name;

            Connections.Remove(name, Context.ConnectionId);
            LeaveGroup("session_group");
            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            string name = Context.User.Identity.Name;

            if (!Connections.GetConnections(name).Contains(Context.ConnectionId))
            {
                Connections.Add(name, Context.ConnectionId);
            }

            return base.OnReconnected();
        }

        public void SendMessage(string message)
        {
            User sendUser = UserRepository.Instance.Get().Find(user => user.Nickname == Context.User.Identity.Name);
            Message newMessage;
            if (sendUser != null)
            {
                newMessage = new Message() {Msg = message, Group = sendUser.GroupName};
                _repository.AddMessage(newMessage);
            }

            else
            {
                newMessage = new Message() {Msg = message, Group = "session_group"};
                _repository.AddMessage(newMessage);
            }
            
            //var sessionHubContext = GlobalHost.ConnectionManager.GetHubContext<SessionHub>();
            //sessionHubContext.Clients.Group("session_group").broadcastMessage("test_user",message);
            Clients.Group(newMessage.Group).broadcastMessage(Context.User.Identity.Name, message);
            //Clients.All.broadcastMessage("test_user", message);
        }
        public void JoinGroup(string groupName)
        {
            Groups.Add(Context.ConnectionId, groupName);
        }

        private void LeaveGroup(string groupName)
        {
            Groups.Remove(Context.ConnectionId, groupName);
        }
    }
}