using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dog2go.Backend.Model;

namespace dog2go.Backend.Repos
{
    public class UserRepository : IUserRepository
    {
        private readonly List<User> _users;

        private UserRepository()
        {
            _users = new List<User>();
        }
        public static UserRepository Instance { get; } = new UserRepository();

        public void Add(User newUser)
        {
            _users.Add(newUser);
        }

        public void Remove(User deleteUser)
        {
            _users.Remove(deleteUser);
        }

        public List<User> Get()
        {
            return _users;
        }
    }

    public interface IUserRepository
    {
        void Add(User newUser);
        void Remove(User deleteUser);
        List<User> Get();
    }
}
