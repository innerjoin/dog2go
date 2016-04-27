
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

        // TODO Will be outsourced to RoundService
        gameHub.client.assignHandCards = (cards: ICard[]) => {
            this.assignHandCardsCB(cards);
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
            GameFieldService.instance = new GameFieldService();
        }
        return GameFieldService.instance;
    }

    public getGameFieldData():void {
        var gameHub = $.connection.gameHub;
        $.connection.hub.start().done(() => {
            var test = gameHub.server.connectToTable();
            console.log("Got Table back: ", test);
        });
    }
}

