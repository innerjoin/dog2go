﻿namespace dog2go.Backend.Model
{
    public class KennelField : MoveDestinationField
    {
        public KennelField(int fieldId) : base(fieldId)
        {

        }

        public override string FieldType => this.GetType().ToString();
    }
}
