using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using dog2go.Backend.Model;
using dog2go.Backend.Interfaces;

namespace dog2go.Backend.Repos
{
    public class GameRepository : IGameRepository
    {
        private readonly List<GameTable> _gameTables;
        private static GameRepository _instance;
        private static readonly Object LockObj = new Object();

        private GameRepository()
        {
            _gameTables = new List<GameTable>();
        }

        public static GameRepository Instance
        {
            get
            {
                if (_instance != null) return _instance;
                Monitor.Enter(LockObj);
                GameRepository temp = new GameRepository();
                Interlocked.Exchange(ref _instance, temp);
                Monitor.Exit(LockObj);
                return _instance;
            }
        }

        public void Add(GameTable newGameTable)
        {
            lock (_gameTables)
            {
                _gameTables.Add(newGameTable);
            }
        }

        public void Remove(GameTable deleteGameTable)
        {
            lock (_gameTables)
            {
                _gameTables.Remove(deleteGameTable);
            }
        }

        public List<GameTable> Get()
        {
            lock (_gameTables)
            {
                return _gameTables;
            }
        }
    }
}
