///<reference path="../../Library/JQuery/jqueryui.d.ts"/>
import BuildUpTypes = require("buildUpTypes");
import PlayerFieldArea = BuildUpTypes.PlayerFieldArea;

export class GameFieldService {
    private static instance: GameFieldService = null;
    public assignHandCardsCB: (cards: ICard[]) => any;
    public createGameTableCB: (ev: IGameTable) => any;

    constructor(callback: (ev: IGameTable) => any) {
        if (GameFieldService.instance) {
            // ReSharper disable once TsNotResolved
            throw new Error("Error: GameFieldService instantiation failed. Singleton module! Use .getInstance() instead of new.");
        }
        var gameHub = $.connection.gameHub;
        gameHub.client.createGameTable = (areas) => {
            callback(areas);
        }
        gameHub.client.assignHandCards = (cards: ICard[]) => {
            this.assignHandCardsCB(cards);
        }
        GameFieldService.instance = this;
    }

    public static getInstance(callback: (ev: IGameTable) => any) {
        // Create new instance if callback is given
        if (GameFieldService.instance === null && callback !== null) {
            GameFieldService.instance = new GameFieldService(callback);
        } else if (GameFieldService.instance === null){
            // ReSharper disable once TsNotResolved
            throw new Error("Error: First call needs a callback!");
        }
        return GameFieldService.instance;
    }

    public getGameFieldData():void {
        var gameHub = $.connection.gameHub;
        $.connection.hub.start().done(() => {
            gameHub.server.connectToTable();
        });
    }
}

