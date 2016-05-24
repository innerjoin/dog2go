define(["require", "exports", "./ChatController", "./GameArea"], function (require, exports, cc, gac) {
    "use strict";
    var ChatController = cc.ChatController;
    var GameArea = gac.GameArea;
    var GameMaster = (function () {
        function GameMaster(tableId) {
            this.chatController = new ChatController(tableId);
            this.gameArea = new GameArea(tableId);
        }
        return GameMaster;
    }());
    exports.GameMaster = GameMaster;
});
//# sourceMappingURL=GameMaster.js.map