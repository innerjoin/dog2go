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
    }
}