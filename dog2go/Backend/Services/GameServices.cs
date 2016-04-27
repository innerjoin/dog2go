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

            // alte IsGameFinished-Methode ohne dynamische Partnerwahl
            //PlayerFieldArea greenArea = gameTable.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Green);
            //PlayerFieldArea yellowArea = gameTable.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Yellow);
            //PlayerFieldArea blueArea = gameTable.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Blue);
            //PlayerFieldArea redArea = gameTable.PlayerFieldAreas.Find(area => area.ColorCode == ColorCode.Red);

            //List<EndField> endFieldListGreen = greenArea.EndFields.FindAll(field => field.CurrentMeeple != null);
            //List<EndField> endFieldListYellow = yellowArea.EndFields.FindAll(field => field.CurrentMeeple != null);
            //List<EndField> endFieldListBlue = blueArea.EndFields.FindAll(field => field.CurrentMeeple != null);
            //List<EndField> endFieldListRed = redArea.EndFields.FindAll(field => field.CurrentMeeple != null);

            //return (IsEndFieldsFull(endFieldListGreen) && IsEndFieldsFull(endFieldListBlue)) || (IsEndFieldsFull(endFieldListRed) && IsEndFieldsFull(endFieldListYellow));
        }

        private static bool IsEndFieldsFull(IReadOnlyCollection<EndField> endFieldList)
        {
            return endFieldList.Count == 4;
        }
    }
}