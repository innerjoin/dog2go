using System;
using System.Collections.Generic;
using System.Linq;
using dog2go.Backend.Interfaces;
using dog2go.Backend.Model;
using dog2go.Backend.Repos;
using dog2go.Backend.Services;
using Microsoft.AspNet.SignalR;
using WebGrease.Css.Extensions;

namespace dog2go.Backend.Hubs
{
    [Authorize]
    public class GameHub : GenericHub
    {
        public GameHub(IGameRepository repos): base(repos){}

        public GameHub() {}

        private int CreateGameTable()
        {
            int newIdentifier = Games.Get().Count;
            GameTable generatedTable = GenerateNewGameTable(newIdentifier);
            Games.Add(generatedTable);
            return generatedTable.Identifier;
        }

        public GameTable ConnectToTable()
        {
            int gameId = 0;
            if (Games.Get().Count == 0)
                gameId = CreateGameTable();
            string curUser = Context.User.Identity.Name;

            GameTable table = Games.Get().Find(x => x.Identifier == gameId);
            if (table?.Participations == null || table.Participations.Count >= 4)
                throw new Exception("Table already full");

            Participation newParticipation;
            if (table.Participations.Count() % 2 == 1)
            {
                newParticipation =
                    new Participation(UserRepository.Instance.Get()
                        .First(user => user.Value.Identifier == curUser).Value)
                    {
                        Partner = table.Participations.Last().Participant
                    };
            }
            else
            {
                newParticipation = new Participation(UserRepository.Instance.Get().First(user => user.Value.Identifier == curUser).Value);
            }

            table.Participations.Add(newParticipation);

            Clients.Client(Context.ConnectionId).createGameTable(table);

            if(table.Participations.Count >= 4)
                AllConnected(table);

            return table;
        }

        // for test method calls only
        public GameTable GetGeneratedGameTable()
        {
            return GenerateNewGameTable(-1);
        }

        private GameTable GenerateNewGameTable(int gameId)
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

            GameTable table = new GameTable(areas, gameId);
            return table;
        }

        public void SendCards(List<HandCard> cards, User user)
        {
            user.ConnectionIds.ForEach(id =>
            {
                Clients.Client(id).assignHandCards(cards);
            });
        }

        public void AllConnected(GameTable table)
        {
            table.RegisterCardService(new CardServices());
            ShuffleAndSendCardsForRound(table);
        }

