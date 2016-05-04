/*
class GameTable implements IGameTable {

    public Name: string;
    public Identifier: number;
    public Start: Date;
    public Cookie: string;
    public Stop: Date;
    PlayerFieldAreas: IPlayerFieldArea[] = [];
    /*        public List < PlayerFieldArea > PlayerFieldAreas;
        public List < Participation > Participations; *//*
}

class HandCard extends Card implements IHandCard {
    IsPlayed: boolean;
}

class Card implements ICard {
    Name: string;
    Value: number;
    ImageIdentifier: string;
    Attributes: CardAttribute[];
}

class CardAttribute implements ICardAttribute {
    public Name: string;
    public Attribute: AttributeEnum;
}*/

export enum AreaColor {
    Red = 0xff0000,
    Blue = 0x0000ff,
    Green = 0x00ff00,
    Yellow = 0xedc613
}
