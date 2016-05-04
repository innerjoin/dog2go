
export class RoundService {
    private static instance: RoundService = null;
    public assignHandCardsCB: (cards: ICard[]) => any;
    
    constructor() {
        if (RoundService.instance) {
            // ReSharper disable once TsNotResolved
            throw new Error("Error: GameFieldService instantiation failed. Singleton module! Use .getInstance() instead of new.");
        }
        var gameHub = $.connection.gameHub;

        gameHub.client.assignHandCards = (cards: ICard[]) => {
            this.assignHandCardsCB(cards);
        }
        RoundService.instance = this;
    }

    public static getInstance() {
        // Create new instance if callback is given
        if (RoundService.instance === null) {
            RoundService.instance = new RoundService();
        }
        return RoundService.instance;
    }
}

