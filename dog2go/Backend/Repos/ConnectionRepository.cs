using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dog2go.Backend.Interfaces;
using dog2go.Backend.Model;

namespace dog2go.Backend.Repos
{
    public class ConnectionRepository : IConnectionRepository
    {
        private readonly ConcurrentDictionary<string, HashSet<string>> _connections;

        private ConnectionRepository()
        {
            _connections = new ConcurrentDictionary<string, HashSet<string>>();
        } 
        public static ConnectionRepository Instance { get; } = new ConnectionRepository();

        public int Count => _connections.Count;

        public HashSet<string> Add(string key, string connection)
        {
            lock (_connections)
            {
                HashSet<string> connectionsSet;
                if (!_connections.TryGetValue(key, out connectionsSet))
                {
                    connectionsSet = new HashSet<string>();
                    _connections.GetOrAdd(key, connectionsSet);
                }

                lock (connectionsSet)
                {
                    connectionsSet.Add(connection);
                }
                return connectionsSet;
            }
        }

        public HashSet<string> GetConnections(string key)
        {
            lock (_connections)
            {
                HashSet<string> connectionsSet;
                if (_connections.TryGetValue(key, out connectionsSet))
                {
                    return connectionsSet;
                }
            }

            return new HashSet<string>();
        }

        public void Remove(string key, string connectionId)
        {
            lock (_connections)
            {
                HashSet<string> connectionSet;
                if (!_connections.TryGetValue(key, out connectionSet))
                {
                    return;
                }

                lock (connectionSet)
                {
                    connectionSet.Remove(connectionId);

                    if (connectionSet.Count == 0)
                    {
                        HashSet<string> removedConnectionSet;
                        _connections.TryRemove(key, out removedConnectionSet);
                    }
                }
            }
        }

        public void Remove(string key)
        {
            lock (_connections)
            {
                HashSet<string> connectionSet;
                _connections.TryRemove(key, out connectionSet);
            }
        }
    }
}
