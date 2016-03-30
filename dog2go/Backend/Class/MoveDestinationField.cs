using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dog2go.Backend.Class
{
    public abstract class MoveDestinationField
    {
        public int Identifier { get; set; }
        // generate identifier on run time (UUID)
        public Meeple CurrentMeeple { get; set; }
        public MoveDestinationField Previous { get; set; }
        public MoveDestinationField Next { get; set; }
    }
}
