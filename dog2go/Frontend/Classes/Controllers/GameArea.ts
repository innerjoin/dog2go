/// <reference path="../Model/TableModel.d.ts"/>
import _phaser = require("phaser");
import _savecpu = require("savecpu");

import gfs = require("../Services/GameFieldsService");
import GameFieldService = gfs.GameFieldService;

import gfc = require("./GameFieldsController");
import GameFieldController = gfc.GameFieldController;

import cc = require("./CardsController");
import CardsController = cc.CardsController;

import mc = require("./MeepleController");
import MeepleController = mc.MeepleController;

const scaleFactor = 1.5;

export class GameArea {
    private gameFieldService: GameFieldService;
    private gameFieldController: GameFieldController;
    private cardsController: CardsController;
    private meepleController: MeepleController;
    private game: Phaser.Game;
    private gameTableId: number;

    constructor(tableId: number, isTesting?: boolean) {
        console.log(_phaser, _savecpu);
        const gameStates = {
            init: this.init.bind(this),
            preload: this.preload.bind(this),
            create: this.create.bind(this)
        };
        this.gameTableId = tableId;
        this.game = new Phaser.Game(scaleFactor * 700, scaleFactor * 700, Phaser.AUTO, "gameContent", gameStates, true);

        //this.game.plugins.add(new Phaser.Plugin.SaveCPU(this.game, null));
        //this.game.plugins.add(Phaser.Plugin.SaveCPU);

        if (!isTesting) {
            this.gameFieldService = GameFieldService.getInstance();
            this.gameFieldService.createGameTableCb = this.buildFields.bind(this);
            this.gameFieldController = new GameFieldController(this.game, scaleFactor);
            this.meepleController = new MeepleController(this.game, this.gameFieldController, scaleFactor);
            this.cardsController = new CardsController(this.meepleController);
        }
    }

    init() {
        console.log("this.game.plugins", this.game.plugins);
        console.log("Phaser.Plugin", Phaser.Plugin.SaveCPU);
        this.game.plugins.add(Phaser.Plugin.SaveCPU);
    }

    /* load game assets here, but not objects */
    preload() {
        
        this.gameFieldService.getGameFieldData(this.gameTableId);
        
        this.game.load.image("meeple_blue", "/Frontend/Images/meeple_blue.png");
        this.game.load.image("meeple_red", "/Frontend/Images/meeple_red.png");
        this.game.load.image("meeple_green", "/Frontend/Images/meeple_green.png");
        this.game.load.image("meeple_yellow", "/Frontend/Images/meeple_yellow.png");

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
        this.gameFieldController.buildFields(gameTable);
        this.meepleController.initializeMeeples(gameTable);

        $("#gamePageOverlay").css("display", "none");
        $(".pageOverlayContent > .loading").css("display", "none");
        $(".pageOverlayContent > .switchOrientation").css("display", "block");

        return;
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
    
    create() {
        console.log("Create GameArea");
        //this.gameFieldService.getGameFieldData();
        return;
    }
}