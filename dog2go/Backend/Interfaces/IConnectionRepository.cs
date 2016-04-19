using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dog2go.Backend.Model;

namespace dog2go.Backend.Interfaces
{
    public interface IConnectionRepository<T>
    {
        void Add(T key, string connectionId);
        IEnumerable<string> GetConnections(T key);
        void Remove(T key, string connectionId);
    }
}

