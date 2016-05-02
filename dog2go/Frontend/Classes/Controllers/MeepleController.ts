
import ts = require("../Services/TurnService");
import TurnService = ts.TurnService;

import gm = require("../Model/GameModel");
import AreaColor = gm.AreaColor;


import gfc = require("./GameFieldsController");
import GameFieldController = gfc.GameFieldController;

export class MeepleController {
    private turnService: TurnService;
    private game: Phaser.Game;

    //private allKennelFields: IKennelField[] = [];
    private fields: Phaser.Graphics[] = [];
    private gameFieldController: GameFieldController;

    private scaleFactor: number;

    constructor(game: Phaser.Game, gameFieldController: GameFieldController, scaleFactor: number) {
        this.scaleFactor = scaleFactor;
        this.gameFieldController = gameFieldController;
        this.game = game;

        this.turnService = TurnService.getInstance();
        this.turnService.notifyActualPlayerCB = this.notifyActualPlayer;
        this.turnService.sendMeeplePositionsCB = this.sendMeeplesPositions;

    }

    public notifyActualPlayer(possibleCards: ICard[]) {
        console.log("notifyActualPlayer(), poss Cards: ", possibleCards);
        
    }

    public sendMeeplesPositions(meeples: IMeeple[]) {
        console.log("sendMeeplesPositions(), meeples: ", meeples);
    }

    public initializeMeeples(gameTable: IGameTable) {
        console.log("Initializing Meeples", this.game);
        for (var player of gameTable.PlayerFieldAreas) {
            for (var meeple of player.Meeples) {
                var coordinates: Phaser.Point = this.getMeeplePosition(meeple);
                //  console.log("Meeple: ", meeple, this.getSpriteNameForColorCode(meeple.ColorCode), coordinates);
                var spriteName: string = this.getSpriteNameForColorCode(meeple.ColorCode);
                //this.game.add.sprite()
                const meepleSprite = this.game.add.sprite(coordinates.x, coordinates.y, spriteName);
                meepleSprite.anchor.setTo(0.5, 0.5);
                meepleSprite.scale.setTo(this.scaleFactor * 0.13, this.scaleFactor * 0.13);
                meepleSprite.inputEnabled = true;
                //meepleSprite.scale.
                meepleSprite.input.enableDrag();
                meepleSprite.input.enableSnap(this.scaleFactor * 40, this.scaleFactor * 40, false, true);
                meepleSprite.events.onDragStop.add(this.dropLimiter, this);
            }
        }
    }

    getMeeplePosition(meeple: IMeeple): Phaser.Point {
        var result: Phaser.Point;
        var fieldType: string = meeple.CurrentPosition.FieldType;

        switch (fieldType) {
            case "dog2go.Backend.Model.KennelField":
                for (var field of this.gameFieldController.getKennelFields) {
                    if (field.Identifier === meeple.CurrentPosition.Identifier) {
                        result = field.viewRepresentation.position;
                        break;
                    }
                }
            default:
        }

        return result;
    }

    getSpriteNameForColorCode(colorCode: number): string {
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

    dropLimiter(item: Phaser.Sprite) {
        var nearest: Phaser.Graphics;
        var smallest = Number.MAX_VALUE;
        var pos = item.world;
        this.gameFieldController.getAllFields.forEach((field) => {
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