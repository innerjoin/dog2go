

interface IHandCard extends ICard {
    IsPlayed: boolean;
    IsValid: boolean;
}

interface ICardMove {
    Card: ICard;
    SelectedAttribute: ICardAttribute;
}

interface ICard {
        Name: string;
        Value: number;
        ImageIdentifier: string;
        Attributes: ICardAttribute[];
}

interface ICardAttribute {
    Name: string;
    Attribute: AttributeEnum;
}

declare enum AttributeEnum {
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
