using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dog2go.Backend.Model;

namespace dog2go.Backend.Interfaces
{
    public interface IConnectionRepository
    {
        HashSet<string> Add(string key, string connectionId);
        HashSet<string> GetConnections(string key);
        void Remove(string key, string connectionId);
        void Remove(string key);
    }
}

