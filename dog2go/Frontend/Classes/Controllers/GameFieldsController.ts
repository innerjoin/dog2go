///<reference path="../../Library/JQuery/jqueryui.d.ts"/>

import gfs = require("../Services/GameFieldsService");
import GameFieldService = gfs.GameFieldService;

export class GameFieldController {
    private gameFieldService: GameFieldService;

    constructor() {
        this.gameFieldService = GameFieldService.getInstance();
    }
}