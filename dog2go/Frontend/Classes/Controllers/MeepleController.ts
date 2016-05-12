
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
    private turnMeepleMove: IMeepleMove;

    private scaleFactor: number;

    constructor(game: Phaser.Game, gameFieldController: GameFieldController, scaleFactor: number) {
        this.scaleFactor = scaleFactor;
        this.gameFieldController = gameFieldController;
        this.game = game;

        this.turnService = TurnService.getInstance();
        this.turnService.notifyActualPlayerCB = this.notifyActualPlayer.bind(this);
        this.turnService.sendMeeplePositionsCB = this.repositionMeeples.bind(this);
        this.turnService.returnMoveCB = this.returnMove.bind(this);

        this.allMeeples = [];
    }

    public notifyActualPlayer(possibleCards: ICard[], meepleColor: number) {
        console.log("notifyActualPlayer(), poss Cards: ", possibleCards, meepleColor);
        this.playerMeepleColor = meepleColor;

    }

    public returnMove() {
        console.log("Return move");
        if (this.turnMeepleMove != null) {
            this.positionMeeple(this.turnMeepleMove.Meeple);
        }
    }

    public repositionMeeples(meeples: IMeeple[]) {
        for (var meeple of this.allMeeples) {
            for (var newMeeple of meeples) {
                if (newMeeple.Identifier === meeple.Identifier) {
                    meeple.CurrentPosition = newMeeple.CurrentPosition;
                    this.positionMeeple(meeple);
                    break;
                }
            }
        }
        console.log("sendMeeplesPositions(), meeples: ", meeples);
    }

    public positionMeeple(meeple: IMeeple) {
        if (meeple.spriteRepresentation && meeple.CurrentPosition) {
            var coordinates: Phaser.Point = this.gameFieldController.getFieldPosition(meeple.CurrentPosition.Identifier);
            this.game.add.tween(meeple.spriteRepresentation).to(coordinates, 500, Phaser.Easing.Quadratic.InOut, true);
        }
    }

    public initializeMeeples(gameTable: IGameTable) {
        console.log("Initializing Meeples", gameTable);
        this.allMeeples = [];
        for (var player of gameTable.PlayerFieldAreas) {
            for (var meeple of player.Meeples) {
                var spriteName: string = this.getSpriteNameForColorCode(meeple.ColorCode);

                //var meepleSprite: Phaser.Sprite = this.game.add.sprite(coordinates.x, coordinates.y, spriteName);
                var meepleSprite: Phaser.Sprite = this.game.add.sprite(this.game.width / 2, this.game.height / 2, spriteName);
                meepleSprite.anchor.setTo(0.5, 0.5);
                meepleSprite.scale.setTo(this.scaleFactor * 0.13, this.scaleFactor * 0.13);
                meepleSprite.inputEnabled = true;

                meeple.spriteRepresentation = meepleSprite;

                this.positionMeeple(meeple);

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
        if (nearest != null && this.gameFieldController.isValidTargetField(nearest)) {
            item.x = nearest.viewRepresentation.x;
            item.y = nearest.viewRepresentation.y;
            if (meeple.CurrentPosition.Identifier !== nearest.Identifier) {
                var cardMoove: ICardMove;
                this.turnMeepleMove = { Meeple: meeple, MoveDestination: nearest, DestinationFieldId: nearest.Identifier };

                console.log(nearest);
                this.turnService.validateMove(this.turnMeepleMove, this.turnCardMove);
            }
        } else {
            console.log("Meeplepos: ", pointer);
            var currentField = this.gameFieldController.getFieldByIdOfAll(meeple.CurrentPosition.Identifier);
            if (currentField !== null) {
                item.x = currentField.viewRepresentation.x;
                item.y = currentField.viewRepresentation.y;
            }
        }
    }


}