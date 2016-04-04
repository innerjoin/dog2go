using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dog2go.Interfaces;

namespace dog2go.Backend.Class
{
    public class Meeple
    {
        public ColorCode ColorCode { get; set; }

        public MoveDestinationField CurrentPosition { get; set; }

    }
}
