namespace dog2go.Backend.Model
{
    public class Meeple
    {
        public Meeple() { }
        
        public int Identifier { get; set; }

        public ColorCode ColorCode { get; set; }

        public MoveDestinationField CurrentPosition;

        public int CurrentFieldId;

        public bool IsStartFieldBlocked { get; set; }

        public Meeple(KennelField kennelField)
        {
            CurrentPosition = kennelField;
        }
        public Meeple(KennelField kennelField, ColorCode colorCode) : this(kennelField)
        {
            ColorCode = colorCode;
        }

        public Meeple(KennelField kennelField, ColorCode colorCode, int identifier) : this(kennelField, colorCode)
        {
            Identifier = identifier;
        }

    }
}
