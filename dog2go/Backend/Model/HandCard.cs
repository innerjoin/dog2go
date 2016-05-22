namespace dog2go.Backend.Model
{
    public class HandCard : Card
    {
        public bool IsPlayed { get; set; }
        public bool IsValid;

        public HandCard(Card card) : base(card.Name, card.Id, card.ImageIdentifier, card.Attributes)
        {
            IsPlayed = false;
        }
    }
}
