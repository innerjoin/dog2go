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
    public gameFieldService: GameFieldService;
    private gameFieldController: GameFieldController;
    private cardsController: CardsController;
    private meepleController: MeepleController;
    private game: Phaser.Game;
    private gameTableId: number;

    constructor(tableId: number, isTesting?: boolean) {
        console.log(_phaser, _savecpu); // DO NOT remove this statement, as import will not properly work else
        const gameStates = {
            init: this.init.bind(this),
            preload: this.preload.bind(this)
        };
        this.gameTableId = tableId;
        this.game = new Phaser.Game(scaleFactor * 700, scaleFactor * 700, Phaser.AUTO, "gameContent", gameStates, true);

        if (!isTesting) { 
            this.gameFieldService = GameFieldService.getInstance(tableId);
            this.gameFieldService.createGameTableCb = this.buildFields.bind(this);
            this.gameFieldService.notifyAllGameIsFinishedCb = this.notifyAllGameIsFinished.bind(this);
            this.gameFieldController = new GameFieldController(this.game, scaleFactor);
            this.meepleController = new MeepleController(tableId, this.game, this.gameFieldController, scaleFactor);
            this.cardsController = new CardsController(tableId, this.meepleController);
        }
    }

    public init() {
        this.game.plugins.add(Phaser.Plugin.SaveCPU);
    }

    // load game assets here, but not objects 
    public preload() {
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
    }

    public buildFields = (gameTable: IGameTable) => {
        this.gameFieldController.buildFields(gameTable);
        this.meepleController.initializeMeeples(gameTable);

        $("#gamePageOverlay").css("display", "none");
        $(".pageOverlayContent > .loading").css("display", "none");
        $(".pageOverlayContent > .switchOrientation").css("display", "block");
        return;
    }

    public notifyAllGameIsFinished(text: string) {
        var finishedField = $(".pageOverlayContent > .finished");
        $(".pageOverlayContent > .switchOrientation").css("display", "none");
        $("#gamePageOverlay").css("display", "block");

        finishedField.css("display", "block");
        $(".pageOverlayContent > .finished > h1").text(text);
    }

    public enterIncorrectOrientation() {
        $("#gamePageOverlay").css("display", "flex");
    }

    public leaveIncorrectOrientation() {
        $("#gamePageOverlay").css("display", "none");
    }
}