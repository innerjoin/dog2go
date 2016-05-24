define(["require", "exports", "../Services/TurnService", "../Model/GameModel"], function (require, exports, ts, gm) {
    "use strict";
    var TurnService = ts.TurnService;
    var AreaColor = gm.AreaColor;
    var MeepleController = (function () {
        function MeepleController(tableId, game, gameFieldController, scaleFactor) {
            this.fields = [];
            this.scaleFactor = scaleFactor;
            this.gameFieldController = gameFieldController;
            this.game = game;
            this.tableId = tableId;
            this.turnService = TurnService.getInstance(tableId);
            this.turnService.notifyActualPlayerCb = this.notifyActualPlayer.bind(this);
            this.turnService.sendMeeplePositionsCb = this.repositionMeeples.bind(this);
            this.turnService.returnMoveCb = this.returnMove.bind(this);
            this.allMeeples = [];
        }
        MeepleController.prototype.notifyActualPlayer = function (possibleCards, meepleColor) {
            this.playerMeepleColor = meepleColor;
        };
        MeepleController.prototype.returnMove = function () {
            if (this.turnMeepleMove != null) {
                this.positionMeeple(this.turnMeepleMove.Meeple);
                this.turnMeepleMove = null;
            }
        };
        MeepleController.prototype.repositionMeeples = function (meeples) {
            this.turnCardMove = null;
            this.turnMeepleMove = null;
            for (var _i = 0, _a = this.allMeeples; _i < _a.length; _i++) {
                var meeple = _a[_i];
                for (var _b = 0, meeples_1 = meeples; _b < meeples_1.length; _b++) {
                    var newMeeple = meeples_1[_b];
                    if (newMeeple.Identifier === meeple.Identifier) {
                        meeple.CurrentPosition = newMeeple.CurrentPosition;
                        this.disableAllMeeplesDraggable();
                        this.positionMeeple(meeple);
                        break;
                    }
                }
            }
        };
        MeepleController.prototype.positionMeeple = function (meeple) {
            if (meeple.spriteRepresentation && meeple.CurrentPosition) {
                var coordinates = this.gameFieldController.getFieldPosition(meeple.CurrentPosition.Identifier);
                this.game.add.tween(meeple.spriteRepresentation).to(coordinates, 500, Phaser.Easing.Quadratic.InOut, true);
            }
        };
        MeepleController.prototype.initializeMeeples = function (gameTable) {
            this.allMeeples = [];
            for (var _i = 0, _a = gameTable.PlayerFieldAreas; _i < _a.length; _i++) {
                var player = _a[_i];
                for (var _b = 0, _c = player.Meeples; _b < _c.length; _b++) {
                    var meeple = _c[_b];
                    var spriteName = this.getSpriteNameForColorCode(meeple.ColorCode);
                    var meepleSprite = this.game.add.sprite(this.game.width / 2, this.game.height / 2, spriteName);
                    meepleSprite.anchor.setTo(0.5, 0.5);
                    meepleSprite.scale.setTo(this.scaleFactor * 0.21, this.scaleFactor * 0.17);
                    meepleSprite.inputEnabled = true;
                    meeple.spriteRepresentation = meepleSprite;
                    this.positionMeeple(meeple);
                    this.allMeeples.push(meeple);
                }
            }
        };
        MeepleController.prototype.proceedMeepleTurn = function (turnCardMove) {
            this.turnCardMove = turnCardMove;
            var meeples = this.getMeeplesByColor(this.playerMeepleColor);
            for (var _i = 0, meeples_2 = meeples; _i < meeples_2.length; _i++) {
                var meeple = meeples_2[_i];
                this.setMeepleDraggable(meeple, true);
            }
        };
        MeepleController.prototype.disableAllMeeplesDraggable = function () {
            for (var _i = 0, _a = this.allMeeples; _i < _a.length; _i++) {
                var meeple = _a[_i];
                this.setMeepleDraggable(meeple, false);
            }
        };
        MeepleController.prototype.setMeepleDraggable = function (meeple, setDraggable) {
            if (!setDraggable) {
                meeple.spriteRepresentation.input.disableDrag();
            }
            else {
                meeple.spriteRepresentation.input.enableDrag();
                meeple.spriteRepresentation.input.enableSnap(this.scaleFactor * 40, this.scaleFactor * 40, false, true);
                meeple.spriteRepresentation.events.onDragStop.add(this.dropLimiter, this, 0, meeple);
            }
        };
        MeepleController.prototype.getMeeplesByColor = function (color) {
            var result = [];
            for (var _i = 0, _a = this.allMeeples; _i < _a.length; _i++) {
                var meeple = _a[_i];
                if (meeple.ColorCode === color) {
                    result.push(meeple);
                }
            }
            return result;
        };
        MeepleController.prototype.getSpriteNameForColorCode = function (colorCode) {
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
        };
        MeepleController.prototype.dropLimiter = function (item, pointer, meeple) {
            var nearest;
            var smallest = Number.MAX_VALUE;
            var pos = item.world;
            this.gameFieldController.getAllFields.forEach(function (field) {
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
                    this.turnMeepleMove = { Meeple: meeple, MoveDestination: nearest, DestinationFieldId: nearest.Identifier };
                    this.turnService.validateMove(this.turnMeepleMove, this.turnCardMove, this.tableId);
                }
            }
            else {
                var currentField = this.gameFieldController.getFieldByIdOfAll(meeple.CurrentPosition.Identifier);
                if (typeof currentField !== "undefined" && currentField !== null) {
                    item.x = currentField.viewRepresentation.x;
                    item.y = currentField.viewRepresentation.y;
                }
            }
        };
        return MeepleController;
    }());
    exports.MeepleController = MeepleController;
});
//# sourceMappingURL=MeepleController.js.map