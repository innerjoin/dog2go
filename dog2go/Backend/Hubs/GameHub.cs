using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Caching;
using dog2go.Backend.Model;
using Microsoft.AspNet.SignalR;

namespace dog2go.Backend.Hubs
{
    public class GameHub : Hub
    {
        private readonly List<GameTable>  _tableList = new List<GameTable>();

        public void SendGameTable()
        {
            List<PlayerFieldArea> areas = new List<PlayerFieldArea>();

            int id = 0;

            int fieldId = 0;
            PlayerFieldArea areaTop = new PlayerFieldArea(++id, ColorCode.Blue, fieldId);
            PlayerFieldArea areaLeft = new PlayerFieldArea(++id, ColorCode.Red, areaTop.FieldId);
            PlayerFieldArea areaBottom = new PlayerFieldArea(++id, ColorCode.Green, areaLeft.FieldId);
            PlayerFieldArea areaRight = new PlayerFieldArea(++id, ColorCode.Yellow, areaBottom.FieldId);
            // Connection between PlayFieldAreas
            areaTop.Next = areaLeft;
            areaTop.Previous = areaRight;
            areaRight.Next = areaTop;
            areaRight.Previous = areaBottom;
            areaLeft.Next = areaBottom;
            areaLeft.Previous = areaTop;
            areaBottom.Next = areaRight;
            areaBottom.Previous = areaLeft;

            areas.Add(areaTop);
            areas.Add(areaLeft);
            areas.Add(areaBottom);
            areas.Add(areaRight);
            
            
            GameTable table = new GameTable(areas, _tableList.Count());
            _tableList.Add(table);
            Clients.All.createGameTable(table);
        }

         

        public bool ValidateMove(MeepleMove meepleMove, CardMove cardMove)
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
                                    //TODO: Zum ersten Mal aus Zwinger unterscheiden
                                    return false;
                                }

                                if (sourceStartField.ColorCode == sourceStartField.CurrentMeeple.ColorCode)
                                {
                                    //TODO: Zum ersten Mal aus Zwinger unterscheiden
                                    return false;
                                }

                                return true;
                            }

                            else
                            {

                                if (destinationStartField != null && meepleMove.Meeple.CurrentPosition is StandardField)
                                {
                                    if (destinationStartField.ColorCode == destinationStartField.CurrentMeeple.ColorCode)
                                    {
                                        //TODO: Zum ersten Mal aus Zwinger unterscheiden
                                        return false;
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
                                        //TODO: Zum ersten Mal aus Zwinger unterscheiden
                                        return false;
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
