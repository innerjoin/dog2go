using System.Collections.Generic;
using System.Drawing;

namespace dog2go.Backend.Model
{
    public class HandCard : Card
    {
        public bool IsPlayed { get; set; }

        public HandCard(string name, int id, List<CardAttribute> attributes) : base(name, id, attributes)
        {

        }
    }
}
