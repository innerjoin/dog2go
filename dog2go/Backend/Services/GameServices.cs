using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using dog2go.Backend.Model;

namespace dog2go.Backend.Services
{
    public class GameServices
    {
        public static User GetPartner(User user, List<Participation> participations)
        {
            return participations.Find(participation => participation.Participant.Identifier == user.Identifier).Partner;
        }
    }
}