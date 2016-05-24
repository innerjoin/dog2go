define(["require", "exports"], function (require, exports) {
    "use strict";
    var GameFieldService = (function () {
        function GameFieldService(tableId) {
            var _this = this;
            this.tableId = tableId;
            if (GameFieldService.instance) {
                // ReSharper disable once TsNotResolved
                throw new Error("Error: GameFieldService instantiation failed. Singleton module! Use .getInstance() instead of new.");
            }
            var gameHub = $.connection.gameHub;
            $.connection.hub.qs = "tableId=" + tableId;
            gameHub.client.createGameTable = function (gameTable, _tableId) {
                // will autoconvert string to int
                // ReSharper disable once CoercedEqualsUsing
                if (_tableId == tableId) {
                    _this.createGameTableCb(gameTable);
                }
            };
            gameHub.client.backToGame = function (gameTable, cards, _tableId) {
                // will autoconvert string to int
                // ReSharper disable once CoercedEqualsUsing
                if (_tableId == tableId) {
                    console.log("our this context: ", _this);
                    _this.createGameTableCb(gameTable);
                    _this.assignHandCardsCb(cards);
                }
            };
            GameFieldService.instance = this;
        }
        GameFieldService.getInstance = function (tableId) {
            // Create new instance if callback is given
            if (GameFieldService.instance === null || GameFieldService.instance.tableId !== tableId) {
                new GameFieldService(tableId);
            }
            return GameFieldService.instance;
        };
        GameFieldService.prototype.getGameFieldData = function (tableId) {
            var gameHub = $.connection.gameHub;
            $.connection.hub.start().done(function () {
                gameHub.server.connectToTable(tableId);
            });
        };
        GameFieldService.instance = null;
        return GameFieldService;
    }());
    exports.GameFieldService = GameFieldService;
});
//# sourceMappingURL=GameFieldsService.js.map