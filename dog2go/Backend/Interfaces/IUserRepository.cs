using System.Collections.Concurrent;
using dog2go.Backend.Model;

namespace dog2go.Backend.Interfaces
{
    public interface IUserRepository
    {
        User Get(string userName);
        User GetOrAdd(string userName, User user);
        User Remove(string userName);
        ConcurrentDictionary<string, User> Get();
    }
}
