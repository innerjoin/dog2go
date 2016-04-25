using System.Collections.Generic;
using System.Drawing;

namespace dog2go.Backend.Model
{
    public class Card
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public List<CardAttribute> Attributes { get; set; }

        public Card(string name, int id, List<CardAttribute> attributes)
        {
            this.Name = name;
            this.Id = id;
            this.Attributes = attributes;
        }
    }
}
