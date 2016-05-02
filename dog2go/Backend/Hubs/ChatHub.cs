using System.Linq;
using System.Threading.Tasks;
using dog2go.Backend.Interfaces;
using dog2go.Backend.Model;
using dog2go.Backend.Constants;
using dog2go.Backend.Repos;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace dog2go.Backend.Hubs
{
    [Authorize]
    [HubName("chatHub")]
    public class ChatHub : GenericHub
    {
        private readonly IChatRepository _chatRepository;
        private readonly IConnectionRepository _connectionRepository;
        public ChatHub(IConnectionRepository connectionRepository, IChatRepository chatRepository)
        {
            _connectionRepository = connectionRepository;
            _chatRepository = chatRepository;
        }
        public override Task OnConnected()
        {
            JoinGroup(GlobalDefinitions.GroupName);
            //string userName = Context.User.Identity.Name;
            SendMessage(ServerMessages.JoinedGame);
            return base.OnConnected();
        }

        //public override Task OnDisconnected(bool stopCalled)
        //{
        //    string userName = Context.User.Identity.Name;

        //    HashSet<string> connections;
        //    connections = _connectionRepository.GetConnections(userName);

        //    if (connections != null)
        //    {

        //        lock (connections)
        //        {

        //            connections.RemoveWhere(cid => cid.Equals(Context.ConnectionId));

        //            if (!connections.Any())
        //            {

        //                HashSet<string> removedConnectionSet;
        //                _connectionRepository.Remove(userName);

        //                // Could be used to show other users that a new has been connected
        //                //Clients.Others.userConnected(userName);
        //            }
        //        }
        //    }

        //    LeaveGroup("session_group");

        //    return base.OnDisconnected(stopCalled);
        //}

        //private HashSet<string> GetConnections(string username)
        //{
        //    return _connectionRepository.GetConnections(username); ;
        //}

        //public override Task OnReconnected()
        //{
        //    string name = Context.User.Identity.Name;

        //    if (!_connectionRepository.GetConnections(name).Contains(Context.ConnectionId))
        //    {
        //        _connectionRepository.Add(name, Context.ConnectionId);
        //    }

        //    return base.OnReconnected();
        //}

        public void SendMessage(string message)
        {
            User sendUser = UserRepository.Instance.Get().FirstOrDefault(u => u.Value.Nickname != Context.User.Identity.Name).Value;
            Message newMessage;
            if (sendUser != null)
            {
                newMessage = new Message() {Msg = message, Group = sendUser.GroupName};
                _chatRepository.AddMessage(newMessage);
            }

            else
            {
                newMessage = new Message() {Msg = message, Group = GlobalDefinitions.GroupName };
                _chatRepository.AddMessage(newMessage);
            }

            Clients.Group(newMessage.Group).broadcastMessage(Context.User.Identity.Name, message);
        }

        private void JoinGroup(string groupName)
        {
            Groups.Add(Context.ConnectionId, groupName);
        }

        //private void LeaveGroup(string groupName)
        //{
        //    Groups.Remove(Context.ConnectionId, groupName);
        //}
    }
}