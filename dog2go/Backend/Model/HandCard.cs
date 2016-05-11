using System.Collections.Generic;
using System.Drawing;

namespace dog2go.Backend.Model
{
    public class HandCard : Card
    {
        public bool IsPlayed { get; set; }
        public bool IsValid;

        public HandCard(Card card) : base(card.Name, card.Id, card.ImageIdentifier, card.Attributes)
        {
            IsPlayed = false;
        }

        //public HandCard(string name, int id, List<CardAttribute> attributes) : base(name, id, attributes)
        //{

        //}
    }
}
