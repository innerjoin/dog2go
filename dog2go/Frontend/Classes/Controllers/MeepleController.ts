
import ts = require("../Services/TurnService");
import TurnService = ts.TurnService;

import gm = require("../Model/GameModel");
import AreaColor = gm.AreaColor;

import gfc = require("./GameFieldsController");
import GameFieldController = gfc.GameFieldController;

export class MeepleController {
    private turnService: TurnService;
    private game: Phaser.Game;

    private fields: Phaser.Graphics[] = [];
    private gameFieldController: GameFieldController;

    private allMeeples: IMeeple[];

    private playerMeepleColor: number;
    private turnCardMove: ICardMove;

    private scaleFactor: number;

    constructor(game: Phaser.Game, gameFieldController: GameFieldController, scaleFactor: number) {
        this.scaleFactor = scaleFactor;
        this.gameFieldController = gameFieldController;
        this.game = game;

        this.turnService = TurnService.getInstance();
        this.turnService.notifyActualPlayerCB = this.notifyActualPlayer.bind(this);
        this.turnService.sendMeeplePositionsCB = this.sendMeeplesPositions.bind(this);

        this.allMeeples = [];
    }

    public notifyActualPlayer(possibleCards: ICard[], meepleColor: number) {
        console.log("notifyActualPlayer(), poss Cards: ", possibleCards, meepleColor);
        this.playerMeepleColor = meepleColor;

    }

    public sendMeeplesPositions(meeples: IMeeple[]) {
        console.log("sendMeeplesPositions(), meeples: ", meeples);
    }

    public initializeMeeples(gameTable: IGameTable) {
        console.log("Initializing Meeples", gameTable);
        this.allMeeples = [];
        for (var player of gameTable.PlayerFieldAreas) {
            for (var meeple of player.Meeples) {
                var coordinates: Phaser.Point = this.getMeeplePosition(meeple);
                var spriteName: string = this.getSpriteNameForColorCode(meeple.ColorCode);

                var meepleSprite: Phaser.Sprite = this.game.add.sprite(coordinates.x, coordinates.y, spriteName);
                meepleSprite.anchor.setTo(0.5, 0.5);
                meepleSprite.scale.setTo(this.scaleFactor * 0.13, this.scaleFactor * 0.13);
                meepleSprite.inputEnabled = true;

                meeple.spriteRepresentation = meepleSprite;
                this.allMeeples.push(meeple);
            }
        }
    }

    public proceedMeepleTurn(turnCardMove: ICardMove) {
        this.turnCardMove = turnCardMove;
        var meeples: IMeeple[] = this.getMeeplesByColor(this.playerMeepleColor);
        for (var meeple of meeples) {
            // TODO: add distinction for blocked meeples
            meeple.spriteRepresentation.input.enableDrag();
            meeple.spriteRepresentation.input.enableSnap(this.scaleFactor * 40, this.scaleFactor * 40, false, true);
            meeple.spriteRepresentation.events.onDragStop.add(this.dropLimiter, this, 0, meeple);
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

    private getMeeplesByColor(color: number): IMeeple[] {
        var result: IMeeple[] = [];
        for (var meeple of this.allMeeples) {
            if (meeple.ColorCode === color) {
                result.push(meeple);
            }
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

    dropLimiter(item: Phaser.Sprite, pointer: Phaser.Pointer, meeple: IMeeple) {
        var nearest: IMoveDestinationField;
        var smallest = Number.MAX_VALUE;
        var pos = item.world;
        this.gameFieldController.getAllFields.forEach((field) => {
            var fieldPos = field.viewRepresentation.world;
            var dist = fieldPos.distance(pos, true);
            if (!(smallest < dist)) {
                smallest = dist;
                nearest = field;
            }
        });
        if (nearest != null) {
            item.x = nearest.viewRepresentation.x;
            item.y = nearest.viewRepresentation.y;
            if (meeple.CurrentPosition.Identifier !== nearest.Identifier) {
                var cardMoove: ICardMove;
                var meepleMove: IMeepleMove = { Meeple: meeple, MoveDestination: nearest, DestinationFieldId: nearest.Identifier };

                console.log(nearest);
                this.turnService.validateMove(meepleMove, this.turnCardMove);
                this.turnCardMove = null;
            }
        }
    }


}