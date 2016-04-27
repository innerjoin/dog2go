using System.Threading.Tasks;
using dog2go.Backend.Interfaces;
using dog2go.Backend.Model;
using dog2go.Backend.Repos;
using Microsoft.AspNet.SignalR;

namespace dog2go.Backend.Hubs
{
    [Authorize]
    public abstract class GenericHub : Hub
    {
        protected readonly IGameRepository Games;

        protected GenericHub(IGameRepository repos)
        {
            Games = repos;
        }

        protected GenericHub()
        {
            Games = GameRepository.Instance;
        }

        public override Task OnConnected()
        {
            string userName = Context.User.Identity.Name;
            string connectionId = Context.ConnectionId;
            User user = UserRepository.Instance.Get(userName);

            lock (user.ConnectionIds)
            {
                user.ConnectionIds.Add(connectionId);
                if (user.ConnectionIds.Count == 1)
                {
                    Clients.Others.userConnected(userName);
                }
            }
            return base.OnConnected();
        }
    }
}