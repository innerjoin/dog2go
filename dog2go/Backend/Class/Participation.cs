using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dog2go.Backend.Class
{
    public class Participation
    {
        public User Participant { get; set; }
        public User Partner { get; set; }
        private GameTable Table { get; set; }
    }
}
