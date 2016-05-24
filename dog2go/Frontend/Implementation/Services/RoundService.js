define(["require", "exports"], function (require, exports) {
    "use strict";
    var RoundService = (function () {
        function RoundService(gameTableId) {
            var _this = this;
            this.tableId = gameTableId;
            if (RoundService.instance) {
                // ReSharper disable once TsNotResolved
                throw new Error("Error: GameFieldService instantiation failed. Singleton module! Use .getInstance(_tableId) instead of new.");
            }
            var gameHub = $.connection.gameHub;
            gameHub.client.assignHandCards = function (cards, tableId) {
                // will autoconvert string to int
                // ReSharper disable once CoercedEqualsUsing
                if (gameTableId == tableId) {
                    _this.assignHandCardsCb(cards);
                }
            };
            RoundService.instance = this;
        }
        RoundService.getInstance = function (tableId) {
            // Create new instance if callback is given
            if (RoundService.instance === null || tableId !== RoundService.instance.tableId) {
                RoundService.instance = new RoundService(tableId);
            }
            return RoundService.instance;
        };
        RoundService.instance = null;
        return RoundService;
    }());
    exports.RoundService = RoundService;
});
//# sourceMappingURL=RoundService.js.map