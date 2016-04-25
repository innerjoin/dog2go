import BuildUpTypes = require("buildUpTypes");
import PlayerFieldArea = BuildUpTypes.PlayerFieldArea;

export class GameFieldService {
    private static instance: GameFieldService = null;
    constructor(callback: (ev: IGameTable) => any) {
        if (GameFieldService.instance) {
            // ReSharper disable once TsNotResolved
            throw new Error("Error: GameFieldService instantiation failed. Singleton module! Use .getInstance() instead of new.");
        }
        var gameHub = $.connection.gameHub;
        gameHub.client.createGameTable = (areas) => {
            callback(areas);
        }
        gameHub.client.assignHandCards = (cards: Card[]) => {
            var container = $("#cardContainer");
            for (var i = 0; i < cards.length; i++) { //TODO: make length recognizable
                container.append("<img src=\"/Frontend/Images/cards-min/" + cards[i].ImageIdentifier + "\" ></img>");
            }

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

