
import BuildUpTypes = require("buildUpTypes");
import PlayerFieldArea = BuildUpTypes.PlayerFieldArea;

export class GameFieldService {
    private static instance: GameFieldService = null;
    constructor(callback: (ev: IGameTable) => any) {
        if (GameFieldService.instance) {
            throw new Error("Error: GameFieldService instantiation failed. Singleton module! Use .getInstance() instead of new.");
        }
        var gameHub = $.connection.gameHub;
        gameHub.client.createGameTable = (areas) => {
            console.log("GameFieldService: GotBack createGameTable!", areas);
            callback(areas);
        }
        gameHub.client.assignHandCards = (cards) => {
            console.log("cards: ", cards);
            var container = $("#cardContainer");
            console.log("container: ", container);
            for (var i = 0; i < cards.length; i++) {
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
            throw new Error("Error: First call needs a callback!");
        }
        return GameFieldService.instance;
    }

    public assignHandCards(cards: any) {
        alert("hi there");
        console.log("cards:", cards);
    }

    public getGameFieldData():void {
        var gameHub = $.connection.gameHub;
        $.connection.hub.start().done(() => {
            console.log("GameFieldService: Connection etablished");
            //gameHub.server.sendGameTable();
            //gameHub.server.createGame();
            gameHub.server.connectToTable();
            //console.log("createGameTable...");
            //gameHub.server.createGameTable().then(id => {
            //    console.log("createGameTable executed, id is: ", id);
            //    console.log("ConnectToTable...");
            //    return gameHub.server.connectToTable(id);
            //}).then(table => {
            //        console.log("ConnectToTable executed, result is: ", table);                
            //  //  });
            //    return table;
            //});

        });
    }
}
//GameFieldService.setup();

