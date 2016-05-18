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
using WebGrease.Css.Extensions;

namespace dog2go.Backend.Hubs
{
    [Authorize]
    public class GameHub : GenericHub
    {
        private static readonly object Locker = new object();

        public GameHub(IGameRepository repos) : base(repos) { }

        public GameHub() { }


        public GameTable ConnectToTable(int gameTableId)
        {
            lock (Locker)
            {
                GameTable table = GameTableService.GetTable(Games, gameTableId);
                string curUser = Context.User.Identity.Name;
                if (GameTableService.AlreadyConnected(table, curUser))
                {
                    Participation participation = ParticipationService.GetParticipation(table, curUser);
                    List<HandCard> cards = table.CardServiceData?.GetActualHandCards(participation.Participant, table);
                    Task task = Clients.Client(Context.ConnectionId).backToGame(table, cards);
                    task.Wait();
                    if (table.ActualParticipation == participation)
                    {
                         NotifyActualPlayer(participation.Participant, cards);
                    }
                }
                else
                {
                    ParticipationService.AddParticipation(table, curUser);
                }
                if (table.Participations.Count == GlobalDefinitions.NofParticipantsPerTable)
                    AllConnected(table);
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
            List<HandCard> validCards = actualGameTable.CardServiceData.ProveCards(handCards, actualGameTable, user);
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            if (validCards.Find(card => card.IsValid) != null)
            {
                if (user.ConnectionIds.Count == 1)
                {
                    Task task = context.Clients.Group(GlobalDefinitions.GroupName, ParticipationService.GetSingleConnectionId(user.ConnectionIds)).broadcastSystemMessage(ServerMessages.InformOtherPlayer.Replace("{0}", user.Nickname));
                    task.Wait();
                }
                actualGameTable.ActualParticipation = ParticipationService.GetParticipation(actualGameTable, user.Nickname);
                user.ConnectionIds.ForEach(cId =>
                {
                    Task task =  context.Clients.Client(cId).broadcastSystemMessage(ServerMessages.NofityActualPlayer);
                    task.Wait();
                    Clients.Client(cId).notifyActualPlayer(validCards, GameTableService.GetColorCodeForUser(user.Nickname, Locker, Games));
                });

            }
            else
            {
                NotifyNextPlayer("");
            }
        }

        public void AllConnected(GameTable table)
        {
            table.RegisterCardService(new CardServices());
            SendCardsForRound(table);
        }

        private void SendCardsForRound(GameTable table)
        {
            GameTableService.UpdateActualRoundCards(table);
            foreach (var participation in table.Participations)
            {
                SendCards(participation.ActualPlayRound.Cards, participation.Participant);
            }

            if (table.ActualParticipation != null)
                NotifyActualPlayer(table.ActualParticipation.Participant, table.ActualParticipation.ActualPlayRound.Cards);
            else
            {
                Participation participation = table.Participations.First();
                if (participation != null)
                {
                    NotifyActualPlayer(participation.Participant, participation.ActualPlayRound.Cards);
                }
            }
        }

        public void ChooseCardExchange(HandCard selectedCard)
        {
            GameTable actualGameTable = GameTableService.GetActualGameTable(Locker, Games, Context.User.Identity.Name);
            User actualUser = actualGameTable.Participations.Find(participation => participation.Participant.Identifier == Context.User.Identity.Name).Participant;
            actualGameTable.CardServiceData.CardExchange(actualUser, ref actualGameTable, selectedCard);
            User partnerUser = ParticipationService.GetPartner(actualUser, actualGameTable.Participations);
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
            meepleMove.MoveDestination = meepleMove.MoveDestination ??
                                                Validation.GetFieldById(GameTableService.GetActualGameTable(Locker, Games, 
                                                Context.User.Identity.Name), meepleMove.DestinationFieldId);
            if (Validation.ValidateMove(meepleMove, cardMove))
            {
                GameTable actualGameTable = GameTableService.GetActualGameTable(Locker, Games, Context.User.Identity.Name);
                GameTableService.UpdateMeeplePosition(meepleMove, actualGameTable, cardMove.SelectedAttribute != null);
                List<Meeple> allMeeples = new List<Meeple>();
                foreach (PlayerFieldArea area in actualGameTable.PlayerFieldAreas)
                {
                    allMeeples.AddRange(area.Meeples);
                }
                actualGameTable.CardServiceData.RemoveCardFromUserHand(actualGameTable, GameTableService.GetActualUser(Context.User.Identity.Name), cardMove.Card);
                Task test = Clients.All.sendMeeplePositions(allMeeples);
                test.Wait();
                NotifyNextPlayer("");
            }
            else
            {
                Clients.Caller.returnMove();
            }
        }

        private void NotifyNextPlayer(string nextUserName)
        {
            GameTable actualGameTable;
            string nextPlayerNickname;
            if (string.IsNullOrWhiteSpace(nextUserName))
            {
                actualGameTable = GameTableService.GetActualGameTable(Locker, Games, Context.User.Identity.Name);
                nextPlayerNickname = ParticipationService.GetNextPlayer(actualGameTable, Context.User.Identity.Name);
            }

            else
            {
                actualGameTable = GameTableService.GetActualGameTable(Locker, Games, nextUserName);
                nextPlayerNickname = nextUserName;
            }
            User nextUser = UserRepository.Instance.Get().FirstOrDefault(user => user.Value.Nickname == nextPlayerNickname).Value;
            List<HandCard> cards = actualGameTable.CardServiceData.GetActualHandCards(nextUser, actualGameTable);
            List<HandCard> validHandCards = actualGameTable.CardServiceData.ProveCards(cards, actualGameTable, nextUser);
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            actualGameTable.ActualParticipation = ParticipationService.GetParticipation(actualGameTable, nextUser.Nickname);
            if (nextUser.ConnectionIds.Count == 1)
            {
                Task task = context.Clients.Group(GlobalDefinitions.GroupName, ParticipationService.GetSingleConnectionId(nextUser.ConnectionIds)).broadcastSystemMessage(ServerMessages.InformOtherPlayer.Replace("{0}", nextUser.Nickname));
                task.Wait();
            }
            nextUser.ConnectionIds.ForEach(id =>  
            {
                if (validHandCards.Find(card => card.IsValid) != null)
                {
                    Task chatTask =  context.Clients.Client(id).broadcastSystemMessage(ServerMessages.NofityActualPlayer);
                    chatTask.Wait();
                    Clients.Client(id).notifyActualPlayer(validHandCards, GameTableService.GetColorCodeForUser(nextUser.Nickname, Locker, Games));
                }    
                else
                {
                    actualGameTable.CardServiceData.RemoveAllCardsFromUser(actualGameTable,nextUser );
                    Clients.Client(id).dropCards();

                    Task chatTask = context.Clients.Client(id).broadcastSystemMessage(ServerMessages.NoValidCardAvailable);
                    chatTask.Wait();
                    if (actualGameTable.CardServiceData.ProveCardsCount%GlobalDefinitions.NofParticipantsPerTable != 0)
                    {
                        NotifyNextPlayer(ParticipationService.GetNextPlayer(actualGameTable, nextUser.Nickname));
                        return;
                    }

                    if (!actualGameTable.CardServiceData.AreCardsOnHand(actualGameTable))
                    {
                        SendCardsForRound(actualGameTable);
                    }
                    else
                    {
                        NotifyNextPlayer(ParticipationService.GetNextPlayer(actualGameTable, nextUser.Nickname));
                    }
                }
            });
        }
    }
}
