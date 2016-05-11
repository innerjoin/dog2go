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

        private int CreateGameTable()
        {
            int newIdentifier = Games.Get().Count;
            GameTable generatedTable = GameFactory.GenerateNewGameTable(newIdentifier);
            Games.Add(generatedTable);
            return generatedTable.Identifier;
        }

        public GameTable ConnectToTable()
        {
            lock (Locker)
            {
                GameTable table = GetTable();
            string curUser = Context.User.Identity.Name;
                if (AlreadyConnected(table, curUser))
            {
                    Participation participation = GetParticipation(table, curUser);
                    List<HandCard> cards = table.cardServiceData?.GetActualHandCards(participation.Participant, table);
                    Clients.Client(Context.ConnectionId).backToGame(table, cards);
                }
                else
                {
                    AddParticipation(table, curUser);

                    if (table.Participations.Count == GlobalDefinitions.NofParticipantsPerTable)
                        AllConnected(table);
                }
                Clients.Client(Context.ConnectionId).createGameTable(table);
                    return table;
                }
            }

        private static void AddParticipation(GameTable table, string curUser)
        {
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
            }
            else
            {
                newParticipation = new Participation(UserRepository.Instance.Get().First(user => user.Value.Nickname == curUser).Value);
            }
            table.PlayerFieldAreas.Find(area => area.Identifier == table.Participations.Count() + 1).Participation = newParticipation;
            table.Participations.Add(newParticipation);
        }

        private GameTable GetTable()
        {
            int gameId = GlobalDefinitions.GameId;
            if (Games.Get().Count == 0)
                gameId = CreateGameTable();
            return Games.Get().Find(x => x.Identifier == gameId);
        }

        private static bool AlreadyConnected(GameTable table, string curUser)
            {
            return table?.Participations != null && 
                (table.Participations).Any(part => 
                        curUser.Equals(part.Participant.Nickname));
        }

        private static Participation GetParticipation(GameTable table, string curUser)
        {
            return table?.Participations?.FirstOrDefault(part => curUser.Equals(part.Participant.Nickname));
        }

        // for test method calls only
        public GameTable GetGeneratedGameTable()
        {
            return GameFactory.GenerateNewGameTable(-1);
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
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            if (validCards != null)
            {
                context.Clients.Group(GlobalDefinitions.GroupName).broadcastSystemMessage(ServerMessages.InformOtherPlayer.Replace("{0}", user.Nickname));
                actualGameTable.ActualParticipation = GetParticipation(actualGameTable, user.Nickname);
                user.ConnectionIds.ForEach(cId =>
                {
                    context.Clients.Client(cId).broadcastSystemMessage(ServerMessages.NofityActualPlayer);
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
            lock (Locker)
            {
            return Games.Get().Find(table => table.Participations.Find(participation => participation.Participant.Nickname == Context.User.Identity.Name) != null);
        }
        }

        public void ValidateMove(MeepleMove meepleMove, CardMove cardMove)
        {
            if (meepleMove == null)
                return;
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
                actualGameTable.cardServiceData.GetActualHandCards(
                    GetActualUser(), actualGameTable);
                actualGameTable.cardServiceData.RemoveCardFromUserHand(actualGameTable, GetActualUser(), cardMove.Card);
                Clients.All.sendMeeplePositions(allMeeples);
                NotifyNextPlayer();
                return;
            }

            else
            {
                Clients.Caller.returnMove();
            }
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
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            nextUser.ConnectionIds.ForEach(id =>
            {
                context.Clients.Group(GlobalDefinitions.GroupName).broadcastSystemMessage(ServerMessages.InformOtherPlayer.Replace("{0}", Context.User.Identity.Name));
                actualGameTable.ActualParticipation = GetParticipation(actualGameTable, nextUser.Nickname);
                if (validHandCards != null)
                {
                    context.Clients.Client(id).broadcastSystemMessage(ServerMessages.NofityActualPlayer);
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
