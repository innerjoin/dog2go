using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using dog2go.Backend.Model;

namespace dog2go.Backend.Services
{
    public class Validation
    {
        public static bool ValidateMove(MeepleMove meepleMove, CardMove cardMove)
        {
            if (cardMove.SelectedAttribute.Attribute == AttributeEnum.ChangePlace)
            {
                if (meepleMove.Meeple != null && meepleMove.MoveDestination.CurrentMeeple != null)
                {
                    if (meepleMove.Meeple.ColorCode != meepleMove.MoveDestination.CurrentMeeple.ColorCode)
                    {
                        if (
                            !(meepleMove.MoveDestination is KennelField || meepleMove.MoveDestination is EndField ||
                              meepleMove.Meeple.CurrentPosition is KennelField ||
                              meepleMove.Meeple.CurrentPosition is EndField))
                        {
                            var destinationStartField = meepleMove.MoveDestination as StartField;
                            var sourceStartField = meepleMove.Meeple.CurrentPosition as StartField;
                            if (destinationStartField != null && sourceStartField != null)
                            {
                                if (destinationStartField.ColorCode == destinationStartField.CurrentMeeple.ColorCode)
                                {
                                    return !destinationStartField.CurrentMeeple.IsStartFieldBlocked;
                                }

                                if (sourceStartField.ColorCode == sourceStartField.CurrentMeeple.ColorCode)
                                {
                                    return !sourceStartField.CurrentMeeple.IsStartFieldBlocked;
                                }

                                return true;
                            }

                            else
                            {

                                if (destinationStartField != null && meepleMove.Meeple.CurrentPosition is StandardField)
                                {
                                    if (destinationStartField.ColorCode == destinationStartField.CurrentMeeple.ColorCode)
                                    {
                                        return !destinationStartField.CurrentMeeple.IsStartFieldBlocked;
                                    }

                                    else
                                    {
                                        return true;
                                    }
                                }

                                if (sourceStartField != null && meepleMove.MoveDestination is StandardField)
                                {
                                    if (sourceStartField.ColorCode == sourceStartField.CurrentMeeple.ColorCode)
                                    {
                                        return !sourceStartField.CurrentMeeple.IsStartFieldBlocked;
                                    }

                                    else
                                    {
                                        return true;
                                    }
                                }

                                var destinationStandField = meepleMove.MoveDestination as StandardField;
                                var sourceStandField = meepleMove.Meeple.CurrentPosition as StandardField;
                                if (destinationStandField != null && sourceStandField != null)
                                {
                                    return true;
                                }
                            }
                        }

                        else
                        {
                            return false;
                        }

                    }

                    else
                    {
                        return false;
                    }
                }

                else
                {
                    return false;
                }
            }

            if (cardMove.SelectedAttribute.Attribute == AttributeEnum.LeaveKennel)
            {
                var destination = meepleMove.MoveDestination as StartField;
                if (meepleMove.MoveDestination.CurrentMeeple != null)
                {
                    return destination?.ColorCode == meepleMove.MoveDestination.CurrentMeeple.ColorCode;
                }
                else
                {
                    return true;
                }
            }

            else
            {
                AttributeEnum attribute = cardMove.SelectedAttribute.Attribute;
                int value = (int)attribute;
                MoveDestinationField currentPos = meepleMove.Meeple.CurrentPosition;
                if (value > 0)
                {
                    while (value > 0)
                    {
                        currentPos = currentPos.Next;
                        value--;
                    }
                }
                else
                {
                    while (value < 0)
                    {
                        currentPos = currentPos.Previous;
                        value++;
                    }
                }

                if (currentPos == meepleMove.MoveDestination)
                {
                    return true;
                }

                else
                {
                    return false;
                }
            }
        }

    }
}