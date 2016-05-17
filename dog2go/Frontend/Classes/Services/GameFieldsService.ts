
export class GameFieldService {
    private static instance: GameFieldService = null;
    public assignHandCardsCB: (cards: ICard[]) => any;
    public createGameTableCB: (gameTable: IGameTable) => any;

    constructor() {
        if (GameFieldService.instance) {
            // ReSharper disable once TsNotResolved
            throw new Error("Error: GameFieldService instantiation failed. Singleton module! Use .getInstance() instead of new.");
        }
        var gameHub = $.connection.gameHub;
        gameHub.client.createGameTable = (gameTable) => {
            this.createGameTableCB(gameTable);
        }

        gameHub.client.backToGame = (gameTable, cards) => {
            this.createGameTableCB(gameTable);
            this.assignHandCardsCB(cards);
        }
        GameFieldService.instance = this;
    }

    public static getInstance() {
        // Create new instance if callback is given
        if (GameFieldService.instance === null) {
            var temp = new GameFieldService();
        }
        return GameFieldService.instance;
    }

    public getGameFieldData(tableId: number):void {
        var gameHub = $.connection.gameHub;
        $.connection.hub.start().done(() => {
            console.log("the ID: ", tableId);
            gameHub.server.connectToTable(tableId);
        });
    }
}

