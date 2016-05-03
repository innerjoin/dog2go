using System.Collections.Generic;
using System.Drawing;

namespace dog2go.Backend.Model
{
    public class Card
    {
        public Card() { }
        public string Name { get; set; }
        public int Id { get; set; }
        public string ImageIdentifier { get; private set; }
        public List<CardAttribute> Attributes { get; set; }

        public Card(string name, int id, string imageIdentifier, List<CardAttribute> attributes)
        {
            Name = name;
            Id = id;
            ImageIdentifier = imageIdentifier;
            Attributes = attributes;
        }
    }
}
