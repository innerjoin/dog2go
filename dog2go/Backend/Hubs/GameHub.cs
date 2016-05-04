using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using dog2go.Backend.Constants;
using dog2go.Backend.Interfaces;
using dog2go.Backend.Model;
using dog2go.Backend.Repos;
using dog2go.Backend.Services;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
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
            bool isParticipating = false;
            if (table?.Participations == null || table.Participations.Count >= GlobalDefinitions.NofParticipantsPerTable)
            {
                table?.Participations?.ForEach(participation =>
                {
                    // Player allready connected to a running table
                    if (!participation.Participant.Nickname.Equals(curUser)) return;
                    isParticipating = true;
                    List<HandCard> cards = table.cardServiceData.GetActualHandCards(participation.Participant, table);
                    Clients.Client(Context.ConnectionId).backToGame(table, cards);

                    // TODO: only during development: Notify any caller als actualplayer:
                    NotifyActualPlayer(participation.Participant, cards);
                });
                if (isParticipating)
                {
                    return table;
                }
                throw new Exception("Should never happen: Table already full");

            }

            Participation newParticipation;
            if (table.Participations.Count() % 2 == 1)
            {
                User actualUser = UserRepository.Instance.Get()
                        .First(user => user.Value.Nickname == curUser).Value;
                newParticipation =
                    new Participation(actualUser)
                    {
                        Partner = table.Participations.Last().Participant
                    };
                table.Participations.Last().Partner = actualUser;
                table.PlayerFieldAreas.Find(area => area.Identifier == table.Participations.Count()+1).Participation =
                    newParticipation;
            }
            else
            {
                newParticipation = new Participation(UserRepository.Instance.Get().First(user => user.Value.Nickname == curUser).Value);
                table.PlayerFieldAreas.Find(area => area.Identifier == table.Participations.Count()+1).Participation =
                    newParticipation;
            }

            table.Participations.Add(newParticipation);

            Clients.Client(Context.ConnectionId).createGameTable(table);

            if (table.Participations.Count >= GlobalDefinitions.NofParticipantsPerTable)
            {
                AllConnected(table);

            }
            return table;
        }

        // for test method calls only
        public GameTable GetGeneratedGameTable()
        {
            return GenerateNewGameTable(-1);
        }

        private static GameTable GenerateNewGameTable(int gameId)
        {
            List<PlayerFieldArea> areas = new List<PlayerFieldArea>();

            int id = 0;

            const int fieldId = 0;
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
        private void NotifyActualPlayer(User user, List<HandCard> handCards)
        {
            GameTable actualGameTable = GetActualGameTable();
            List<HandCard> validCards = actualGameTable.cardServiceData.ProveCards(handCards, actualGameTable, user);
            var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            if (validCards != null)
            {
                context.Clients.Group(GlobalDefinitions.GroupName).broadcastMessage(Context.User.Identity.Name, ServerMessages.InformOtherPlayer.Replace("{0}", Context.User.Identity.Name));
                user.ConnectionIds.ForEach(cId =>
                {
                    // TODO: get Cards, that are possible.
                    context.Clients.Client(cId).broadcastMessage(Context.User.Identity.Name, ServerMessages.NofityActualPlayer);
                    
                    Clients.Client(cId).notifyActualPlayer(validCards, GetColorCodeForUser(user.Nickname));
                });

            }
            else
            {
                NotifyNextPlayer();
            }
        }

        public void AllConnected(GameTable table)
        {
            table.RegisterCardService(new CardServices());
            SendCardsForRound(table);
        }

        private void SendCardsForRound(GameTable table)
        {
            table = GameServices.UpdateActualRoundCards(table);
            List<HandCard> validateHandCards = null;
            foreach (var participation in table.Participations)
            {
                SendCards(participation.ActualPlayRound.Cards, participation.Participant);
                if (participation.Participant.Nickname == Context.User.Identity.Name)
                    validateHandCards = participation.ActualPlayRound.Cards;

                // TODO: Notify the realy ActualPlayer
                if (table.Participations.IndexOf(participation) == 0)
                {
                    NotifyActualPlayer(participation.Participant, participation.ActualPlayRound.Cards);
                }
            }
            User actualUser = UserRepository.Instance.Get().FirstOrDefault(user => user.Value.Identifier == Context.User.Identity.Name).Value;
            var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            Clients.Client(Context.ConnectionId).notifyActualPlayer(table.cardServiceData.ProveCards(validateHandCards, table, actualUser));
        }

        public void ChooseCardExchange(HandCard selectedCard)
        {
            GameTable actualGameTable = GetActualGameTable();
            User actualUser = actualGameTable.Participations.Find(participation => participation.Participant.Identifier == Context.User.Identity.Name).Participant;
            actualGameTable.cardServiceData.CardExchange(actualUser, ref actualGameTable, selectedCard);
            User partnerUser = GameServices.GetPartner(actualUser, actualGameTable.Participations);
            partnerUser.ConnectionIds.ForEach(id =>
            {
                Clients.Client(id).exchangeCard(selectedCard);
            });
        }

        private GameTable GetActualGameTable()
        {
            return Games.Get().Find(table => table.Participations.Find(participation => participation.Participant.Nickname == Context.User.Identity.Name) != null);
        }

        public bool ValidateMove(MeepleMove meepleMove, CardMove cardMove)
        {
            if (meepleMove == null)
                return false;
            meepleMove.Meeple.CurrentPosition = meepleMove.Meeple.CurrentPosition ??
                                                Validation.GetFieldById(GetActualGameTable(),
                                                    meepleMove.Meeple.CurrentFieldId);
            meepleMove.MoveDestination = meepleMove.MoveDestination ?? Validation.GetFieldById(GetActualGameTable(), meepleMove.DestinationFieldId);
            if (Validation.ValidateMove(meepleMove, cardMove))
            {
                GameTable actualGameTable = GetActualGameTable();
                GameServices.UpdateMeeplePosition(meepleMove, actualGameTable);
                List<Meeple> allMeeples = new List<Meeple>();

                foreach (var area in actualGameTable.PlayerFieldAreas)
                {
                    allMeeples.AddRange(area.Meeples);
                }
                List<HandCard> actualCards = actualGameTable.cardServiceData.GetActualHandCards(
                   GetActualUser() , actualGameTable);
                actualGameTable.cardServiceData.RemoveCardFromUserHand(actualGameTable, GetActualUser(), cardMove.Card);
                Clients.All.sendMeeplePositions(allMeeples);
                NotifyNextPlayer();
                return true;
            }
            return false;
        }

        private User GetActualUser()
        {
            return UserRepository.Instance.Get()
                .FirstOrDefault(user => user.Value.Nickname == Context.User.Identity.Name)
                .Value;
        }

        private ColorCode GetColorCodeForUser(string userName)
        {
            GameTable actualGameTable = GetActualGameTable();
            return
                actualGameTable.PlayerFieldAreas.Find(area => area.Participation.Participant.Nickname == userName)
                    .ColorCode;
        }

        private void NotifyNextPlayer()
        {
            GameTable actualGameTable = GetActualGameTable();
            string nextPlayerNickname = GameServices.GetNextPlayer(actualGameTable, Context.User.Identity.Name);
            User nextUser = UserRepository.Instance.Get().FirstOrDefault(user => user.Value.Nickname == nextPlayerNickname).Value;
            List<HandCard> cards = actualGameTable.cardServiceData.GetActualHandCards(nextUser, actualGameTable);
            List<HandCard> validHandCards = actualGameTable.cardServiceData.ProveCards(cards, actualGameTable, nextUser);
            var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            nextUser.ConnectionIds.ForEach(id =>
            {
                context.Clients.Group(GlobalDefinitions.GroupName).broadcastMessage(Context.User.Identity.Name, ServerMessages.InformOtherPlayer.Replace("{0}", Context.User.Identity.Name));
                if (validHandCards != null)
                {
                    context.Clients.Client(id).broadcastMessage(Context.User.Identity.Name, ServerMessages.NofityActualPlayer);
                    
                    Clients.Client(id).notifyActualPlayer(validHandCards, GetColorCodeForUser(nextUser.Nickname));
                }    
                    

                else
                {
                    foreach (var card in actualGameTable.cardServiceData.GetActualHandCards(nextUser, actualGameTable))
                    {
                        actualGameTable.cardServiceData.RemoveCardFromUserHand(actualGameTable, GetActualUser(), card);
                    }

                    Clients.Client(id).dropCards();

                    if (actualGameTable.cardServiceData.ProveCardsCount%GlobalDefinitions.NofParticipantsPerTable != 0)
                        return;
                    if (!actualGameTable.cardServiceData.AreCardsOnHand(actualGameTable))
                    {
                        SendCardsForRound(actualGameTable);
                    }
                }
            });
        }
    }
}
