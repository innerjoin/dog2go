using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using dog2go.Backend.Model;


namespace dog2go.Backend.Services
{
    public class GameServices
    {
        public bool IsGameFinished(GameTable gameTable)
        {
            List<Participation> participationsList = gameTable.Participations;
            List<PlayerFieldArea> playerFieldAreasList = gameTable.PlayerFieldAreas;
            foreach (PlayerFieldArea participantFieldArea in playerFieldAreasList)
            {
                String partnerIdentifier = participantFieldArea.Participation.Partner.Identifier;
                PlayerFieldArea partnerFieldArea = playerFieldAreasList.Find(area => area.Participation.Participant.Identifier.Equals(partnerIdentifier));
                List<EndField> participantEndFields = participantFieldArea.EndFields.FindAll(field => field.CurrentMeeple != null);
                List<EndField> partnerEndFields = partnerFieldArea.EndFields.FindAll(field => field.CurrentMeeple != null);
                if (IsEndFieldsFull(partnerEndFields) && IsEndFieldsFull(participantEndFields))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsEndFieldsFull(IReadOnlyCollection<EndField> endFieldList)
        {
            return endFieldList.Count == 4;
        }

        public static User GetPartner(User user, List<Participation> participations)
        {
            if (user == null || participations == null)
                return null;
            return participations.Find(participation => participation.Participant.Identifier == user.Identifier)?.Partner;
        }

        public static string GetNextPlayer(GameTable actualGameTable, string userName)
        {
            int? identifier = actualGameTable?.PlayerFieldAreas.Find(area => area.Participation.Participant.Nickname == userName)?.NextIdentifier;
            return actualGameTable?.PlayerFieldAreas.Find(area => area.Identifier == identifier)?.Participation?.Participant.Nickname;
        }

        public static void UpdateMeeplePosition(MeepleMove meepleMove, GameTable gameTable)
        {
            if (gameTable == null || meepleMove == null)
                return;
            foreach(var area in gameTable.PlayerFieldAreas)
            {
                MoveDestinationField updateField = area.Fields.Find(field => field.Identifier == meepleMove.MoveDestination.Identifier);
                if (updateField != null)
                {
                    updateField.CurrentMeeple = meepleMove.Meeple;
                }
            }
        }

        public static void UpdateActualRoundCards(GameTable table)
        {
            if (table?.cardServiceData == null ||table.Participations == null )
                return;
            int nr = table.cardServiceData.GetNumberOfCardsPerUser();
            table.cardServiceData.CurrentRound++;
            PlayRound actualPlayRound = new PlayRound(table.cardServiceData.CurrentRound - 1, nr);
            List<Participation> participations = table.Participations;
            List<HandCard> cards = null;
            foreach (Participation participation in participations)
            {
                cards = new List<HandCard>();
                for (int i = 0; i < nr; i++)
                {
                    cards.Add(new HandCard(table.cardServiceData.GetCard()));
                }

                actualPlayRound.Cards = cards;
                participation.ActualPlayRound = actualPlayRound;
            }
        } 

    }
}