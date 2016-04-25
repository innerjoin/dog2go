using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dog2go.Backend.Model;
using dog2go.Backend.Interfaces;

namespace dog2go.Backend.Repos
{
    public class UserRepository : IUserRepository
    {
        private readonly ConcurrentDictionary<string, User> _users;

        private UserRepository()
        {
            _users = new ConcurrentDictionary<string, User>();
        }
        public static UserRepository Instance { get; } = new UserRepository();

        public User Get(string userName)
        {
            User user;
            _users.TryGetValue(userName, out user);
            return user;
        }

        public User GetOrAdd(string userName, User user)
        {
            return _users.GetOrAdd(userName, user);
        }

        public User Remove(string userName)
        {
            User user;
            _users.TryRemove(userName, out user);
            return user;
        }

        public ConcurrentDictionary<string, User> Get()
        {
            return _users;
        }
    }
}
