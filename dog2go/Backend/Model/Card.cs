using System.Collections.Generic;
using System.Drawing;

namespace dog2go.Backend.Model
{
    public class Card
    {
        public string Name { get; set; }
        public int Value { get; set; }
        public Image Picture { get; set; }
        public List<CardAttribute> Attributes { get; set; }

        public Card(string name, int value, Image picture, List<CardAttribute> attributes)
        {
            this.Name = name;
            this.Value = value;
            this.Picture = picture;
            this.Attributes = attributes;
        }
    }
}
