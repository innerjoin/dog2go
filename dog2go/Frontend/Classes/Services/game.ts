/// <reference path="../../Library/Phaser/phaser.comments.d.ts"/>

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

function addTestData(): PlayerFieldArea[] {
    let areas: PlayerFieldArea[] = [];
    const colors: AreaColor[] = [AreaColor.Red, AreaColor.Blue, AreaColor.Yellow, AreaColor.Green];
    for (let i = 0; i < 4; i++) {
        const area = new PlayerFieldArea(colors[i]);
        areas.push(area);
    }
    return areas;
}






class GameArea extends Phaser.State {
    constructor() {
        super();
        const gameStates = {
            preload: this.preload,
            create: this.create
        };
        this.game = new Phaser.Game(720, 720, Phaser.AUTO, "content", gameStates);
        this.game.state.add('GameArea', this, false);

        this.game.state.start('GameArea');
    }

    game:Phaser.Game;
    areas: PlayerFieldArea[] = [];
    fields: Phaser.Graphics[] = [];

    /* load game assets here, but not objects */
    preload() {
        this.areas = addTestData();
        this.fields = [];
        
        this.game.load.image('meeple_blue', '../Frontend/Images/pawn_blue.png');

    }
    addField(x: number, y: number, color: number): Phaser.Graphics {
        let graphics = this.game.add.graphics(x, y);
        graphics.beginFill(color, 1);
        graphics.drawCircle(graphics.x, graphics.y, 20);
        graphics.endFill();
        this.fields.push(graphics);
        return graphics;
    }
    dropLimiter(item) {
        var nearest: Phaser.Graphics;
        var smalest: number = 99999999;
        
        this.fields.forEach((field) => {
            var dist = Math.sqrt((item.x - field.x) ^ 2 + (item.y - field.y) ^ 2);
            if (!(smalest < dist)) {
                smalest = dist;
                nearest = field;
            }
        });
        if (nearest != null) {
            item.x = nearest.x;
            item.y = nearest.y;
        }

        /* this.fields.forEach((field) => {
            var dist = Math.sqrt((item.x - field.x) ^ 2 + (item.y - field.y) ^ 2);
        });*/
        /*if (item.x > 150) {
            item.x = 90;
        }
        if (item.y > 150) {
            item.y = 90;
        }*/
    }

    create() {
        var cellSpan = 20;
        this.game.stage.backgroundColor = 0xddeeCC;
        let pos = 0;
        const xStart = [260, 20, 100, 340];
        const yStart = [20, 100, 340, 260];
        const x1 = [-cellSpan, 0, cellSpan, 0];
        const y1 = [0, cellSpan, 0, -cellSpan];
        const x2 = [0, cellSpan, 0, -cellSpan];
        const y2 = [cellSpan, 0, -cellSpan, 0];

        for (let area of this.areas) {
            console.log(area);
            let el = area.gameFields[0];
            let x = xStart[pos]; //right 360; //left 40; //top 280; // bottom 120;
            let y = yStart[pos]; // right 260;// left 100; //top 20; // bottom 250;
            for(let i = 0; i < area.gameFields.length; i++) {
                console.log(x, y, el);
                var color = 0xeeeeee;
                if (el instanceof StartField) {
                    color = area.color;
                    let ex = x;
                    let ey = y;
                    let finEl = el.endFieldEntry;
                    for (let j = 0; j < area.endFields.length; j++) {
                        ex += x2[pos];
                        ey += y2[pos];
                        el.viewRepresentation = this.addField(ex, ey, color);
                        finEl = finEl.next;
                    }
                }
                el.viewRepresentation = this.addField(x, y, color);
                // Calculate Position for next field or something like that
                if (i < 8 || i > 11) {
                    x += x1[pos];//x += 0; //left x += 0; //top x -= 20; //bottom x += 20;
                    y += y1[pos];//y += -20;  //left y += 20; //top y += 0; //bottom y += 20;
                } else {
                    x += x2[pos];//x += -20; //left x += 20; //top  x += 0; // bottom x += 20;
                    y += y2[pos];//y += 0;  //left y += 0;  //top y += 20; // bottom y += -20;
                }
                el = el.next;
            } //while (el.next);
            pos++;
        }

        var meepleBlue = this.game.add.sprite(this.game.world.centerX, this.game.world.height - 45, 'meeple_blue');
        meepleBlue.anchor.setTo(0.5, 0.5);
        meepleBlue.scale.setTo(0.08, 0.08);
        meepleBlue.inputEnabled = true;
        meepleBlue.input.enableDrag();
        meepleBlue.input.enableSnap(20, 20, true, true);
        //meepleBlue.events.onDragStop.add(this.dropLimiter);
        meepleBlue.events.onDragUpdate.add(this.dropLimiter, this);
    }
    
}

window.onload = () => {
    var gameArea = new GameArea();
};