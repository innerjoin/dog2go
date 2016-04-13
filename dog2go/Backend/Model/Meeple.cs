namespace dog2go.Backend.Model
{
    public class Meeple
    {
        public ColorCode ColorCode { get; set; }

        public MoveDestinationField CurrentPosition;

        public bool IsStartFieldBlocked { get; set; }

        public Meeple(KennelField kennelField)
        {
            CurrentPosition = kennelField;
        }
        public Meeple(KennelField kennelField, ColorCode colorCode) : this(kennelField)
        {
            ColorCode = colorCode;
        }

    }
}
