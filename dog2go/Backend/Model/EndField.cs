namespace dog2go.Backend.Model
{
    public class EndField : MoveDestinationField
    {
        public EndField(int fieldId) : base(fieldId)
        {
            
        }

        public override string FieldType => this.GetType().ToString();
    }
}
