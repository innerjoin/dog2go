using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dog2go.Backend.Model;
using dog2go.Backend.Interfaces;

namespace dog2go.Backend.Repos
{
    public class GameRepository : IGameRepository
    {
        private readonly List<GameTable> _gameTables;

        private GameRepository()
        {
            _gameTables = new List<GameTable>();
        }
        public static GameRepository Instance { get; } = new GameRepository();

        public void Add(GameTable newGameTable)
        {
            _gameTables.Add(newGameTable);
        }

        public void Remove(GameTable deleteGameTable)
        {
            _gameTables.Remove(deleteGameTable);
        }

        public List<GameTable> Get()
        {
            return _gameTables;
        }
    }
}
