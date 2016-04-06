using System;

namespace dog2go.Backend.Model
{
    public class StandardField : MoveDestinationField
    {
        public StandardField(int fieldId) : base(fieldId)
        {

        }

        public override string FieldType => this.GetType().ToString();
    }
}
