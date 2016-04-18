class GameTable implements IGameTable {

    public Name: string;
    public Identifier: number;
    public Start: Date;
    public Cookie: string;
    public Stop: Date;
    /*        public List < PlayerFieldArea > PlayerFieldAreas;
        public List < Participation > Participations; */
}

class HandCard extends Card implements IHandCard {
    IsPlayed: boolean;
}

class Card implements ICard {
    Name: string;
    Value: number;
    Picture: any;
    Attributes: CardAttribute[];
}

class CardAttribute implements ICardAttribute {
    public Name: string;
    public Attribute: AttributeEnum;
}