using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web.UI.WebControls;

namespace dog2go.Backend.Model
{
    public class PlayerFieldArea
    {
        private const int NumberOfMeeple = 4;
        private const int NumberOfFieldsBeforeStart = 4;
        private const int NumberOfFieldsAfterStart = 11;

        private int _previousIdentifier;
        private int _nextIdentifier;
        public int PreviousIdentifier => _previousIdentifier;
        public int NextIdentifier => _nextIdentifier;
        public int Identifier { get; }
        public int FieldId { get; private set; }
        private PlayerFieldArea _previous;
        private PlayerFieldArea _next;
        public ColorCode ColorCode { get; set; }
        public List<MoveDestinationField> Fields { get; set; }
        public List<KennelField> KennelFields { get; set; }
        public List<EndField> EndFields { get; set; }
        public StartField StartField { get; set; }
        public List<Meeple> Meeples { get; set; }
        public Participation Participation { get; set; }
        [IgnoreDataMember]
        public PlayerFieldArea Previous
        {
            get { return _previous; }
            set
            {
                _previous = value;
                if (value == null) return;
                _previousIdentifier = value.Identifier;
                value.Fields.Last(field => field.FieldType.Contains("StandardField")).Next = this.Fields.First(field => field.FieldType.Contains("StandardField"));
                this.Fields.First(field => field.FieldType.Contains("StandardField")).Previous = value.Fields.Last(field => field.FieldType.Contains("StandardField"));
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
                _nextIdentifier = value.Identifier;
                value.Fields.First(field => field.FieldType.Contains("StandardField")).Previous = this.Fields.Last(field => field.FieldType.Contains("StandardField"));
                this.Fields.Last(field => field.FieldType.Contains("StandardField")).Next = value.Fields.First(field => field.FieldType.Contains("StandardField"));
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

            for (var count = 0; count < NumberOfMeeple; count++)
            {
                KennelField kennelField = new KennelField(++fieldId);
                meepleList.Add(new Meeple(kennelField, this.ColorCode) {IsStartFieldBlocked = true});
                kennelFields.Add(kennelField);
            }

            List<MoveDestinationField> fields = new List<MoveDestinationField>();
            StandardField standardField = new StandardField(++fieldId) { Previous = null };

            for (var count = 1; count < NumberOfFieldsBeforeStart; count++)
            {
                StandardField tempField = new StandardField(++fieldId) { Previous = standardField };
                standardField.Next = tempField;
                fields.Add(standardField);
                standardField = tempField;
            }

            StartField startField = new StartField(++fieldId, this.ColorCode);
            standardField.Next = startField;
            startField.Previous = standardField;

            fields.Add(standardField);

            EndField endField = new EndField(fieldId + NumberOfFieldsAfterStart + 1) { Previous = startField };
            startField.EndFieldEntry = endField;
            

            StandardField standardFieldAfter = new StandardField(fieldId) { Previous = startField };
            startField.Next = standardFieldAfter;
            fields.Add(startField);
            StartField = startField;

            for (int count = 1; count <= NumberOfFieldsAfterStart; count++)
            {
                StandardField tempField = new StandardField(++fieldId) { Previous = standardFieldAfter };
                if (count == 1)
                {
                    StartField.Next = tempField;
                }
                standardFieldAfter.Next = tempField;
                fields.Add(standardFieldAfter);
                standardFieldAfter = tempField;
            }

            fields.Add(standardFieldAfter);

            ++fieldId;
            for (int count = 1; count < NumberOfFieldsBeforeStart; count++)
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