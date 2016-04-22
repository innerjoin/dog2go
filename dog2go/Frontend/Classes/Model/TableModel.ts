

export interface IGameTable {
    Name: string;
    Identifier: number;
    Start: Date;
    Cookie: string;
    Stop: Date;
    PlayerFieldAreas: IPlayerFieldArea[];
    /*        public List < PlayerFieldArea > PlayerFieldAreas;
        public List < Participation > Participations; */
}

export interface IMeeple {
    ColorCode: IAreaColor;
    CurrentPosition: IMoveDestinationField;
    IsStartFieldBlocked: boolean;
}



export interface IMoveDestinationField {
    Identifier: number;
    previous: IMoveDestinationField;
    next: IMoveDestinationField;

    FieldType: string;

    NextIdentifier: number;
    PreviousIdentifier: number;

    viewRepresentation: Phaser.Graphics;
}

export interface IKennelField extends IMoveDestinationField {

}

export interface IEndField extends IMoveDestinationField {
    
}

export interface IStartField extends IMoveDestinationField {
    EndFieldEntry: IEndField;
}

export interface IPlayerFieldArea {

    PreviousIdentifier: number;
    NextIdentifier: number;
    Identifier: number;
    FieldId: number;
    _previous: IPlayerFieldArea;
    _next: IPlayerFieldArea;
    ColorCode: IAreaColor;
    KennelFields: IKennelField[];
    Fields: IMoveDestinationField[];
    EndFields: IEndField[];
    StartField: IStartField;
    Meeples: IMeeple[];
    //   public Participation Participation { get; set; } 
}

export enum  IAreaColor {
    Red = 0xff0000,
    Blue = 0x0000ff,
    Green = 0x00ff00,
    Yellow = 0xedc613
}
