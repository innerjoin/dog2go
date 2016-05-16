using System.Collections.Generic;
using System.Linq;
using dog2go.Backend.Constants;
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
        private static readonly object Locker = new object();

        public GameHub(IGameRepository repos): base(repos){}

        public GameHub() {}


        public GameTable ConnectToTable()
        {
            lock (Locker)
            {
                GameTable table = GameTableService.GetTable(Games);
            string curUser = Context.User.Identity.Name;
                if (GameTableService.AlreadyConnected(table, curUser))
            {
                    Participation participation = ParticipationService.GetParticipation(table, curUser);
                    List<HandCard> cards = table.cardServiceData?.GetActualHandCards(participation.Participant, table);
                    Clients.Client(Context.ConnectionId).backToGame(table, cards);
                if (table.ActualParticipation == participation)
                {
                     NotifyActualPlayer(participation.Participant, cards);
                }
                }
                else
                {
                    ParticipationService.AddParticipation(table, curUser);

                    if (table.Participations.Count == GlobalDefinitions.NofParticipantsPerTable)
                        AllConnected(table);
                }
                Clients.Client(Context.ConnectionId).createGameTable(table);
                    return table;
                }
            }

        // for test method calls only
        public GameTable GetGeneratedGameTable()
        {
            int gameTableId = GameFactory.CreateGameTable(Games, GlobalDefinitions.GroupName);
            lock (Locker)
            {
                return Games.Get().Find(table => table.Identifier.Equals(gameTableId));
            }
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
            GameTable actualGameTable = GameTableService.GetActualGameTable(Locker, Games, Context.User.Identity.Name);
            List<HandCard> validCards = actualGameTable.cardServiceData.ProveCards(handCards, actualGameTable, user);
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            if (validCards != null)
            {
                context.Clients.Group(GlobalDefinitions.GroupName).broadcastSystemMessage(ServerMessages.InformOtherPlayer.Replace("{0}", user.Nickname));
                actualGameTable.ActualParticipation = ParticipationService.GetParticipation(actualGameTable, user.Nickname);
                user.ConnectionIds.ForEach(cId =>
                {
                    context.Clients.Client(cId).broadcastSystemMessage(ServerMessages.NofityActualPlayer);
                    Clients.Client(cId).notifyActualPlayer(validCards, GameTableService.GetColorCodeForUser(user.Nickname, Locker, Games));
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
            GameServices.UpdateActualRoundCards(table);
            foreach (var participation in table.Participations)
            {
                SendCards(participation.ActualPlayRound.Cards, participation.Participant);

                // TODO: Notify the realy ActualPlayer
                if (table.Participations.IndexOf(participation) == 0)
                {
                    NotifyActualPlayer(participation.Participant, participation.ActualPlayRound.Cards);
                }
            }
        }

        public void ChooseCardExchange(HandCard selectedCard)
        {
            GameTable actualGameTable = GameTableService.GetActualGameTable(Locker, Games, Context.User.Identity.Name);
            User actualUser = actualGameTable.Participations.Find(participation => participation.Participant.Identifier == Context.User.Identity.Name).Participant;
            actualGameTable.cardServiceData.CardExchange(actualUser, ref actualGameTable, selectedCard);
            User partnerUser = GameServices.GetPartner(actualUser, actualGameTable.Participations);
            partnerUser.ConnectionIds.ForEach(id =>
            {
                Clients.Client(id).exchangeCard(selectedCard);
            });
        }

        public void ValidateMove(MeepleMove meepleMove, CardMove cardMove)
        {
            if (meepleMove == null)
                return;
            meepleMove.Meeple.CurrentPosition = meepleMove.Meeple.CurrentPosition ??
                                                Validation.GetFieldById(GameTableService.GetActualGameTable(Locker, Games, Context.User.Identity.Name),
                                                    meepleMove.Meeple.CurrentFieldId);
            meepleMove.MoveDestination = meepleMove.MoveDestination ?? Validation.GetFieldById(GameTableService.GetActualGameTable(Locker, Games, Context.User.Identity.Name), meepleMove.DestinationFieldId);
            if (Validation.ValidateMove(meepleMove, cardMove))
            {
                GameTable actualGameTable = GameTableService.GetActualGameTable(Locker, Games, Context.User.Identity.Name);
                GameServices.UpdateMeeplePosition(meepleMove, actualGameTable);
                List<Meeple> allMeeples = new List<Meeple>();
                foreach (var area in actualGameTable.PlayerFieldAreas)
                {
                    allMeeples.AddRange(area.Meeples);
                }
                actualGameTable.cardServiceData.GetActualHandCards(
                    GameTableService.GetActualUser(Context.User.Identity.Name), actualGameTable);
                actualGameTable.cardServiceData.RemoveCardFromUserHand(actualGameTable, GameTableService.GetActualUser(Context.User.Identity.Name), cardMove.Card);
                Clients.All.sendMeeplePositions(allMeeples);
                NotifyNextPlayer();
                return;
            }
            else
            {
                Clients.Caller.returnMove();
            }
        }

        private void NotifyNextPlayer()
        {
            GameTable actualGameTable = GameTableService.GetActualGameTable(Locker, Games, Context.User.Identity.Name);
            string nextPlayerNickname = GameServices.GetNextPlayer(actualGameTable, Context.User.Identity.Name);
            User nextUser = UserRepository.Instance.Get().FirstOrDefault(user => user.Value.Nickname == nextPlayerNickname).Value;
            List<HandCard> cards = actualGameTable.cardServiceData.GetActualHandCards(nextUser, actualGameTable);
            List<HandCard> validHandCards = actualGameTable.cardServiceData.ProveCards(cards, actualGameTable, nextUser);
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            context.Clients.Group(GlobalDefinitions.GroupName).broadcastSystemMessage(ServerMessages.InformOtherPlayer.Replace("{0}", nextUser.Nickname));
            nextUser.ConnectionIds.ForEach(id =>
            {
                actualGameTable.ActualParticipation = ParticipationService.GetParticipation(actualGameTable, nextUser.Nickname);
                if (validHandCards != null)
                {
                    context.Clients.Client(id).broadcastSystemMessage(ServerMessages.NofityActualPlayer);
                    Clients.Client(id).notifyActualPlayer(validHandCards, GameTableService.GetColorCodeForUser(nextUser.Nickname, Locker, Games));
                }    
                else
                {
                    foreach (var card in actualGameTable.cardServiceData.GetActualHandCards(nextUser, actualGameTable))
                    {
                        actualGameTable.cardServiceData.RemoveCardFromUserHand(actualGameTable, GameTableService.GetActualUser(Context.User.Identity.Name), card);
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
