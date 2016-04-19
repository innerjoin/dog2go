using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dog2go.Backend.Repos;

namespace dog2go.Backend.Model
{
    public static class SessionUtil
    {
        public static bool Login(string identifier, string name, string cookie)
        {
            if (cookie == null)
            {
                string sessionCookie = "dog2go=" + name + ";expires=" + new DateTime().AddSeconds(24 * 60 * 60).ToString("d", CultureInfo.CurrentCulture);
                //Clients.Client(Context.ConnectionId).newSession(sessionCookie);
                User newUser = new User(name, identifier) { Cookie = sessionCookie };
                UserRepository.Instance.Add(newUser);

                //Clients.Client(Context.ConnectionId).updateOpenGames(GameRepository.Instance.Get());
                return false;
            }

            else
            {
                if (UserRepository.Instance.Get().Find(user => user.Identifier == identifier)== null)
                {
                    User newUser = new User(name, identifier) { Cookie = cookie };
                    UserRepository.Instance.Add(newUser);
                }
                return true;
                
            }
        }

    }
}
