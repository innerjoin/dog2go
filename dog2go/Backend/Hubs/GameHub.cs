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
                table.Participations.ForEach(participation =>
                {
                    if (participation.Participant.Nickname.Equals(curUser))
                    {
                        // TODO Send Cards, when they are saved in Repo
                        isParticipating = true;
                        Clients.Client(Context.ConnectionId).backToGame(table, null);
                    }
                });
                if (isParticipating)
                {
                    return table;
                }
                throw new Exception("Table already full");

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

            if(table.Participations.Count >= GlobalDefinitions.NofParticipantsPerTable)
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
            SendCardsForRound(table);
        }

        private void SendCardsForRound(GameTable table)
        {
            GameServices.UpdateActualRoundCards(table);
            List<HandCard> validateHandCards = null;
            foreach (var participation in table.Participations)
            {
                SendCards(participation.ActualPlayRound.Cards, participation.Participant);
                if (participation.Participant.Nickname == Context.User.Identity.Name)
                    validateHandCards = participation.ActualPlayRound.Cards;
            }
            User actualUser = UserRepository.Instance.Get().FirstOrDefault(user => user.Value.Identifier == Context.User.Identity.Name).Value;
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

        private void NotifyNextPlayer()
        {
            GameTable actualGameTable = GetActualGameTable();
            string nextPlayerNickname = GameServices.GetNextPlayer(actualGameTable, Context.User.Identity.Name);
            User nextUser = UserRepository.Instance.Get().FirstOrDefault(user => user.Value.Nickname == nextPlayerNickname).Value;
            List<HandCard> cards = actualGameTable.cardServiceData.GetActualHandCards(nextUser, actualGameTable);
            nextUser.ConnectionIds.ForEach(id =>
            {
                Clients.Client(id).notifyActualPlayer(actualGameTable.cardServiceData.ProveCards(cards, actualGameTable, nextUser));
            });
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
        //public void SendGameTable()
        //{
        //    Clients.All.createGameTable(GenerateNewGameTable());
        //}

        /*public bool ValidateMove(MeepleMove meepleMove, CardMove cardMove)
        {
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(2000);
                GameRepository repo = GameRepository.Instance;
                //repo.Get()[0].Participations.
            });
            Clients.Caller.notifyActualPlayer(null);
            Clients.Caller.sendMeeplePositions(null);
            Clients.Caller.dropCards();

            return true;
            //return Validation.ValidateMove(meepleMove, cardMove);
        }*/

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
    }
}
