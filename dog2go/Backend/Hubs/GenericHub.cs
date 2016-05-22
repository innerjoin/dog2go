using System;
using System.Threading.Tasks;
using dog2go.Backend.Constants;
using dog2go.Backend.Interfaces;
using dog2go.Backend.Model;
using dog2go.Backend.Repos;
using Microsoft.AspNet.SignalR;

namespace dog2go.Backend.Hubs
{
    [Authorize]
    public abstract class GenericHub : Hub
    {
        private static readonly object Locker = new object();

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
            string tableId = Context.QueryString["tableId"];
            string userName = Context.User.Identity.Name;
            string connectionId = Context.ConnectionId;
            User user = UserRepository.Instance.Get(userName);

            if (user != null)
            {
                lock (Locker)
                {
                    if (user.ConnectionIds.Add(connectionId))
                    {
                        IHubContext context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
                        Task test = context.Groups.Add(Context.ConnectionId, tableId);
                        test.Wait();
                        context.Clients.Group(tableId)
                            .broadcastSystemMessage(ServerMessages.JoinedGame.Replace("{0}", Context.User.Identity.Name), tableId);
                    }
                }
            }
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            Console.WriteLine(stopCalled
                ? $"Client {Context.ConnectionId} explicitly closed the connection."
                : $"Client {Context.ConnectionId} timed out .");

            return base.OnDisconnected(stopCalled);
        }
    }
}