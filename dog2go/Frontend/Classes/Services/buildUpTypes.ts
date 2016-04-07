    enum AreaColor {
        Red = 0xff0000,
        Blue = 0x0000ff,
        Green = 0x00ff00,
        Yellow = 0xedc613
    }

    class MoveDestinationField {
        identifier: number;
        previous: MoveDestinationField;
        next: MoveDestinationField;

        NextIdentifier: number;

        PreviousIdentifier: number;

        viewRepresentation;

        constructor(previous: MoveDestinationField) {
            this.previous = previous;
            let self = this;
            if (previous instanceof StartField && self instanceof EndField) {
                previous.setEndFieldEntry(self);
            } else if (previous != null) {
                previous.setNext(self);
            }
        }

        private setNext(next: MoveDestinationField) {
            this.next = next;
        }
    }

    class EndField extends MoveDestinationField {
        constructor(previous: MoveDestinationField) {
            super(previous);
        }
    }

    class StartField extends MoveDestinationField {
        constructor(previous: MoveDestinationField) {
            super(previous);
        }
        endFieldEntry: EndField;
        setEndFieldEntry(next: EndField) {
            this.endFieldEntry = next;
        }
    }


    class Persontest {
        private firstName: string;
        private lastName: string;

        setFirstName(value: string) {
            this.firstName = value;
        }

        setLastName(value: string) {
            this.lastName = value;
        }

        getFullName(lastNameFirst: boolean = false): string {
            if (lastNameFirst) {
                return this.lastName + ", " + this.firstName;
            }
            return this.firstName + ", " + this.lastName;
        }
    }

    class PlayerFieldArea {
        constructor(color: AreaColor) {
            this.color = color;
            this.createFields();
        }

        color: AreaColor;
        //kennelFields: MoveDestinationField[];
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
            let startField = new StartField(prev);
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
        }
    }
