using System.Collections.Generic;
using System.Linq;
using dog2go.Backend.Interfaces;
using dog2go.Backend.Model;
using dog2go.Backend.Repos;

namespace dog2go.Backend.Services
{
    public static class GameTableService
    {
        public static ColorCode GetColorCodeForUser(IGameRepository games, string userName, int tableId)
        {
            GameTable actualGameTable = GetTable(games, tableId);
            return
                actualGameTable.PlayerFieldAreas.Find(area => area.Participation.Participant.Nickname == userName)
                    .ColorCode;
        }

        public static GameTable GetTable(IGameRepository games, int gameId)
        {
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
            foreach (PlayerFieldArea playFieldArea in gameTable.PlayerFieldAreas)
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
            MoveDestinationField saveField = null;
            Meeple moveDestinationMeeple = null;

            PrepareMeeplePos(gameTable, meepleMove, ref saveField, ref moveDestinationMeeple, isChangePlace);
            SetFirstMeeplePosition(gameTable, meepleMove, saveField);

            if (moveDestinationMeeple == null) return;
            if (!isChangePlace)
            {
                moveDestinationMeeple = SetBackToKennelField(gameTable,moveDestinationMeeple );   
            }

            SetSecondMeeplePosition(gameTable, saveField, moveDestinationMeeple);
        }

        private static Meeple SetBackToKennelField(GameTable actualGameTable, Meeple moveDestinationMeeple)
        {
            if (moveDestinationMeeple == null) return null;
            foreach (KennelField kennelField in actualGameTable.PlayerFieldAreas.Select(area => area.KennelFields.Find(
                field => field.CurrentMeeple == null && area.ColorCode == moveDestinationMeeple.ColorCode)).Where(kennelField => kennelField != null))
            {
                moveDestinationMeeple.CurrentFieldId = kennelField.Identifier;
                moveDestinationMeeple.CurrentPosition = kennelField;
            }

            return moveDestinationMeeple;
        }

        private static void SetSecondMeeplePosition(GameTable gameTable,  MoveDestinationField saveField, Meeple moveDestinationMeeple)
        {
            foreach (Meeple actualMeeple in gameTable.PlayerFieldAreas.Select(area =>
                                                                        area.Meeples.Find(meeple => meeple.CurrentPosition.Identifier == saveField.Identifier && meeple.ColorCode == moveDestinationMeeple.ColorCode)).
                                                                        Where(actualMeeple => actualMeeple != null))
            {
                actualMeeple.CurrentPosition = moveDestinationMeeple.CurrentPosition;
                actualMeeple.CurrentFieldId = moveDestinationMeeple.CurrentFieldId;
            }
        }

        private static void SetFirstMeeplePosition(GameTable gameTable, MeepleMove meepleMove, MoveDestinationField saveField)
        {
            foreach (Meeple actualMeeple in gameTable.PlayerFieldAreas.Select(area =>
                                                                        area.Meeples.Find(meeple => meeple.CurrentPosition.Identifier == meepleMove.Meeple.CurrentFieldId)).
                                                                        Where(actualMeeple => actualMeeple != null))
            {
                if (actualMeeple.CurrentPosition.FieldType.Contains("StartField"))
                    actualMeeple.IsStartFieldBlocked = false;
                actualMeeple.CurrentPosition = saveField;
                actualMeeple.CurrentFieldId = saveField?.Identifier ?? -1;
            }
        }

        private static void PrepareMeeplePos(GameTable gameTable, MeepleMove meepleMove, ref MoveDestinationField saveField, ref Meeple meeple, bool isChangePlace)
        {
            foreach (PlayerFieldArea area in gameTable.PlayerFieldAreas)
            {
                MoveDestinationField oldDestinationField = area.Fields.Find(field => field.Identifier == meepleMove.Meeple.CurrentFieldId) ??
                                                           area.KennelFields.Find(field => field.Identifier == meepleMove.Meeple.CurrentFieldId);
                if (oldDestinationField != null)
                    oldDestinationField.CurrentMeeple = null;

                MoveDestinationField updateField = area.Fields.Find(field => field.Identifier == meepleMove.MoveDestination.Identifier) ??
                                                   area.KennelFields.Find(field => field.Identifier == meepleMove.MoveDestination.Identifier);
                if (updateField == null) continue;
                saveField = updateField;
                if (isChangePlace)
                {
                    if (updateField.CurrentMeeple != null)
                    {
                        meeple = updateField.CurrentMeeple;
                        meeple.CurrentFieldId = meepleMove.Meeple.CurrentPosition.Identifier;
                        meeple.CurrentPosition = meepleMove.Meeple.CurrentPosition;
                    }
                }

                else
                {
                    if (updateField.CurrentMeeple != null)
                    {
                        meeple = updateField.CurrentMeeple;
                    }
                }
                updateField.CurrentMeeple = meepleMove.Meeple;
            }
        }

        public static GameTable UpdateActualRoundCards(GameTable table)
        {
            if (table?.CardServiceData == null || table.Participations == null)
                return null;
            int nr = table.CardServiceData.GetNumberOfCardsPerUser();
            table.CardServiceData.CurrentRound++;

            List<Participation> participations = table.Participations;
            
            foreach (Participation participation in participations)
            {
                PlayRound actualPlayRound = new PlayRound(table.CardServiceData.CurrentRound - 1, nr);
                List<HandCard> cards = new List<HandCard>();
                for (int i = 0; i < nr; i++)
                {
                    cards.Add(new HandCard(table.CardServiceData.GetCard()));
                }

                actualPlayRound.Cards = cards;
                participation.ActualPlayRound = actualPlayRound;
            }

            return table;
        }
    }

}