using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using dog2go.Backend.Constants;
using dog2go.Backend.Interfaces;
using dog2go.Backend.Model;
using dog2go.Backend.Repos;

namespace dog2go.Backend.Services
{
    public class GameTableService
    {
        public static ColorCode GetColorCodeForUser(string userName, object locker, IGameRepository games)
        {
            GameTable actualGameTable = GetActualGameTable(locker, games, userName);
            return
                actualGameTable.PlayerFieldAreas.Find(area => area.Participation.Participant.Nickname == userName)
                    .ColorCode;
        }

        public static GameTable GetActualGameTable(object locker, IGameRepository games, string userName)
        {
            lock (locker)
            {
                return games.Get().Find(table => table.Participations.Find(participation => participation.Participant.Nickname == userName) != null);
            }
        }

        public static GameTable GetTable(IGameRepository games, int gameId)
        {
            //int gameId = GlobalDefinitions.GameId;
            //if (games.Get().Count == 0)
            //    gameId = GameFactory.CreateGameTable(games, GlobalDefinitions.GroupName);
            return games.Get().Find(x => x.Identifier == gameId);
        }

        public static bool AlreadyConnected(GameTable table, string curUser)
        {
            return table?.Participations != null &&
                   (table.Participations).Any(part =>
                       curUser.Equals(part.Participant.Nickname));
        }

        public static User GetActualUser(string userName)
        {
            return UserRepository.Instance.Get()
                .FirstOrDefault(user => user.Value.Nickname == userName)
                .Value;
        }

        public static List<Meeple> GetOtherMeeples(GameTable gameTable, List<Meeple> myMeeples)
        {
            if (gameTable == null)
                return null;
            List<Meeple> otherMeeples = new List<Meeple>();
            foreach (var playFieldArea in gameTable.PlayerFieldAreas)
            {
                otherMeeples.AddRange(playFieldArea.Meeples);
            }

            otherMeeples.RemoveAll(myMeeples.Contains);
            return otherMeeples;
        }

        public static List<Meeple> GetOpenMeeples(List<Meeple> myMeeples)
        {
            return myMeeples?.FindAll(
                meeple =>
                    Validation.IsValidStartField(meeple.CurrentPosition) ||
                    meeple.CurrentPosition.FieldType.Contains("StandardField"));
        }

        public static void UpdateMeeplePosition(MeepleMove meepleMove, GameTable gameTable, bool isChangePlace)
        {
            if (gameTable == null || meepleMove == null)
                return;
            foreach (var area in gameTable.PlayerFieldAreas)
            {
                MoveDestinationField updateField = area.Fields.Find(field => field.Identifier == meepleMove.MoveDestination.Identifier) ??
                                                   area.KennelFields.Find(field => field.Identifier == meepleMove.MoveDestination.Identifier);
                if(updateField == null)continue;
                
                if (isChangePlace)
                {
                    Meeple moveDestinationMeeple = updateField.CurrentMeeple;
                    if(moveDestinationMeeple != null)
                        moveDestinationMeeple.CurrentPosition = meepleMove.Meeple.CurrentPosition;
                }

                else
                {
                    Meeple removeDestinationMeeple = updateField.CurrentMeeple;
                    if (removeDestinationMeeple != null)
                        removeDestinationMeeple.CurrentPosition =
                            area.KennelFields.Find(field => field.CurrentMeeple == null);
                }
                updateField.CurrentMeeple = meepleMove.Meeple;
                Meeple actualMeeple = area.Meeples.Find(meeple => meeple.CurrentPosition.Identifier == meepleMove.Meeple.CurrentFieldId);
                if (actualMeeple == null) continue;
                if (actualMeeple.CurrentPosition.FieldType.Contains("StartField"))
                    actualMeeple.IsStartFieldBlocked = false;
                actualMeeple.CurrentPosition = updateField;
                actualMeeple.CurrentFieldId = updateField.Identifier;
                MoveDestinationField oldDestinationField = area.Fields.Find(field => field.Identifier == meepleMove.Meeple.CurrentFieldId) ??
                                                           area.KennelFields.Find(field => field.Identifier == meepleMove.Meeple.CurrentFieldId);
                if(oldDestinationField == null)continue;
                oldDestinationField.CurrentMeeple = null;
            }
        }

        public static GameTable UpdateActualRoundCards(GameTable table)
        {
            if (table?.cardServiceData == null || table.Participations == null)
                return null;
            int nr = table.cardServiceData.GetNumberOfCardsPerUser();
            table.cardServiceData.CurrentRound++;

            List<Participation> participations = table.Participations;
            
            foreach (Participation participation in participations)
            {
                PlayRound actualPlayRound = new PlayRound(table.cardServiceData.CurrentRound - 1, nr);
                List<HandCard> cards = new List<HandCard>();
                for (int i = 0; i < nr; i++)
                {
                    cards.Add(new HandCard(table.cardServiceData.GetCard()));
                }

                actualPlayRound.Cards = cards;
                participation.ActualPlayRound = actualPlayRound;
            }

            return table;
        }
    }

}