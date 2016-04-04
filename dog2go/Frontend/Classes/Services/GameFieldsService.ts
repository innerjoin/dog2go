
class GameFieldService {
    private static instance: GameFieldService = null;
    constructor(callback: (ev: PlayerFieldArea[]) => any) {
        if (GameFieldService.instance) {
            throw new Error("Error: GameFieldService instantiation failed. Singleton module! Use .getInstance() instead of new.");
        }
        var gameHub = $.connection.gameHub;
        gameHub.client.createGameTable = (areas) => {
            console.log("GameFieldService: GotBack createGameTable!", areas);
            callback(areas);
        }
        GameFieldService.instance = this;
    }

    public static getInstance(callback: (ev: PlayerFieldArea[]) => any) {
        // Create new instance if callback is given
        if (GameFieldService.instance === null && callback !== null) {
            GameFieldService.instance = new GameFieldService(callback);
        } else if (GameFieldService.instance === null){
            throw new Error("Error: First call needs a callback!");
        }
        return GameFieldService.instance;
    }

    public getGameFieldData():void {
        var gameHub = $.connection.gameHub;
        $.connection.hub.start().done(() => {
            console.log("GameFieldService: Connection etablished");
            gameHub.server.sendGameTable();
        });
    }
}
//GameFieldService.setup();

