using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dog2go.Backend.Class
{
    public class PlayerFieldArea
    {
        public string ColorCode { get; set; }
    
        public List<MoveDestinationField> Fields { get; set; }
        public List<Meeple> Meeples { get; set; }
        public Participation Participation { get; set; }
        public PlayerFieldArea Previous { get; set; }
        public PlayerFieldArea Next { get; set; }
    }
}
