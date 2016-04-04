﻿using System.Collections.Generic;

namespace dog2go.Backend.Model
{
    class PlayRound
    {
        public List<HandCard> Cards { get; set; }
        public int RoundNumber { get; set; }
        public int CardCounter { get; set; }
        public List<CardMove> CardMoves { get; set; }
    }
}