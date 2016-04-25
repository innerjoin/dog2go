/// <reference path="TableModel.ts"/>


interface IGameTable {
    Name: string;
    Identifier: number;
    Start: Date;
    Cookie: string;
    Stop: Date;
    PlayerFieldAreas: IPlayerFieldArea[];
    /*        public List < PlayerFieldArea > PlayerFieldAreas;
        public List < Participation > Participations; */
}

interface IPlayerFieldArea {
    PreviousIdentifier: number;
    NextIdentifier: number;
    Identifier: number;
    FieldId: number;
    _previous: IPlayerFieldArea;
    _next: IPlayerFieldArea;
    ColorCode: number;
    KennelFields: IKennelField[];
    Fields: IMoveDestinationField[];
    EndFields: IEndField[];
    StartField: IStartField;
    Meeples: IMeeple[];
    //   public Participation Participation { get; set; } 
}

interface IMeeple {
    ColorCode: number;
    CurrentPosition: IMoveDestinationField;
    IsStartFieldBlocked: boolean;
}

interface IMoveDestinationField {
    Identifier: number;
    previous: IMoveDestinationField;
    next: IMoveDestinationField;

    FieldType: string;

    NextIdentifier: number;
    PreviousIdentifier: number;

    viewRepresentation: Phaser.Graphics;
}

interface IKennelField extends IMoveDestinationField {

}

interface IEndField extends IMoveDestinationField {
    
}

interface IStartField extends IMoveDestinationField {
    EndFieldEntry: IEndField;
}


/*interface IAreaColor {
    Red;// = 0xff0000;
    Blue;// = 0x0000ff;
    Green;// = 0x00ff00;
    Yellow;// = 0xedc613;
}*/
