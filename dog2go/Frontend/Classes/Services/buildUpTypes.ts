/*export class MoveDestinationField implements IMoveDestinationField {
    Identifier: number;
    previous: MoveDestinationField;
    next: MoveDestinationField;

    nextIdentifier: number;
    previousIdentifier: number;

    viewRepresentation: Phaser.Graphics;

    constructor(previous: MoveDestinationField) {
        this.previous = previous;
        const self = this;
        if (previous instanceof StartField && self instanceof EndField) {
            previous.setEndFieldEntry(self);
        } else if (previous != null) {
            previous.setNext(self);
        }
    }

    private setNext(next: MoveDestinationField) {
        this.next = next;
    }

    NextIdentifier: number;
    PreviousIdentifier: number;
    FieldType: string;
}

export class KennelField extends MoveDestinationField implements IKennelField{
    constructor() {
        super(null);
    }
}

export class EndField extends MoveDestinationField {
    constructor(previous: MoveDestinationField) {
        super(previous);
    }
}

export class StartField extends MoveDestinationField {
    constructor(previous: MoveDestinationField) {
        super(previous);
    }
    endFieldEntry: EndField;
    setEndFieldEntry(next: EndField) {
        this.endFieldEntry = next;
    }
}

export class PlayerFieldArea {
    constructor(color: AreaColor) {
        this.color = color;
        this.createFields();
    }

    color: AreaColor;
    kennelFields: KennelField[] = [];
    gameFields: MoveDestinationField[] = [];
    endFields: EndField[] = [];

    private createFields() {
        let prev = null;
        let field: MoveDestinationField;
        let i: number;
        // create the 4 fields before the start field
        for (i = 0; i < 4; i++) {
            field = new MoveDestinationField(prev);
            this.gameFields.push(field);
            prev = field;
        }
        // create the start field itself
        const startField = new StartField(prev);
        this.gameFields.push(startField);
        // create the 11 fields after the start field
        prev = startField;
        for (i = 0; i < 11; i++) {
            field = new MoveDestinationField(prev);
            this.gameFields.push(field);
            prev = field;
        }
        // create the 4 end fields 
        prev = startField;
        for (i = 0; i < 4; i++) {
            field = new EndField(prev);
            this.endFields.push(field);
            prev = field;
        }

        // create the 4 kennel fields
        for (i = 0; i < 4; i++) {
            this.kennelFields.push(new KennelField());
        }
    }
}
*/
