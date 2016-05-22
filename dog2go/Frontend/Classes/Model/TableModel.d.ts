interface IGameTable {
    Name: string;
    Identifier: number;
    Start: Date;
    Cookie: string;
    Stop: Date;
    PlayerFieldAreas: IPlayerFieldArea[];
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
}

interface IMeeple {
    ColorCode: number;
    CurrentPosition: IMoveDestinationField;
    IsStartFieldBlocked: boolean;
    CurrentFieldId: number;
    Identifier: number;

    spriteRepresentation: Phaser.Sprite;
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

interface IMeepleMove {
    Meeple: IMeeple;
    MoveDestination: IMoveDestinationField;
    DestinationFieldId: number;
}

interface IKennelField extends IMoveDestinationField {

}

interface IEndField extends IMoveDestinationField {
    
}

interface IStartField extends IMoveDestinationField {
    EndFieldEntry: IEndField;
}
