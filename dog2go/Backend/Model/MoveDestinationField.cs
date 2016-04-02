using System.Runtime.Serialization;
namespace dog2go.Backend.Model
{
    public abstract class MoveDestinationField
    {
        private int _previousIdentifier;
        private int _nextIdentifier;
        private MoveDestinationField _previous;
        private MoveDestinationField _next;
        public int PreviousIdentifier => _previousIdentifier;
        public int NextIdentifier => _nextIdentifier;
        public int Identifier { get; }
        // generate identifier on run time (UUID)
        public Meeple CurrentMeeple { get; set; }

        [IgnoreDataMember]
        public MoveDestinationField Previous
        {
            get { return _previous; }
            set
            {
                _previous = value;
                if (value != null)
                {
                    _previousIdentifier = value.Identifier;
                }
            }
        }

        [IgnoreDataMember]
        public MoveDestinationField Next
        {
            get { return _next; }
            set
            {
                _next = value;
                if (value != null)
                {
                    _nextIdentifier = value.Identifier;
                }
            }
        }

        protected MoveDestinationField(int fieldId)
        {
            Identifier = fieldId;
        }
    }
}
