using System;
using System.Collections.Generic;
using System.Linq;
using dog2go.Backend.Model;

namespace dog2go.Backend.Services
{
    public class GameServices
    {
        public static bool IsGameFinished(GameTable gameTable)
        {
            if (gameTable == null)
                throw new ArgumentNullException(nameof(gameTable));
            List<PlayerFieldArea> playerFieldAreasList = gameTable.PlayerFieldAreas;
            
            return (from participantFieldArea in playerFieldAreasList
                        let partnerIdentifier = participantFieldArea.Participation.Partner.Identifier
                        let partnerFieldArea = playerFieldAreasList.Find(area => area.Participation.Participant.Identifier.Equals(partnerIdentifier))
                        let participantEndFields = participantFieldArea.Fields.FindAll(field => field.CurrentMeeple != null && field.FieldType.Contains("EndField"))
                        let partnerEndFields = partnerFieldArea.Fields.FindAll(field => field.CurrentMeeple != null && field.FieldType.Contains("EndField"))
                    where AreEndFieldsFull(partnerEndFields) && AreEndFieldsFull(participantEndFields)
                    select participantEndFields).Any();
        }

        public static IEnumerable<string> GetWinners(GameTable gameTable)
        {
            if (gameTable == null)
                throw new ArgumentNullException(nameof(gameTable));
            if (!IsGameFinished(gameTable))
                return null;

            IEnumerable<string> names = (from participantFieldArea in gameTable.PlayerFieldAreas
                        let partnerIdentifier = participantFieldArea.Participation.Partner.Identifier
                        let partnerFieldArea = gameTable.PlayerFieldAreas.Find(area => area.Participation.Participant.Identifier.Equals(partnerIdentifier))
                        let participantEndFields = participantFieldArea.Fields.FindAll(field => field.CurrentMeeple != null && field.FieldType.Contains("EndField"))
                        let partnerEndFields = partnerFieldArea.Fields.FindAll(field => field.CurrentMeeple != null && field.FieldType.Contains("EndField"))
                    where AreEndFieldsFull(partnerEndFields) && AreEndFieldsFull(participantEndFields)
                    select participantFieldArea.Participation.Participant.Nickname);
            return names;
        }

        private static bool AreEndFieldsFull(IReadOnlyCollection<MoveDestinationField> endFieldList)
        {
            return endFieldList.Count == 4;
        }
    }
}