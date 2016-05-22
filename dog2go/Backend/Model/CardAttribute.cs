namespace dog2go.Backend.Model
{
    public class CardAttribute
    {
        public CardAttribute() { }
        public string Name { get; set; }
        public CardFeature Attribute { get; set; }

        public CardAttribute(CardFeature cardFeature)
        {
            this.Attribute = cardFeature;
        }
    }

    public enum CardFeature
    {
        OneField = 1,
        TwoFields = 2,
        ThreeFields = 3,
        FourFields = 4,
        FourFieldsBack = -4,
        FiveFields = 5,
        SixFields = 6,
        SevenFields = 7,
        EightFields = 8,
        NineFields = 9,
        TenFields = 10,
        ElevenFields = 11,
        TwelveFields = 12,
        ThirteenFields = 13,
        ChangePlace = 14,
        LeaveKennel = 15
    }
}
