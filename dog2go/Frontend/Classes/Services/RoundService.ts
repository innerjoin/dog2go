
export class RoundService {
    private static instance: RoundService = null;
    public assignHandCardsCb: (cards: ICard[]) => any;
    private tableId;

    constructor(gameTableId: number) {
        this.tableId = gameTableId;
        if (RoundService.instance) {
            // ReSharper disable once TsNotResolved
            throw new Error("Error: GameFieldService instantiation failed. Singleton module! Use .getInstance(_tableId) instead of new.");
        }
        const gameHub = $.connection.gameHub;

        gameHub.client.assignHandCards = (cards: ICard[], tableId: number) => {
            // will autoconvert string to int
            // ReSharper disable once CoercedEqualsUsing
            if (gameTableId == tableId) {
                this.assignHandCardsCb(cards);
            }
        }
        RoundService.instance = this;
    }

    public static getInstance(tableId: number) {
        // Create new instance if callback is given
        if (RoundService.instance === null || tableId !== RoundService.instance.tableId) {
            RoundService.instance = new RoundService(tableId);
        }
        return RoundService.instance;
    }
}

