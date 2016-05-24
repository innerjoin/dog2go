using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace dog2go.Backend.Model
{
    public class PlayerFieldArea
    {
        private const int NumberOfMeeple = 4; // defines also the number of kennels and end fields
        private const int NumberOfFieldsBeforeStart = 4;
        private const int NumberOfFieldsAfterStart = 11;

        private PlayerFieldArea _previous;
        private PlayerFieldArea _next;

        public int PreviousIdentifier { get; private set; }
        public int NextIdentifier { get; private set; }

        public int Identifier { get; }
        public int FieldId { get; private set; }

        public ColorCode ColorCode { get; }
        public List<MoveDestinationField> Fields { get; private set; }
        public List<KennelField> KennelFields { get; private set; }
        public List<EndField> EndFields { get; private set; }
        public StartField StartField { get; private set; }
        public List<Meeple> Meeples { get; private set; }
        public Participation Participation { get; set; }
        [IgnoreDataMember]
        public PlayerFieldArea Previous
        {
            get { return _previous; }
            set
            {
                _previous = value;
                if (value == null) return;
                PreviousIdentifier = value.Identifier;
                value.Fields.Last(field => field.FieldType.Contains("StandardField")).Next = Fields.First(field => field.FieldType.Contains("StandardField"));
                Fields.First(field => field.FieldType.Contains("StandardField")).Previous = value.Fields.Last(field => field.FieldType.Contains("StandardField"));
            }
        }

        [IgnoreDataMember]
        public PlayerFieldArea Next
        {
            get { return _next; }
            set
            {
                _next = value;
                if (value == null) return;
                NextIdentifier = value.Identifier;
                value.Fields.First(field => field.FieldType.Contains("StandardField")).Previous = Fields.Last(field => field.FieldType.Contains("StandardField"));
                Fields.Last(field => field.FieldType.Contains("StandardField")).Next = value.Fields.First(field => field.FieldType.Contains("StandardField"));
            }
        }

        public PlayerFieldArea(int identifier, ColorCode colorCode, int fieldId)
        {
            Identifier = identifier;
            ColorCode = colorCode;
            FieldId = fieldId;
            GeneratePlayerFieldArea(fieldId);
        }

        private void GeneratePlayerFieldArea(int fieldId)
        {
            List<Meeple> meepleList = new List<Meeple>();
            List<KennelField> kennelFields = new List<KennelField>();
            List<EndField> endFields = new List<EndField>();

            for (int count = 0; count < NumberOfMeeple; count++)
            {
                KennelField kennelField = new KennelField(++fieldId);
                int meepleId = (Identifier - 1) * NumberOfMeeple + count;
                Meeple newMeeple = new Meeple(kennelField, ColorCode, meepleId) {IsStartFieldBlocked = true};
                meepleList.Add(newMeeple);
                kennelField.CurrentMeeple = newMeeple;
                kennelFields.Add(kennelField);
            }

            List<MoveDestinationField> fields = new List<MoveDestinationField>();
            StandardField standardField = new StandardField(++fieldId) { Previous = null };

            for (int count = 1; count < NumberOfFieldsBeforeStart; count++)
            {
                StandardField tempField = new StandardField(++fieldId) { Previous = standardField };
                standardField.Next = tempField;
                fields.Add(standardField);
                standardField = tempField;
            }

            StartField startField = new StartField(++fieldId, ColorCode);
            standardField.Next = startField;
            startField.Previous = standardField;

            fields.Add(standardField);

            StandardField standardFieldAfter = new StandardField(++fieldId) { Previous = startField };
            startField.Next = standardFieldAfter;
            fields.Add(startField);
            StartField = startField;

            for (int count = 1; count < NumberOfFieldsAfterStart; count++)
            {
                StandardField tempField = new StandardField(++fieldId) { Previous = standardFieldAfter };
                standardFieldAfter.Next = tempField;
                fields.Add(standardFieldAfter);
                standardFieldAfter = tempField;
            }

            fields.Add(standardFieldAfter);

            EndField endField = new EndField(++fieldId) { Previous = startField };
            startField.EndFieldEntry = endField;

            for (int count = 1; count < NumberOfMeeple; count++)
            {
                EndField tempEndField = new EndField(++fieldId) { Previous = endField };
                endField.Next = tempEndField;
                fields.Add(endField);
                endFields.Add(endField);
                endField = tempEndField;
            }

            fields.Add(endField);
            endFields.Add(endField);
            Meeples = meepleList;
            KennelFields = kennelFields;
            EndFields = endFields;
            Fields = fields;
            FieldId = fieldId;
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