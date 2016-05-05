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
            SendMessage(ServerMessages.JoinedGame);
            return base.OnConnected();
        }

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
    }
}