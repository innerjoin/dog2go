
export class GameFieldService {
    private static instance: GameFieldService = null;
    public assignHandCardsCb: (cards: ICard[]) => any;
    public createGameTableCb: (gameTable: IGameTable) => any;
    public tableId: number;
    constructor(tableId: number) {
        this.tableId = tableId;
        //alert("GameFieldService: " + tableId);
        if (GameFieldService.instance) {
            // ReSharper disable once TsNotResolved
            throw new Error("Error: GameFieldService instantiation failed. Singleton module! Use .getInstance() instead of new.");
        }
        const gameHub = $.connection.gameHub;
        $.connection.hub.qs = `tableId=${tableId}`;
        gameHub.client.createGameTable = (gameTable, _tableId) => {
            console.log("createGameTable: ", _tableId);
            // will autoconvert string to int
            // ReSharper disable once CoercedEqualsUsing
            if (_tableId == tableId) {
                this.createGameTableCb(gameTable);
            }
        }

        gameHub.client.backToGame = (gameTable, cards, _tableId) => {
            console.log("backToGame: ", _tableId);
            // will autoconvert string to int
            // ReSharper disable once CoercedEqualsUsing
            if (_tableId == tableId) {
                console.log("our this context: ", this);
                this.createGameTableCb(gameTable);
                this.assignHandCardsCb(cards);
            }
        }
        GameFieldService.instance = this;
    }

    public static getInstance(tableId: number) {
        // Create new instance if callback is given
        if (GameFieldService.instance === null || GameFieldService.instance.tableId !== tableId) {
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

