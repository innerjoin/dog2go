using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using dog2go.Backend.Interfaces;
using dog2go.Backend.Model;
using dog2go.Backend.Repos;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace dog2go.Backend.Hubs
{
    [HubName("sessionHub")]
    public class SessionHub : Hub
    {
        private static readonly ConnectionRepository<string> Connections = ConnectionRepository<string>.Instance;
        public IUserRepository SessionUserRepository { get; set; }

        public SessionHub(IUserRepository repository)
        {
            SessionUserRepository = repository;
        }
        public SessionHub()
        {
            SessionUserRepository = UserRepository.Instance;
        }
        public override Task OnConnected()
        {
            Connections.Add(Context.User.Identity.Name, Context.ConnectionId);
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            Connections.Remove(Context.User.Identity.Name, Context.ConnectionId);
            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {

            if (!Connections.GetConnections(Context.User.Identity.Name).Contains(Context.ConnectionId))
            {
                Connections.Add(Context.User.Identity.Name, Context.ConnectionId);
            }

            return base.OnReconnected();
        }

        public void Login(string name, string cookie)
        {
            if (string.IsNullOrEmpty(cookie))
            {
                string sessionCookie = "dog2go=" + name + ";expires=" + new DateTime().AddSeconds(24 * 60 * 60).ToString("d", CultureInfo.CurrentCulture);
                Clients.Client(Context.ConnectionId).newSession(sessionCookie);
                User newUser = new User(name, Context.ConnectionId) { Cookie = sessionCookie };
                SessionUserRepository.Add(newUser);

                Clients.Client(Context.ConnectionId).updateOpenGames(GameRepository.Instance.Get().Find(game => game.Participations.Count < 4));
            }

            else
            {
                foreach (var table in GameRepository.Instance.Get().Where(table => table.Participations.Any(participation => participation.Participant.Cookie == cookie)))
                {
                    Clients.Client(Context.ConnectionId).backToGame(table, table.Participations.Find(participation => participation.Participant == SessionUserRepository.Get().Find(user => user.Identifier == Context.ConnectionId)).ActualPlayRound.Cards);
                }
            }
        }

        public void Logout(string name)
        {
            //LeaveGroup("session_group");
            SessionUserRepository.Remove(SessionUserRepository.Get().Find(user => user.Nickname == name));
        }

    }
}