using System.Drawing;

namespace dog2go.Backend.Model
{
    public class Card
    {
        public string Name { get; set; }
        public int Value { get; set; }
        public Image Picture { get; set; }
        public CardAttribute Attributes { get; set; }
    }
}
