using System;
using System.Collections.Generic;

namespace dog2go.Backend.Model
{
    public class GameTable
    {
        public string Name { get; set; }

        public int Identifier { get; set; }

        public DateTime Start { get; set; }

        public DateTime Stop { get; set; }
        public List<PlayerFieldArea> PlayerFieldAreas;

        public GameTable(List<PlayerFieldArea> areas, int id)
        {
            PlayerFieldAreas = areas;
            Identifier = id;
        }
    }
}
