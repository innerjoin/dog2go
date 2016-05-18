
export class GameFieldService {
    private static instance: GameFieldService = null;
    public assignHandCardsCb: (cards: ICard[]) => any;
    public createGameTableCb: (gameTable: IGameTable) => any;

    constructor(tableId: number) {
        if (GameFieldService.instance) {
            // ReSharper disable once TsNotResolved
            throw new Error("Error: GameFieldService instantiation failed. Singleton module! Use .getInstance() instead of new.");
        }
        const gameHub = $.connection.gameHub;
        gameHub.qs = `tableId=${tableId}`;
        gameHub.client.createGameTable = (gameTable) => {
            this.createGameTableCb(gameTable);
        }

        gameHub.client.backToGame = (gameTable, cards) => {
            this.createGameTableCb(gameTable);
            this.assignHandCardsCb(cards);
        }
        GameFieldService.instance = this;
    }

    public static getInstance(tableId: number) {
        // Create new instance if callback is given
        if (GameFieldService.instance === null) {
            new GameFieldService(tableId);
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

