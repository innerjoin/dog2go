define(["require", "exports", "phaser", "savecpu", "../Services/GameFieldsService", "./GameFieldsController", "./CardsController", "./MeepleController"], function (require, exports, _phaser, _savecpu, gfs, gfc, cc, mc) {
    "use strict";
    var GameFieldService = gfs.GameFieldService;
    var GameFieldController = gfc.GameFieldController;
    var CardsController = cc.CardsController;
    var MeepleController = mc.MeepleController;
    var scaleFactor = 1.5;
    var GameArea = (function () {
        function GameArea(tableId, isTesting) {
            var _this = this;
            this.buildFields = function (gameTable) {
                _this.gameFieldController.buildFields(gameTable);
                _this.meepleController.initializeMeeples(gameTable);
                $("#gamePageOverlay").css("display", "none");
                $(".pageOverlayContent > .loading").css("display", "none");
                $(".pageOverlayContent > .switchOrientation").css("display", "block");
                return;
            };
            console.log(_phaser, _savecpu); // DO NOT remove this statement, as import will not properly work else
            var gameStates = {
                init: this.init.bind(this),
                preload: this.preload.bind(this)
            };
            this.gameTableId = tableId;
            this.game = new Phaser.Game(scaleFactor * 700, scaleFactor * 700, Phaser.AUTO, "gameContent", gameStates, true);
            if (!isTesting) {
                this.gameFieldService = GameFieldService.getInstance(tableId);
                this.gameFieldService.createGameTableCb = this.buildFields.bind(this);
                this.gameFieldController = new GameFieldController(this.game, scaleFactor);
                this.meepleController = new MeepleController(tableId, this.game, this.gameFieldController, scaleFactor);
                this.cardsController = new CardsController(tableId, this.meepleController);
            }
        }
        GameArea.prototype.init = function () {
            this.game.plugins.add(Phaser.Plugin.SaveCPU);
        };
        // load game assets here, but not objects 
        GameArea.prototype.preload = function () {
            this.gameFieldService.getGameFieldData(this.gameTableId);
            this.game.load.image("meeple_blue", "/Frontend/Images/meeple_blue.png");
            this.game.load.image("meeple_red", "/Frontend/Images/meeple_red.png");
            this.game.load.image("meeple_green", "/Frontend/Images/meeple_green.png");
            this.game.load.image("meeple_yellow", "/Frontend/Images/meeple_yellow.png");
            this.game.scale.scaleMode = Phaser.ScaleManager.SHOW_ALL;
            this.game.scale.refresh();
            if (!this.game.device.desktop) {
                this.game.scale.forceOrientation(true, false);
                this.game.scale.enterIncorrectOrientation.add(this.enterIncorrectOrientation, this);
                this.game.scale.leaveIncorrectOrientation.add(this.leaveIncorrectOrientation, this);
            }
        };
        GameArea.prototype.enterIncorrectOrientation = function () {
            $("#gamePageOverlay").css("display", "flex");
        };
        GameArea.prototype.leaveIncorrectOrientation = function () {
            $("#gamePageOverlay").css("display", "none");
        };
        return GameArea;
    }());
    exports.GameArea = GameArea;
});
//# sourceMappingURL=GameArea.js.map