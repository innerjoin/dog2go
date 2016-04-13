using System;

namespace dog2go.Backend.Model
{
    public class Participation
    {
        public User Participant { get; set; }
        public User Partner { get; set; }
        public PlayRound ActualPlayRound { get; set; }
        public Participation(User participantUser)
        {
            if(participantUser != null)
                Participant = participantUser;
            else
                throw new Exception("User was not logged in");
        }
    }
}
