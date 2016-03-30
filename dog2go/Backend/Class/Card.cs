using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dog2go.Backend.Class
{
    public class Card
    {
        public string Name { get; set; }
        public int Value { get; set; }
        public Image Picture { get; set; }
        public CardAttribute Attributes { get; set; }
    }
}
