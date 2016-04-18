import _phaser = require("phaser");
import Coordinates = require("./FieldCoordinates");
import BuildUpTypes = require("../Services/buildUpTypes");
import Gfs = require("../Services/GameFieldsService");
import AreaColor = BuildUpTypes.AreaColor;
import FieldCoordinatesData = Coordinates.FieldCoordinatesData;
import AreaCoordinates = Coordinates.AreaCoordinates;
import PlayerFieldArea = BuildUpTypes.PlayerFieldArea;
import MoveDestinationField = BuildUpTypes.MoveDestinationField;
import KennelField = BuildUpTypes.KennelField;
import StartField = BuildUpTypes.StartField;
import GameFieldService = Gfs.GameFieldService;

export class GameArea {

    constructor(isTesting?: boolean) {
        if (!isTesting) {
            this.gameFieldService = GameFieldService.getInstance(this.buildFields.bind(this));
        }
        //var chat = new ChatController();
        //this.gameFieldService = GameFieldsService.GameFieldService.getInstance(this.buildFields.bind(this));
        const gameStates = {
            preload: this.preload.bind(this),
            create: this.create.bind(this)
        };
        console.log(_phaser);
        this.game = new Phaser.Game(2100, 2100, Phaser.AUTO, "gameContent", gameStates, true);
        const fc = new Coordinates.FieldCoordinates();
        this.fieldCoordinates = fc.FOUR_PlAYERS;
    }

    gameFieldService: GameFieldService;
    fieldCoordinates: FieldCoordinatesData;

    game: Phaser.Game;
    areas: BuildUpTypes.PlayerFieldArea[] = [];
    fields: Phaser.Graphics[] = [];

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
        
        console.log("Going to load GameFields");
        this.gameFieldService.getGameFieldData();
        console.log("GFS_Command Out");
        this.areas = GameArea.addTestData();
        this.fields = [];
        
