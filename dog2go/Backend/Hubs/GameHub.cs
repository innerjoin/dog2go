using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Caching;
using dog2go.Backend.Model;
using dog2go.Backend.Repos;
using Microsoft.AspNet.SignalR;

namespace dog2go.Backend.Hubs
{
    public class GameHub : Hub
    {
        private readonly List<GameTable>  _tableList = new List<GameTable>();
        //private readonly  List<User> _userList = new List<User>();

        public GameHub(IUserRepository repository)
        {
            GameUserRepository = repository;
        }

        public IUserRepository GameUserRepository{ get; set; }

        private GameTable GenerateNewGameTable()
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

            return table;
        }

        public GameTable GetGeneratedGameTable()
        {
            return GenerateNewGameTable();
        }
        public void SendGameTable()
        {
            Clients.All.createGameTable(GenerateNewGameTable());
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

        public void Login(string name, string cookie)
        {
            if (cookie == null)
            {
                string sessionCookie = "dog2go="+name+";expires="+new DateTime().AddSeconds(24*60*60).ToString("d", CultureInfo.CurrentCulture);
                Clients.Client(Context.ConnectionId).newSession(sessionCookie);
                User newUser = new User(name, Context.ConnectionId) {Cookie = sessionCookie};
                GameUserRepository.Add(newUser);
                Clients.Client(Context.ConnectionId).updateOpenGames(_tableList);
            }

            else
            {
                foreach (var table in _tableList.Where(table => table.Participations.Any(participation => participation.Participant.Cookie == cookie)))
                {
                    Clients.Client(Context.ConnectionId).backToGame(table, table.Participations.Find(participation => participation.Participant == GameUserRepository.Get().Find(user => user.Identifier == Context.ConnectionId)).ActualPlayRound.Cards);
                }
            }
        }

        public void Logout(string name)
        {
            LeaveGroup(GameUserRepository.Get().Find(user => user.Nickname == name).GroupName);
            GameUserRepository.Remove(GameUserRepository.Get().Find(user => user.Nickname == name));
        }

        public async Task CreateGame()
        {
            GameTable table = GenerateNewGameTable();
            User selectedUser = GameUserRepository.Get().Find(user => user.Identifier == Context.ConnectionId);
            Task task = JoinGroup(selectedUser.Nickname + "_group");
            
            table.Cookie = "dog2go_group="+ selectedUser.Nickname + "_group;expires" + new DateTime().AddSeconds(24 * 60 * 60).ToString("d", CultureInfo.CurrentCulture); ;
            table.Participations.Add(new Participation(GameUserRepository.Get().Find(user => user.Identifier == Context.ConnectionId)));
            await task;
            Clients.Client(Context.ConnectionId).createGameTable(table);
        }

        public async Task JoinGame(int gameId)
        {
            GameTable selectedGameTable = _tableList.Find(table => table.Identifier == gameId);
            string cookie = selectedGameTable.Cookie;
            GameUserRepository.Get().Find(user => user.Identifier == Context.ConnectionId).GroupName = cookie.Substring(cookie.IndexOf("=", StringComparison.CurrentCulture) + 1, cookie.IndexOf("_group;", StringComparison.CurrentCulture) - cookie.IndexOf("=", StringComparison.CurrentCulture) + 1);
            Participation newParticipation = selectedGameTable.Participations.Count()%2 == 1
                ? new Participation(GameUserRepository.Get().Find(user => user.Identifier == Context.ConnectionId))
                {
                    Partner = selectedGameTable.Participations.Last().Participant
                }
                : new Participation(GameUserRepository.Get().Find(user => user.Identifier == Context.ConnectionId));
            Task task = JoinGroup(GameUserRepository.Get().Find(user => user.Identifier == Context.ConnectionId).GroupName);
            selectedGameTable.Participations.Add(newParticipation);
            selectedGameTable.Participations.Find((participation => participation.Participant == selectedGameTable.Participations.Last().Partner)).Partner = newParticipation.Participant;
            await task;
            Clients.Client(Context.ConnectionId).creatGameTable(selectedGameTable);
        }

        public void CheckHasOpportunity()
        {
            GameTable actualGameTable = _tableList.Find(table => table.Cookie == GameUserRepository.Get().Find(user => user.Identifier == Context.ConnectionId).Cookie);
            List<HandCard> actualHand = actualGameTable.Participations.Find(
                participation =>
                    participation.Participant == GameUserRepository.Get().Find(user => user.Identifier == Context.ConnectionId)).ActualPlayRound.Cards;
            ProveCards(actualHand, actualGameTable);
        }

        private async Task JoinGroup(string groupName)
        {
            await Groups.Add(Context.ConnectionId, groupName);
            //Clients.OthersInGroup(groupName).addChatMessage(name, message);
        }

        private Task LeaveGroup(string groupName)
        {
            return Groups.Remove(Context.ConnectionId, groupName);
        }

        private bool ProveCards(List<HandCard> actualHandCards, GameTable actualGameTable)
        {
            PlayerFieldArea actualArea = actualGameTable.PlayerFieldAreas.Find(
                area =>
                    area.Participation.Participant == GameUserRepository.Get().Find(user => user.Identifier == Context.ConnectionId));
            List<Meeple> myMeeples = actualArea.Meeples;

            List<Meeple> otherMeeples = new List<Meeple>();
            foreach(var playFieldArea in actualGameTable.PlayerFieldAreas)
            {
                otherMeeples.AddRange(playFieldArea.Meeples);
            }

            otherMeeples.RemoveAll(meeple => myMeeples.Contains(meeple));

            foreach (var card in actualHandCards)
            {
                if (card.Attributes.Find(attribute => attribute.Attribute == AttributeEnum.LeaveKennel) != null)
                {
                    Meeple proveMeeple = myMeeples.FindAll(meeple =>
                    {
                        KennelField field = meeple.CurrentPosition as KennelField;
                        return (field != null);
                    }).Find(meeple => myMeeples.Exists(startMeeple =>
                    {
                        StartField start = startMeeple.CurrentPosition as StartField;
                        return start != null && meeple.IsStartFieldBlocked;
                    }));

                    return proveMeeple != null;
                }

                else if (card.Attributes.Find(attribute => attribute.Attribute == AttributeEnum.ChangePlace) != null)
                {
                    List<Meeple> myOpenMeeples = myMeeples.FindAll(meeple =>
                    {
                        StandardField standardField = meeple.CurrentPosition as StandardField;
                        StartField startField = meeple.CurrentPosition as StartField;
                        return (standardField != null || (startField != null && meeple.IsStartFieldBlocked == false));
                    });

                    List<Meeple> otherOpenMeeples = otherMeeples.FindAll(meeple =>
                    {
                        StandardField standardField = meeple.CurrentPosition as StandardField;
                        StartField startField = meeple.CurrentPosition as StartField;
                        return (standardField != null || (startField != null && meeple.IsStartFieldBlocked == false));
                    });

                    return myOpenMeeples.Count > 0 && otherOpenMeeples.Count > 0;
                }

                else if (card.Attributes.Find(attribute =>attribute.Attribute == AttributeEnum.SevenFields) != null)
                {
                    List<Meeple> myOpenMeeples = myMeeples.FindAll(meeple =>
                    {
                        StandardField standardField = meeple.CurrentPosition as StandardField;
                        StartField startField = meeple.CurrentPosition as StartField;
                        EndField endField = meeple.CurrentPosition as EndField;
                        return (standardField != null || startField != null || endField != null);
                    });

                    int count = (int)AttributeEnum.SevenFields;
                    for (int i = 0; i <= count; i++)
                    {
                        Meeple openMeeple = myMeeples.Find(meeple => !HasBlockedField(meeple.CurrentPosition, count - i));
                        if(myMeeples.Any(meeple => meeple != openMeeple && !HasBlockedField(meeple.CurrentPosition, i)))
                            return true;
                            //return meeples != null || meeples.Find(meeple => CanMoveToEndFields(meeple.CurrentPosition, i)) != null;
                    }
                }

                else
                {
                    List<Meeple> myOpenMeeples = myMeeples.FindAll(meeple =>
                    {
                        StandardField standardField = meeple.CurrentPosition as StandardField;
                        StartField startField = meeple.CurrentPosition as StartField;
                        EndField endField = meeple.CurrentPosition as EndField;
                        return (standardField != null || startField != null || endField != null);
                    });

                    return myOpenMeeples.Any(meeple => card.Attributes.Select(attribute => (meeple.CurrentPosition.Identifier + ((int) attribute.Attribute))).Any(newPositionId => ! HasBlockedField(meeple.CurrentPosition, newPositionId)));
                }
            }
            return false;
        }

        public void ChooseCardExchange(HandCard card)
        {
            
        }

        public bool CanMoveToEndFields(MoveDestinationField startCountField, int fieldCount)
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

        public bool HasBlockedField(MoveDestinationField startCountField, int fieldCount)
        {
            if (fieldCount < 0)
            {
                for (var i = 0; i >= fieldCount; i--)
                {
                    startCountField = startCountField.Previous;
                    StartField startField = startCountField as StartField;
                    if (startField != null)
                    {
                        return startField.CurrentMeeple != null && startField.CurrentMeeple.IsStartFieldBlocked;
                    }
                }

                return true;
            }

            else
            {
                for (var i = 0; i <= fieldCount; i++)
                {
                    startCountField = startCountField.Next;
                    StartField startField = startCountField as StartField;
                    if (startField != null)
                    {
                        return startField.CurrentMeeple != null && startField.CurrentMeeple.IsStartFieldBlocked;
                    }
                }

                return true;
            }
        }
    }
}
