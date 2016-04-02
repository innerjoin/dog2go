namespace dog2go.Backend.Model
{
    public class StartField : MoveDestinationField
    {
        public ColorCode ColorCode { get; set; }
        public EndField EndFieldEntry { get; set; }

        public StartField(int fieldId, ColorCode colorCode) : base(fieldId)
        {
            ColorCode = colorCode;
        }
    }
}