        this.game.load.image("meeple_blue", "../Frontend/Images/pawn_blue.png");
        this.game.scale.scaleMode = Phaser.ScaleManager.SHOW_ALL;
        this.game.scale.refresh();
        if (!this.game.device.desktop) {
            this.game.scale.forceOrientation(true, false);
            this.game.scale.setResizeCallback(this.gameResized, this);
            this.game.scale.enterIncorrectOrientation.add(this.enterIncorrectOrientation, this);
            this.game.scale.leaveIncorrectOrientation.add(this.leaveIncorrectOrientation, this);
        }
    }

    public buildFields = (gameTable: IGameTable) => {
        // Source of Create() when good data comes from server

        var game = this.game;
        let currentPos = 0;
        for (var k = 0; k < gameTable.PlayerFieldAreas.length; k++) {
            var area: IPlayerFieldArea = gameTable.PlayerFieldAreas[k];
            var current: IMoveDestinationField = area.Fields[0];
            const areaPos = this.fieldCoordinates.getAreaCoordinates(currentPos);
            // create kennel fields           
            this.addKennelFields(this.game, area.KennelFields, areaPos, area.ColorCode);
            var fieldNr = 0;

            // create destination fields
            while (current) {
                var color = 0xeeeeee;
                if (current.Identifier === area.StartField.Identifier) {
                    
                    var startField: IStartField = <IStartField>current;
                    color = area.ColorCode;
                    let ex = areaPos.x;
                    let ey = areaPos.y;
                    
                    // Generate Endfields from startfield
                    var finalField = startField.EndFieldEntry;
                    /*for (let j = 0; j < area.EndFields.length; j++) {
                        ex += areaPos.xAltOffset;
                        ey += areaPos.yAltOffset;
                        finalField.viewRepresentation = this.addField(game, ex, ey, color);
                        finalField = this.getFieldById(finalField.nextIdentifier, area.Fields);
                    }*/
                }
                current.viewRepresentation = this.addField(game, areaPos.x, areaPos.y, color);
                // Calculate Position for next field 
                if (fieldNr < 8 || fieldNr > 11) {
                    areaPos.x += areaPos.xOffset;
                    areaPos.y += areaPos.yOffset;
                } else {
                    areaPos.x += areaPos.xAltOffset;
                    areaPos.y += areaPos.yAltOffset;
                }
                current = this.getFieldById(current.NextIdentifier, area.Fields);
                fieldNr++;
                // TODO: Put Meeples on field
            }
            currentPos++;
        }

        // Single Meeple on field
        const meepleBlue = this.game.add.sprite(this.game.world.centerX, this.game.world.centerY, 'meeple_blue');
        meepleBlue.anchor.setTo(0.5, 0.5);
        meepleBlue.scale.setTo(0.4, 0.4);
        meepleBlue.inputEnabled = true;
        meepleBlue.input.enableDrag();
        meepleBlue.input.enableSnap(120, 120, false, true);
        meepleBlue.events.onDragStop.add(this.dropLimiter, this);
        console.log("GameArea: Finished Building");
        return;
        /*var areas: IPlayerFieldArea[] = gameTable.PlayerFieldAreas;
        
        for (let area of areas) {
            
            let field = area.Fields[0];
            const areaPos = this.fieldCoordinates.getAreaCoordinates(pos);
            
            // create kennel fields           
            this.addKennelFields(this.game, area.KennelFields, areaPos, area.ColorCode);

            // create destination fields
            for (let i = 0; i < area.Fields.length; i++) {
                var color = 0xeeeeee;
                if (field instanceof StartField) {
                    color = area.ColorCode;
                    let ex = areaPos.x;
                    let ey = areaPos.y;
                    let finEl = field.endFieldEntry;
                    for (let j = 0; j < area.EndFields.length; j++) {
                        ex += areaPos.xAltOffset;
                        ey += areaPos.yAltOffset;
                        field.viewRepresentation = this.addField(game, ex, ey, color);
                        finEl = finEl.next;
                    }
                }
                field.viewRepresentation = this.addField(game, areaPos.x, areaPos.y, color);
                // Calculate Position for next field 
                if (i < 8 || i > 11) {
                    areaPos.x += areaPos.xOffset;
                    areaPos.y += areaPos.yOffset;
                } else {
                    areaPos.x += areaPos.xAltOffset;
                    areaPos.y += areaPos.yAltOffset;
                }
                field = this.getFieldById(field.nextIdentifier, area.Fields);
                field = field.next;
            }
            pos++;
        }*/
    }

    gameResized(width, height) {
        //  This could be handy if you need to do any extra processing if the game resizes.
        //  A resize could happen if for example swapping orientation on a device or resizing the browser window.
        //  Note that this callback is only really useful if you use a ScaleMode of RESIZE and place it inside your main game state.
    }

    enterIncorrectOrientation() {
        $("#gamePageOverlay").css("display", "flex");
    }

    leaveIncorrectOrientation() {
        $("#gamePageOverlay").css("display", "none");
    }

    getFieldById(id: number, fields: IMoveDestinationField[]): IMoveDestinationField {
        for (let field of fields) {
            if (id === field.Identifier) {
                return field;
            }
        }
        return null;
    }

    create() {
        console.log("Create GameArea");
        //this.gameFieldService.getGameFieldData();
        return;
        var game = this.game;
        let pos = 0;

        for (let area of this.areas) {
            let el = area.gameFields[0];
            const areaPos = this.fieldCoordinates.getAreaCoordinates(pos);
            
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
        meepleBlue.scale.setTo(0.4, 0.4);
        meepleBlue.inputEnabled = true;
        meepleBlue.input.enableDrag();
        meepleBlue.input.enableSnap(120, 120, false, true);
        meepleBlue.events.onDragStop.add(this.dropLimiter, this);
    }
    addKennelFields(game: Phaser.Game, kennelFields: IKennelField[], areaPos: AreaCoordinates, color: number) {
        const kennelX = areaPos.x + 11 * areaPos.xOffset;
        const kennelY = areaPos.y + 11 * areaPos.yOffset;
        for (let i = 0; i < kennelFields.length; i++) {
            let xx = 0;
            let yy = 0;
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
            kennelFields[i].viewRepresentation = this.addField(game, kennelX + xx, kennelY + yy, color);
        }
    }

    addField(game: Phaser.Game, x: number, y: number, color: number): Phaser.Graphics {
        const graphics = game.add.graphics(x, y); // positioning is relative to parent (in this case, to the game world as no parent is defined)
        graphics.beginFill(color, 1);
        graphics.lineStyle(6, 0x222222, 1);
        graphics.drawCircle(0, 0, 90); //draw a circle relative to it's parent (in this case, the graphics object)
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
   
}