using System.Collections.Generic;

namespace dog2go.Backend.Model
{
    public class PlayRound
    {
        public List<HandCard> Cards { get; set; }
        public int RoundNumber { get; set; }
        public int CardCounter { get; set; }
        public List<CardMove> CardMoves { get; set; }
        public PlayRound(int roundNumber, int cardCounter)
        {
            RoundNumber = roundNumber;
            CardCounter = cardCounter;
        }
    }
}
