using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
namespace dog2go.Backend.Class
{
    public class PlayerFieldArea
    {
        private int _previousIdentifier;
        private int _nextIdentifier;
        public int Identifier { get; }
        private PlayerFieldArea _previous;
        private PlayerFieldArea _next;
        public ColorCode ColorCode { get; set; }
        public List<MoveDestinationField> Fields { get; set; }
        public List<Meeple> Meeples { get; set; }
        public Participation Participation { get; set; }
        [IgnoreDataMember]
        public PlayerFieldArea Previous {
            get { return _previous;}
            set
            {
                _previous = value;
                _previousIdentifier = value.Identifier;
            }
        }
        [IgnoreDataMember]
        public PlayerFieldArea Next { 
            get { return _next; }
            set
            {
                _next = value;
                _nextIdentifier = value.Identifier;
            }
        }

        public PlayerFieldArea(int identifier)
        {
            Identifier = identifier;
        }
    }
}

public enum ColorCode
{
    Red = 0xff0000,
    Blue = 0x0000ff,
    Green = 0x00ff00,
    Yellow = 0xedc613
}