        private void ShuffleAndSendCardsForRound(GameTable table)
        {
            int nr = table.cardServiceData.GetNumberOfCardsPerUser();
            List<Participation> users = table.Participations;
            foreach (Participation user in users)
            {
                List<HandCard> cards = new List<HandCard>();
                for (int i = 0; i < nr; i++)
                {
                    cards.Add(new HandCard(table.cardServiceData.GetCard()));
                }
                SendCards(cards, user.Participant);
            }
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


        /*
         * Server Methoden
         * public void SendCards(List<HandCard> cards);
         * public void CheckHasOpportunity();// true notifyActualPlayer | false dropCards
         * public void ChooseCard(HandCard card);
         * public void ChooseCardExchange(HandCard card);
         * public void ChooseMove(MeepleMove move);
         */

        //public void UpdateOpenGames()
        //{
        //    Clients.Client(Context.ConnectionId).updateOpenGames(GameRepository.Instance.Get().Find(game => game.Participations.Count < 4));
        //}
        //public void BackToGame()
        //{
        //    foreach (var table in GameRepository.Instance.Get().Where(table => table.Participations.Any(participation => participation.Participant.Nickname == Context.User.Identity.Name)))
        //    {
        //        Clients.Client(Context.ConnectionId).backToGame(table, table.Participations.Find(participation => participation.Participant.Nickname == Context.User.Identity.Name).ActualPlayRound.Cards);
        //    }
        //}
        //public GameTable GetGeneratedGameTable()
        //{
        //    return GenerateNewGameTable();
        //}
        //public void SendGameTable()
        //{
        //    Clients.All.createGameTable(GenerateNewGameTable());
        //}

        //public bool ValidateMove(MeepleMove meepleMove, CardMove cardMove)
        //{
        //    return Validation.ValidateMove(meepleMove, cardMove);
        //}

        //public void CreateGame()
        //{
        //    if (singleGameTableIdentification < 0)
        //    {


        //    /*DefaultHubManager hd = new DefaultHubManager(GlobalHost.DependencyResolver);
        //    var hub = hd.ResolveHub("sessionHub") as SessionHub;*/

        //        GameTable table = GenerateNewGameTable();
        //    //User selectedUser = UserRepository.Instance.Get().Find(user => user.Identifier == Context.ConnectionId);
        //    //Task task = JoinGroup(selectedUser.Nickname + "_group");
        //    //Task task = hub.JoinGroup(selectedUser.Nickname + "_group");

        //    //table.Cookie = "dog2go_group=" + selectedUser.Nickname + "_group;expires" + new DateTime().AddSeconds(24 * 60 * 60).ToString("d", CultureInfo.CurrentCulture);
        //        singleGameTableIdentification = table.Identifier;
        //    }
        //    JoinGame(singleGameTableIdentification);
        //    //table.Participations.Add(new Participation(UserRepository.Instance.Get().Find(user => user.Identifier == Context.ConnectionId)));
        //    ////await task;
        //    //Clients.Client(Context.ConnectionId).createGameTable(table);
        //}

        //public void JoinGame(int gameId)
        //{

        //    var el = _games.Get();
        //    GameTable selectedGameTable = el.Find(table => table.Identifier == gameId);
        //    if (selectedGameTable.Participations.Count >= MaxPlayers)
        //        throw new Exception("Gameteable already full");

        //    string cookie = selectedGameTable.Cookie;
        //    var userRes = UserRepository.Instance.Get().Find(
        //        user => user.Identifier == Context.ConnectionId
        //        );
        //    var grpName = userRes.GroupName;
        //    var something = cookie.Substring(cookie.IndexOf("=", StringComparison.CurrentCulture) + 1, cookie.IndexOf("_group;", StringComparison.CurrentCulture) - cookie.IndexOf("=", StringComparison.CurrentCulture) + 1);
        //    grpName = something;
        //    Participation newParticipation = selectedGameTable.Participations.Count() % 2 == 1
        //        ? new Participation(UserRepository.Instance.Get().Find(user => user.Identifier == Context.ConnectionId))
        //        {
        //            Partner = selectedGameTable.Participations.Last().Participant
        //        }
        //        : new Participation(UserRepository.Instance.Get().Find(user => user.Identifier == Context.ConnectionId));
        //    //Task task = JoinGroup(UserRepository.Instance.Get().Find(user => user.Identifier == Context.ConnectionId).GroupName);
        //   // Task task = hub.JoinGroup(UserRepository.Instance.Get().Find(user => user.Identifier == Context.ConnectionId).GroupName);
        //    selectedGameTable.Participations.Add(newParticipation);         
        //    selectedGameTable.Participations.Find((participation => participation.Participant == selectedGameTable.Participations.Last().Partner)).Partner = newParticipation.Participant;
        //    //await task;
        //    Clients.Client(Context.ConnectionId).creatGameTable(selectedGameTable);

        //    if(selectedGameTable.Participations.Count == MaxPlayers)
        //        AllConnected(selectedGameTable);
        //}

        //public void CheckHasOpportunity()
        //{
        //    GameTable actualGameTable = _games.Get().Find(table => table.Participations.Find(participation => participation.Participant.Nickname == Context.User.Identity.Name )!= null );
        //    List<HandCard> actualHand = actualGameTable.Participations.Find(
        //        participation =>
        //            participation.Participant == UserRepository.Instance.Get().Find(user => user.Identifier == Context.ConnectionId)).ActualPlayRound.Cards;
        //    ProveCards(actualHand, actualGameTable);
        //}


        //private bool ProveCards(List<HandCard> actualHandCards, GameTable actualGameTable)
        //{
        //    PlayerFieldArea actualArea = actualGameTable.PlayerFieldAreas.Find(
        //        area =>
        //            area.Participation.Participant == UserRepository.Instance.Get().Find(user => user.Identifier == Context.ConnectionId));
        //    List<Meeple> myMeeples = actualArea.Meeples;

        //    List<Meeple> otherMeeples = new List<Meeple>();
        //    foreach (var playFieldArea in actualGameTable.PlayerFieldAreas)
        //    {
        //        otherMeeples.AddRange(playFieldArea.Meeples);
        //    }

        //    otherMeeples.RemoveAll(meeple => myMeeples.Contains(meeple));

        //    foreach (var card in actualHandCards)
        //    {
        //        if (card.Attributes.Find(attribute => attribute.Attribute == AttributeEnum.LeaveKennel) != null)
        //        {
        //            Meeple proveMeeple = myMeeples.FindAll(meeple =>
        //            {
        //                KennelField field = meeple.CurrentPosition as KennelField;
        //                return (field != null);
        //            }).Find(meeple => myMeeples.Exists(startMeeple =>
        //            {
        //                StartField start = startMeeple.CurrentPosition as StartField;
        //                return start != null && meeple.IsStartFieldBlocked;
        //            }));

        //            return proveMeeple != null;
        //        }

        //        else if (card.Attributes.Find(attribute => attribute.Attribute == AttributeEnum.ChangePlace) != null)
        //        {
        //            List<Meeple> myOpenMeeples = myMeeples.FindAll(meeple =>
        //            {
        //                StandardField standardField = meeple.CurrentPosition as StandardField;
        //                StartField startField = meeple.CurrentPosition as StartField;
        //                return (standardField != null || (startField != null && meeple.IsStartFieldBlocked == false));
        //            });

        //            List<Meeple> otherOpenMeeples = otherMeeples.FindAll(meeple =>
        //            {
        //                StandardField standardField = meeple.CurrentPosition as StandardField;
        //                StartField startField = meeple.CurrentPosition as StartField;
        //                return (standardField != null || (startField != null && meeple.IsStartFieldBlocked == false));
        //            });

        //            return myOpenMeeples.Count > 0 && otherOpenMeeples.Count > 0;
        //        }

        //        else if (card.Attributes.Find(attribute => attribute.Attribute == AttributeEnum.SevenFields) != null)
        //        {
        //            List<Meeple> myOpenMeeples = myMeeples.FindAll(meeple =>
        //            {
        //                StandardField standardField = meeple.CurrentPosition as StandardField;
        //                StartField startField = meeple.CurrentPosition as StartField;
        //                EndField endField = meeple.CurrentPosition as EndField;
        //                return (standardField != null || startField != null || endField != null);
        //            });

        //            int count = (int)AttributeEnum.SevenFields;
        //            for (int i = 0; i <= count; i++)
        //            {
        //                Meeple openMeeple = myMeeples.Find(meeple => !HasBlockedField(meeple.CurrentPosition, count - i));
        //                if (myMeeples.Any(meeple => meeple != openMeeple && !HasBlockedField(meeple.CurrentPosition, i)))
        //                    return true;
        //                //return meeples != null || meeples.Find(meeple => CanMoveToEndFields(meeple.CurrentPosition, i)) != null;
        //            }
        //        }

        //        else
        //        {
        //            List<Meeple> myOpenMeeples = myMeeples.FindAll(meeple =>
        //            {
        //                StandardField standardField = meeple.CurrentPosition as StandardField;
        //                StartField startField = meeple.CurrentPosition as StartField;
        //                EndField endField = meeple.CurrentPosition as EndField;
        //                return (standardField != null || startField != null || endField != null);
        //            });

        //            return myOpenMeeples.Any(meeple => card.Attributes.Select(attribute => (meeple.CurrentPosition.Identifier + ((int)attribute.Attribute))).Any(newPositionId => !HasBlockedField(meeple.CurrentPosition, newPositionId)));
        //        }
        //    }
        //    return false;
        //}

        //public void ChooseCardExchange(HandCard card)
        //{

        //}

        //public bool CanMoveToEndFields(MoveDestinationField startCountField, int fieldCount)
        //{
        //    if (!HasBlockedField(startCountField, fieldCount))
        //    {
        //        for (var i = 0; i <= fieldCount; i++)
        //        {
        //            startCountField = startCountField.Next;
        //            StartField startField = startCountField as StartField;
        //            if (startField != null)
        //            {
        //                EndField endField = startField.EndFieldEntry;
        //                for (var j = fieldCount - i; j >= 0; j--)
        //                {
        //                    endField = (EndField)endField.Next;
        //                    if (endField == null)
        //                        return false;
        //                }
        //                return true;
        //            }
        //        }
        //    }

        //    return false;
        //}

    }
}
