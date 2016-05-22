using System;
using System.Collections.Generic;
using System.Linq;
using dog2go.Backend.Model;


namespace dog2go.Backend.Services
{
    public class GameServices
    {
        public bool IsGameFinished(GameTable gameTable)
        {
            if (gameTable == null)
                throw new ArgumentNullException(nameof(gameTable));
            List<PlayerFieldArea> playerFieldAreasList = gameTable.PlayerFieldAreas;
            
            return (from participantFieldArea in playerFieldAreasList
                        let partnerIdentifier = participantFieldArea.Participation.Partner.Identifier
                        let partnerFieldArea = playerFieldAreasList.Find(area => area.Participation.Participant.Identifier.Equals(partnerIdentifier))
                        let participantEndFields = participantFieldArea.EndFields.FindAll(field => field.CurrentMeeple != null)
                        let partnerEndFields = partnerFieldArea.EndFields.FindAll(field => field.CurrentMeeple != null)
                    where IsEndFieldsFull(partnerEndFields) && IsEndFieldsFull(participantEndFields)
                    select participantEndFields).Any();
        }

        private static bool IsEndFieldsFull(IReadOnlyCollection<EndField> endFieldList)
        {
            return endFieldList.Count == 4;
        }
    }
}