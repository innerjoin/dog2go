import _ = require("phaser");
import Coordinates = require("./FieldCoordinates");
import BuildUpTypes = require("../Services/buildUpTypes");
import FieldCoordinatesData = Coordinates.FieldCoordinatesData;
import AreaCoordinates = Coordinates.AreaCoordinates;
import PlayerFieldArea = BuildUpTypes.PlayerFieldArea;
import AreaColor = BuildUpTypes.AreaColor;
import MoveDestinationField = BuildUpTypes.MoveDestinationField;
import KennelField = BuildUpTypes.KennelField;
import StartField = BuildUpTypes.StartField;

//import GameFieldsService = require("../Services/GameFieldsService");
//import GameFieldService = GameFieldsService.GameFieldService;

export class GameArea {

    constructor() {
        //var chat = new ChatController();
        //this.gameFieldService = GameFieldsService.GameFieldService.getInstance(this.buildFields.bind(this));
        const gameStates = {
            preload: this.preload.bind(this),
            create: this.create.bind(this)
        };
        
        this.game = new Phaser.Game(720, 720, Phaser.AUTO, "content", gameStates);
        const fc = new Coordinates.FieldCoordinates();
        this.pos = fc.FOUR_PlAYERS;
    }

    pos: FieldCoordinatesData;
    //gameFieldService: GameFieldService;
    game: Phaser.Game;
    areas: BuildUpTypes.PlayerFieldArea[] = [];
    fields: Phaser.Graphics[] = [];

    public buildFields(areasPar: PlayerFieldArea[]) {
        // Source of Create() when good data comes from server
    }
    // Remove this function when GameAreaData comes from server!
    static addTestData(): PlayerFieldArea[] {
        const areas: PlayerFieldArea[] = [];
        const colors: AreaColor[] = [AreaColor.Red, AreaColor.Blue, AreaColor.Yellow, AreaColor.Green];
        for (let i = 0; i < 4; i++) {
            const area = new PlayerFieldArea(colors[i]);
            areas.push(area);
        }
        return areas;
    }

    /* load game assets here, but not objects */
    preload() {
        this.areas = GameArea.addTestData();
        this.fields = [];
        
        this.game.load.image("meeple_blue", "../Frontend/Images/pawn_blue.png");

    }

    static getFieldById(id: number, fields: MoveDestinationField[]): MoveDestinationField {
        for (let field of fields) {
            if (id === field.identifier) {
                return field;
            }
        }
        return null;
    }

    addKennelFields(game: Phaser.Game, kennelFields: KennelField[], areaPos: AreaCoordinates, color: number) {
        const kennelX = areaPos.x - 4 * areaPos.xOffset;
        console.log(areaPos.x + " - " + 4 + " * " + areaPos.xOffset + " = " + kennelX);
        const kennelY = areaPos.y - 4 * areaPos.yOffset;
        console.log(areaPos.y + " - " + 4 + " * " + areaPos.yOffset + " = " + kennelY);
        console.log(kennelFields);
        for (let i = 0; i < kennelFields.length; i++) {
            let xx = 0;
            let yy = 0;
            console.log("i % 4 = ", i % 4);
            switch (i % 4) {
                case 1:
                    xx = areaPos.xOffset;
                    yy = areaPos.yOffset;
                    break;
                case 2:
                    xx = areaPos.xAltOffset;
                    yy = areaPos.yAltOffset;
                    break;
                case 3:
                    xx = areaPos.xOffset + areaPos.xAltOffset;
                    yy = areaPos.yOffset + areaPos.yAltOffset;
                    break;
            }
            console.log("kennelX + xx = ", kennelX + xx);
            console.log("kennelY + yy = ", kennelY + yy);
            kennelFields[i].viewRepresentation = this.addField(game, kennelX + xx, kennelY + yy, color);
        }
    }

    addField(game: Phaser.Game, x: number, y: number, color: number): Phaser.Graphics {
        const graphics = game.add.graphics(x, y); // positioning is relative to parent (in this case, to the game world as no parent is defined)
        graphics.beginFill(color, 1);
        graphics.drawCircle(0, 0, 30); //draw a circle relative to it's parent (in this case, the graphics object)
        graphics.endFill();
        this.fields.push(graphics);
        return graphics;
    }

    dropLimiter(item: Phaser.Sprite) {
        var nearest: Phaser.Graphics;
        var smallest = Number.MAX_VALUE;
        var pos = item.world;
        this.fields.forEach((field) => {
            var fieldPos = field.world;
            var dist = fieldPos.distance(pos, true);
            if (!(smallest < dist)) {
                smallest = dist;
                nearest = field;
            }
        });
        if (nearest != null) {          
            item.x = nearest.x;
            item.y = nearest.y;
        }
    }

    //// This function is called when a full screen request comes in
    //onGoFullScreen() {
    //    // tell Phaser how you want it to handle scaling when you go full screen
    //    this.game.scale.fullScreenScaleMode = Phaser.ScaleManager.SHOW_ALL;
    //    // and this causes it to actually do it
    //    this.game.scale.refresh();
    //}
    //goFullScreen() {

    //}

    create() {
        //this.gameFieldService.getGameFieldData();
        
        var game = this.game;
        this.game.stage.backgroundColor = 0xddeeCC;
        let pos = 0;

        for (let area of this.areas) {
            let el = area.gameFields[0];
            const areaPos = this.pos.getAreaCoordinates(pos);
            
            // create kennel fields           
            this.addKennelFields(this.game, area.kennelFields, areaPos, area.color);

            // create destination fields
            for (let i = 0; i < area.gameFields.length; i++) {
                var color = 0xeeeeee;
                if (el instanceof StartField) {
                    color = area.color;
                    let ex = areaPos.x;
                    let ey = areaPos.y;
                    let finEl = el.endFieldEntry;
                    for (let j = 0; j < area.endFields.length; j++) {
                        ex += areaPos.xAltOffset;
                        ey += areaPos.yAltOffset;
                        el.viewRepresentation = this.addField(game, ex, ey, color);
                        finEl = finEl.next;
                    }
                }
                el.viewRepresentation = this.addField(game, areaPos.x, areaPos.y, color);
                // Calculate Position for next field 
                if (i < 8 || i > 11) {
                    areaPos.x += areaPos.xOffset;
                    areaPos.y += areaPos.yOffset;
                } else {
                    areaPos.x += areaPos.xAltOffset;
                    areaPos.y += areaPos.yAltOffset;
                }
                el = el.next;
            }
            pos++;
        }
        const meepleBlue = this.game.add.sprite(this.game.world.centerX, this.game.world.centerY, 'meeple_blue');
        meepleBlue.anchor.setTo(0.5, 0.5);
        meepleBlue.scale.setTo(0.08, 0.08);
        meepleBlue.inputEnabled = true;
        meepleBlue.input.enableDrag();
        meepleBlue.input.enableSnap(40, 40, false, true);
        console.log("meeples parent is: ", meepleBlue.parent);
        meepleBlue.events.onDragStop.add(this.dropLimiter, this);

        // would allow to go to fullscreen on desktop systems
        //this.game.scale.onFullScreenInit.add(GameArea.prototype.onGoFullScreen, this);        
        //this.game.input.onTap.add(() => { this.game.scale.startFullScreen(true); }, this);
    }
    
}
