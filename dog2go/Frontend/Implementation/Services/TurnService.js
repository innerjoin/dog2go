define(["require", "exports"], function (require, exports) {
    "use strict";
    var TurnService = (function () {
        function TurnService(gameTableId) {
            var _this = this;
            this.tableId = gameTableId;
            if (TurnService.instance) {
                // ReSharper disable once TsNotResolved
                throw new Error("Error: GameFieldService instantiation failed. Singleton module! Use .getInstance(_tableId) instead of new.");
            }
            var gameHub = $.connection.gameHub;
            gameHub.client.notifyActualPlayer = function (possibleCards, meepleColor, tableId) {
                // will autoconvert string to int
                // ReSharper disable once CoercedEqualsUsing
                if (gameTableId == tableId) {
                    _this.notifyActualPlayerCb(possibleCards, meepleColor);
                    _this.notifyActualPlayerCardsCb(possibleCards, meepleColor);
                }
            };
            gameHub.client.sendMeeplePositions = function (meeples, tableId) {
                // will autoconvert string to int
                // ReSharper disable once CoercedEqualsUsing
                if (gameTableId == tableId) {
                    _this.sendMeeplePositionsCb(meeples);
                }
            };
            gameHub.client.dropCards = function (tableId) {
                // will autoconvert string to int
                // ReSharper disable once CoercedEqualsUsing
                if (gameTableId == tableId) {
                    _this.dropCardsCb();
                }
            };
            gameHub.client.returnMove = function (tableId) {
                // will autoconvert string to int
                // ReSharper disable once CoercedEqualsUsing
                if (gameTableId == tableId) {
                    _this.returnMoveCb();
                }
            };
            TurnService.instance = this;
        }
        TurnService.getInstance = function (tableId) {
            // Create new instance if callback is given
            if (TurnService.instance === null || tableId !== TurnService.instance.tableId) {
                TurnService.instance = new TurnService(tableId);
            }
            return TurnService.instance;
        };
        TurnService.prototype.validateMove = function (meepleMove, cardMove, tableId) {
            var mMoveReady = $.extend({}, meepleMove);
            mMoveReady.Meeple = $.extend({}, meepleMove.Meeple);
            mMoveReady.Meeple.CurrentFieldId = meepleMove.Meeple.CurrentPosition.Identifier;
            delete mMoveReady.Meeple.spriteRepresentation;
            delete mMoveReady.Meeple.CurrentPosition;
            mMoveReady.DestinationFieldId = meepleMove.MoveDestination.Identifier;
            delete mMoveReady.MoveDestination;
            var gameHub = $.connection.gameHub;
            $.connection.hub.start().done(function () {
                gameHub.server.validateMove(mMoveReady, cardMove, tableId);
            });
        };
        TurnService.instance = null;
        return TurnService;
    }());
    exports.TurnService = TurnService;
});
//# sourceMappingURL=TurnService.js.map