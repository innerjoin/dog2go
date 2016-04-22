/// <reference path="../Model/TableModel.d.ts"/>
import _phaser = require("phaser");
import BuildUpTypes = require("../Services/buildUpTypes");
import Gfs = require("../Services/GameFieldsService");
import AreaColor = BuildUpTypes.AreaColor;

import coords = require("./FieldCoordinates");
import FieldCoordinatesData = coords.FieldCoordinatesData;
import AreaCoordinates = coords.AreaCoordinates;
import FieldCoordinates = coords.FieldCoordinates;

import MoveDestinationField = BuildUpTypes.MoveDestinationField;
import KennelField = BuildUpTypes.KennelField;
import StartField = BuildUpTypes.StartField;
import GameFieldService = Gfs.GameFieldService;

const scaleFactor = 2;
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
        this.game = new Phaser.Game(scaleFactor * 700, scaleFactor * 700, Phaser.AUTO, "gameContent", gameStates, true);
        const fc = new FieldCoordinates(scaleFactor);
        this.fieldCoordinates = fc.FOUR_PlAYERS;
    }

    gameFieldService: GameFieldService;
    fieldCoordinates: FieldCoordinatesData;

    game: Phaser.Game;
    fields: Phaser.Graphics[] = [];
    allKennelFields: IKennelField[] = [];

    /* load game assets here, but not objects */
    preload() {
        
        this.gameFieldService.getGameFieldData();
        this.fields = [];
        
        this.game.load.image("meeple_blue", "../Frontend/Images/meeple_blue.png");
        this.game.load.image("meeple_red", "../Frontend/Images/meeple_red.png");
        this.game.load.image("meeple_green", "../Frontend/Images/meeple_green.png");
        this.game.load.image("meeple_yellow", "../Frontend/Images/meeple_yellow.png");

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

            let endFields: IEndField[] = this.getEndFields(area.Fields);

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
                    for (let j = 0; j < endFields.length; j++) {
                        ex += areaPos.xAltOffset;
                        ey += areaPos.yAltOffset;
                        finalField.viewRepresentation = this.addField(game, ex, ey, color, finalField.Identifier);
                        finalField = this.getFieldById(finalField.NextIdentifier, area.Fields);
                    }
                }
                current.viewRepresentation = this.addField(game, areaPos.x, areaPos.y, color, current.Identifier);
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
        this.initializeMeeples(gameTable);

        $("#gamePageOverlay").css("display", "none");
        $(".pageOverlayContent > .loading").css("display", "none");
        $(".pageOverlayContent > .switchOrientation").css("display", "block");

        return;
    }

    private initializeMeeples(gameTable: IGameTable) {
        console.log("Initializing Meeples", this.game);
        for (var player of gameTable.PlayerFieldAreas) {
            for (var meeple of player.Meeples) {
                var coordinates: Phaser.Point = this.getMeeplePosition(meeple);
                    console.log("Meeple: ", meeple, this.getSpriteNameForColorCode(meeple.ColorCode), coordinates);
                var spriteName: string = this.getSpriteNameForColorCode(meeple.ColorCode);
                //this.game.add.sprite()
                const meepleSprite = this.game.add.sprite(coordinates.x, coordinates.y, spriteName);
                meepleSprite.anchor.setTo(0.5, 0.5);
                meepleSprite.scale.setTo(scaleFactor * 0.13, scaleFactor * 0.13);
                meepleSprite.inputEnabled = true;
                //meepleSprite.scale.
                meepleSprite.input.enableDrag();
                meepleSprite.input.enableSnap(scaleFactor * 40, scaleFactor * 40, false, true);
                meepleSprite.events.onDragStop.add(this.dropLimiter, this);
            }
        }
    }
 
    getMeeplePosition(meeple: IMeeple): Phaser.Point {
        var result: Phaser.Point;
        var fieldType: string = meeple.CurrentPosition.FieldType;

        switch (fieldType) {
            case "dog2go.Backend.Model.KennelField":
                for (var field of this.allKennelFields) {
                    if (field.Identifier === meeple.CurrentPosition.Identifier) {
                        result = field.viewRepresentation.position;
                        break;
                    }
                }
        default:
        }

        return result;
    }

    getSpriteNameForColorCode(colorCode: number) : string {
        switch (colorCode) {
            case AreaColor.Blue:
                return "meeple_blue";
            case AreaColor.Red:
                return "meeple_red";
            case AreaColor.Green:
                return "meeple_green";
            case AreaColor.Yellow:
                return "meeple_yellow";
            default:
                throw new Error("Color not found for Code: " + colorCode);
        }
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

    getEndFields(fields: IMoveDestinationField[]): IEndField[] {
        var result: IEndField[] = [];
        for (var field of fields) {
            if (field.FieldType.localeCompare("dog2go.Backend.Model.EndField") === 0) {
                result.push(field);
            }
        }
        return result;
    }

    create() {
        console.log("Create GameArea");
        //this.gameFieldService.getGameFieldData();
        return;
    }

    addKennelFields(game: Phaser.Game, kennelFields: IKennelField[], areaPos: AreaCoordinates, color: number) {
        const kennelX = areaPos.x + 11 * areaPos.xOffset;
        const kennelY = areaPos.y + 11 * areaPos.yOffset;
        for (let i = 0; i < kennelFields.length; i++) {
            var kennelField: IKennelField = kennelFields[i];
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
            kennelField.viewRepresentation = this.addField(game, kennelX + xx, kennelY + yy, color, kennelField.Identifier);
            this.allKennelFields.push(kennelField);
        }
    }

    addField(game: Phaser.Game, x: number, y: number, color: number, id?: number): Phaser.Graphics {
        const graphics = game.add.graphics(x, y); // positioning is relative to parent (in this case, to the game world as no parent is defined)
        graphics.beginFill(color, 1);
        graphics.lineStyle(scaleFactor * 2, 0x222222, 1);
        graphics.drawCircle(0, 0, scaleFactor * 30); //draw a circle relative to it's parent (in this case, the graphics object)
        graphics.endFill();
        var style = { font: "20px Arial", fill: "#000000", align: "center" };
        var text = game.make.text(2, 2, id + "", style);
        graphics.addChild(text);
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