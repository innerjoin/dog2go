namespace dog2go.Backend.Model
{
    public class Participation
    {
        public User Participant { get; set; }
        public User Partner { get; set; }
        private GameTable Table { get; set; }
    }
}
