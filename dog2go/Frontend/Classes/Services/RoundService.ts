
export class RoundService {
    private static instance: RoundService = null;
    public assignHandCardsCb: (cards: ICard[]) => any;
    
    constructor(gameTableId: number) {
        if (RoundService.instance) {
            // ReSharper disable once TsNotResolved
            throw new Error("Error: GameFieldService instantiation failed. Singleton module! Use .getInstance(_tableId) instead of new.");
        }
        const gameHub = $.connection.gameHub;

        gameHub.client.assignHandCards = (cards: ICard[], tableId: number) => {
            if (gameTableId === tableId) {
                this.assignHandCardsCb(cards);
            }
        }
        RoundService.instance = this;
    }

    public static getInstance(tableId: number) {
        // Create new instance if callback is given
        if (RoundService.instance === null) {
            RoundService.instance = new RoundService(tableId);
        }
        return RoundService.instance;
    }
}

