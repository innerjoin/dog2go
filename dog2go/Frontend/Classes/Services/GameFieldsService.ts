
export class GameFieldService {
    private static instance: GameFieldService = null;
    public assignHandCardsCb: (cards: ICard[]) => any;
    public createGameTableCb: (gameTable: IGameTable) => any;

    constructor() {
        if (GameFieldService.instance) {
            // ReSharper disable once TsNotResolved
            throw new Error("Error: GameFieldService instantiation failed. Singleton module! Use .getInstance() instead of new.");
        }
        const gameHub = $.connection.gameHub;
        gameHub.client.createGameTable = (gameTable) => {
            this.createGameTableCb(gameTable);
        }

        gameHub.client.backToGame = (gameTable, cards) => {
            this.createGameTableCb(gameTable);
            this.assignHandCardsCb(cards);
        }
        GameFieldService.instance = this;
    }

    public static getInstance() {
        // Create new instance if callback is given
        if (GameFieldService.instance === null) {
            new GameFieldService();
        }
        return GameFieldService.instance;
    }

    public getGameFieldData(tableId: number):void {
        var gameHub = $.connection.gameHub;
        $.connection.hub.start().done(() => {
            gameHub.server.connectToTable(tableId);
        });
    }
}

