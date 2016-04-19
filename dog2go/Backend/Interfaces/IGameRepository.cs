using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dog2go.Backend.Model;

namespace dog2go.Backend.Interfaces
{
    public interface IGameRepository
    {
        void Add(GameTable gameTable);
        void Remove(GameTable deleteGameTable);
        List<GameTable> Get();
    }
}

