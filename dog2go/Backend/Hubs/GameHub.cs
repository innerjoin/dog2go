using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using dog2go.Backend.Interfaces;
using dog2go.Backend.Model;
using dog2go.Backend.Repos;
using dog2go.Backend.Services;
using Microsoft.AspNet.SignalR;

namespace dog2go.Backend.Hubs
{
    [Authorize]
    public class GameHub : Hub
    {
        //private readonly  List<User> _userList = new List<User>();

        private IGameRepository _games;
        public GameHub(IGameRepository repos)
        {
            _games = repos;
        }
        public GameHub()
        {
            _games = GameRepository.Instance;
        }
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


            GameTable table = new GameTable(areas, _games.Get().Count());
            _games.Get().Add(table);

            return table;
        }
        public void UpdateOpenGames()
        {
            Clients.Client(Context.ConnectionId).updateOpenGames(GameRepository.Instance.Get().Find(game => game.Participations.Count < 4));
        }
        public void BackToGame()
        {
            foreach (var table in GameRepository.Instance.Get().Where(table => table.Participations.Any(participation => participation.Participant.Nickname == Context.User.Identity.Name)))
            {
                Clients.Client(Context.ConnectionId).backToGame(table, table.Participations.Find(participation => participation.Participant.Nickname == Context.User.Identity.Name).ActualPlayRound.Cards);
            }
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
            return Validation.ValidateMove(meepleMove, cardMove);
        }

        public async Task CreateGame()
        {
            /*DefaultHubManager hd = new DefaultHubManager(GlobalHost.DependencyResolver);
            var hub = hd.ResolveHub("sessionHub") as SessionHub;*/

            GameTable table = GenerateNewGameTable();
            User selectedUser = UserRepository.Instance.Get().Find(user => user.Identifier == Context.ConnectionId);
            //Task task = JoinGroup(selectedUser.Nickname + "_group");
            //Task task = hub.JoinGroup(selectedUser.Nickname + "_group");

            table.Cookie = "dog2go_group=" + selectedUser.Nickname + "_group;expires" + new DateTime().AddSeconds(24 * 60 * 60).ToString("d", CultureInfo.CurrentCulture);
            table.Participations.Add(new Participation(UserRepository.Instance.Get().Find(user => user.Identifier == Context.ConnectionId)));
            //await task;
            Clients.Client(Context.ConnectionId).createGameTable(table);
        }

        public async Task JoinGame(int gameId)
        {
            //DefaultHubManager hd = new DefaultHubManager(GlobalHost.DependencyResolver);
            //var hub = hd.ResolveHub("sessionHub") as SessionHub;

            GameTable selectedGameTable = _games.Get().Find(table => table.Identifier == gameId);
            string cookie = selectedGameTable.Cookie;
            UserRepository.Instance.Get().Find(user => user.Identifier == Context.ConnectionId).GroupName = cookie.Substring(cookie.IndexOf("=", StringComparison.CurrentCulture) + 1, cookie.IndexOf("_group;", StringComparison.CurrentCulture) - cookie.IndexOf("=", StringComparison.CurrentCulture) + 1);
            Participation newParticipation = selectedGameTable.Participations.Count() % 2 == 1
                ? new Participation(UserRepository.Instance.Get().Find(user => user.Identifier == Context.ConnectionId))
                {
                    Partner = selectedGameTable.Participations.Last().Participant
                }
                : new Participation(UserRepository.Instance.Get().Find(user => user.Identifier == Context.ConnectionId));
            //Task task = JoinGroup(UserRepository.Instance.Get().Find(user => user.Identifier == Context.ConnectionId).GroupName);
           // Task task = hub.JoinGroup(UserRepository.Instance.Get().Find(user => user.Identifier == Context.ConnectionId).GroupName);
            selectedGameTable.Participations.Add(newParticipation);
            selectedGameTable.Participations.Find((participation => participation.Participant == selectedGameTable.Participations.Last().Partner)).Partner = newParticipation.Participant;
            //await task;
            Clients.Client(Context.ConnectionId).creatGameTable(selectedGameTable);
        }

        public void CheckHasOpportunity()
        {
            GameTable actualGameTable = _games.Get().Find(table => table.Participations.Find(participation => participation.Participant.Nickname == Context.User.Identity.Name )!= null );
            List<HandCard> actualHand = actualGameTable.Participations.Find(
                participation =>
                    participation.Participant == UserRepository.Instance.Get().Find(user => user.Identifier == Context.ConnectionId)).ActualPlayRound.Cards;
            ProveCards(actualHand, actualGameTable);
        }


        private bool ProveCards(List<HandCard> actualHandCards, GameTable actualGameTable)
        {
            PlayerFieldArea actualArea = actualGameTable.PlayerFieldAreas.Find(
                area =>
                    area.Participation.Participant == UserRepository.Instance.Get().Find(user => user.Identifier == Context.ConnectionId));
            List<Meeple> myMeeples = actualArea.Meeples;

            List<Meeple> otherMeeples = new List<Meeple>();
            foreach (var playFieldArea in actualGameTable.PlayerFieldAreas)
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

                else if (card.Attributes.Find(attribute => attribute.Attribute == AttributeEnum.SevenFields) != null)
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
                        if (myMeeples.Any(meeple => meeple != openMeeple && !HasBlockedField(meeple.CurrentPosition, i)))
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

                    return myOpenMeeples.Any(meeple => card.Attributes.Select(attribute => (meeple.CurrentPosition.Identifier + ((int)attribute.Attribute))).Any(newPositionId => !HasBlockedField(meeple.CurrentPosition, newPositionId)));
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
                for (var i = 0; i > fieldCount; i--)
                {
                    startCountField = startCountField.Previous;
                    StartField startField = startCountField as StartField;
                    if (startField != null)
                    {
                        return startField.CurrentMeeple != null && startField.CurrentMeeple.IsStartFieldBlocked;
                    }
                }

                return false;
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
