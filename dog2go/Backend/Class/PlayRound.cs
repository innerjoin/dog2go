using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dog2go.Backend.Class
{
    class PlayRound
    {
        public List<HandCard> Cards { get; set; }
        public int RoundNumber { get; set; }
        public int CardCounter { get; set; }
        public List<CardMove> CardMoves { get; set; }
    }
}
