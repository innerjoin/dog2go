namespace dog2go.Backend.Model
{
    public abstract class MoveDestinationField
    {
        public int Identifier { get; set; }
        // generate identifier on run time (UUID)
        public Meeple CurrentMeeple { get; set; }
        public MoveDestinationField Previous { get; set; }
        public MoveDestinationField Next { get; set; }
    }
}
