using System.Collections.Concurrent;
using System.Threading;
using dog2go.Backend.Model;
using dog2go.Backend.Interfaces;

namespace dog2go.Backend.Repos
{
    public class UserRepository : IUserRepository
    {
        private readonly ConcurrentDictionary<string, User> _users;
        private static readonly object LockObj = new object();
        private static UserRepository _instance;
        private UserRepository()
        {
            _users = new ConcurrentDictionary<string, User>();
        }
        public static UserRepository Instance
        {
            get
            {
                if (_instance != null) return _instance;
                Monitor.Enter(LockObj);
                UserRepository temp = new UserRepository();
                Interlocked.Exchange(ref _instance, temp);
                Monitor.Exit(LockObj);
                return _instance;
            }
        }

        public User Get(string userName)
        {
            lock (_users)
            {
                User user;
                _users.TryGetValue(userName, out user);
                return user;
            }
        }

        public User GetOrAdd(string userName, User user)
        {
            lock (_users)
            {
                return _users.GetOrAdd(userName, user);
            }
        }

        public User Remove(string userName)
        {
            lock (_users)
            {
                User user;
                _users.TryRemove(userName, out user);
                return user;
            }
        }

        public ConcurrentDictionary<string, User> Get()
        {
            return _users;
        }
    }
}
