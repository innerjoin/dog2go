using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using dog2go.Backend.Model;

namespace dog2go.Backend.Services
{
    public class Validation
    {
        public static bool ProveChangePlace(Meeple moveMeeple, MoveDestinationField destinationField)
        {
            if (moveMeeple == null || destinationField.CurrentMeeple == null)
                return false;
            if (IsSameColorCode(moveMeeple.ColorCode, destinationField.CurrentMeeple.ColorCode))
                return false;
            if (IsSimpleInvalidChangeField(destinationField) || IsSimpleInvalidChangeField(moveMeeple.CurrentPosition))
                return false;
            if (IsStandardField(destinationField) && !IsStandardField(moveMeeple.CurrentPosition))
                return IsValidStartField(moveMeeple.CurrentPosition);
            if (!IsStandardField(destinationField) && IsStandardField(moveMeeple.CurrentPosition))
                return IsValidStartField(destinationField);
            if (!(IsStandardField(moveMeeple.CurrentPosition) && IsStandardField(destinationField)))
                return IsValidStartField(destinationField) && IsValidStartField(moveMeeple.CurrentPosition);
            return true;
        }

        public static bool ProveLeaveKennel(Meeple moveMeeple, MoveDestinationField destinationField)
        {
            if (!moveMeeple.CurrentPosition.FieldType.Contains("KennelField"))
                return false;
            if (!destinationField.FieldType.Contains("StartField"))
                return false;
            StartField startField = destinationField as StartField;
            if (!IsSameColorCode(startField.ColorCode, moveMeeple.ColorCode))
                return false;
            return destinationField.CurrentMeeple == null || IsValidStartField(destinationField);
        }

        public static bool ProveValueCard(Meeple moveMeeple, MoveDestinationField destinationField, int value)
        {
            MoveDestinationField currentPos = moveMeeple.CurrentPosition;
            if (HasBlockedField(currentPos, value))
                return false;
            if (destinationField.FieldType.Contains("EndField"))
                return CanMoveToEndFields(currentPos, value);
            currentPos = GetNextField(currentPos, value);
            return currentPos.Identifier == destinationField.Identifier;
        }

        private static bool IsSameColorCode(ColorCode firstColorCode, ColorCode secondColorCode)
        {
            return firstColorCode == secondColorCode;
        }

        public static bool IsMovableField(MoveDestinationField field)
        {
            return IsValidStartField(field) || field.FieldType.Contains("EndField") ||
                   field.FieldType.Contains("StandardField");
        }

        public static bool IsStandardField(MoveDestinationField field)
        {
            return field.FieldType.Contains("StandardField");
        }

        public static bool IsValidStartField(MoveDestinationField field)
        {
            StartField startField = field as StartField;
            return startField != null && !startField.CurrentMeeple.IsStartFieldBlocked;
        }
        private static bool IsSimpleInvalidChangeField(MoveDestinationField field)
        {
            KennelField kennelField = field as KennelField;
            EndField endField = field as EndField;

            return kennelField != null || endField != null;
        }
        public static bool ValidateMove(MeepleMove meepleMove, CardMove cardMove)
        {
            Meeple movedMeeple = meepleMove.Meeple;
            MoveDestinationField destinationField = meepleMove.MoveDestination;

            if (movedMeeple == null || cardMove.SelectedAttribute == null)
                return false;

            if (cardMove.SelectedAttribute.Attribute == AttributeEnum.ChangePlace)
            {
               return ProveChangePlace(movedMeeple, destinationField);
            }

            if (cardMove.SelectedAttribute.Attribute == AttributeEnum.LeaveKennel)
            {
                return ProveLeaveKennel(movedMeeple, destinationField);               
            }
            return ProveValueCard(movedMeeple, destinationField, (int)cardMove.SelectedAttribute.Attribute);
        }

        public static bool CanMoveToEndFields(MoveDestinationField startCountField, int fieldCount)
        {
            if (!HasBlockedField(startCountField, fieldCount))
            {
                for (var i = 0; i <= fieldCount; i++)
                {
                    startCountField = startCountField.Next;
                    StartField startField = startCountField as StartField;
                    if (startField != null)
                    {
                        EndField endField = startField.EndFieldEntry;
                        fieldCount--;
                        for (var j = fieldCount - i; j >= 0; j--)
                        {
                            endField = (EndField)endField.Next;
                            if (endField == null)
                                return false;
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool HasBlockedField(MoveDestinationField startCountField, int fieldCount)
        {
            if (fieldCount < 0)
            {
                if (startCountField.FieldType.Contains("StartField"))
                {
                    startCountField = startCountField.Previous;
                    fieldCount--;
                }

                for (var i = 0; i > fieldCount; i--)
                {
                    while (startCountField.FieldType.Contains("EndField"))
                        startCountField = startCountField.Previous;
                    StartField startField = startCountField as StartField;
                    if (startField != null)
                    {
                        if (startField.CurrentMeeple != null && startField.CurrentMeeple.IsStartFieldBlocked)
                            return true;
                        return false;
                    }

                    startCountField = startCountField.Previous;
                }

                return false;
            }

            else
            {
                if (startCountField.FieldType.Contains("StartField"))
                {
                    startCountField = startCountField.Next;
                    fieldCount++;
                }

                for (var i = 0; i <= fieldCount; i++)
                {
                    while (startCountField.FieldType.Contains("EndField"))
                        startCountField = startCountField.Next;
                    
                    StartField startField = startCountField as StartField;
                    if (startField != null)
                    {
                        if (startField.CurrentMeeple != null && startField.CurrentMeeple.IsStartFieldBlocked)
                            return true;
                        return false;
                    }

                    startCountField = startCountField.Next;
                }

                return false;
            }
        }

        public static MoveDestinationField GetNextField(MoveDestinationField currentPos, int value)
        {
            if (value > 0)
            {
                while (value > 0)
                {
                    currentPos = currentPos.Next;
                    if (!IsSimpleInvalidChangeField(currentPos))
                        --value;
                }
            }
            else
            {
                while (value < 0)
                {
                    currentPos = currentPos.Previous;
                    if (!IsSimpleInvalidChangeField(currentPos))
                        value++;
                }
            }

            return currentPos;
        }
    }
}