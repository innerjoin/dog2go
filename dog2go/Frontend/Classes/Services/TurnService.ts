
export class TurnService {
    private static instance: TurnService = null;
    public notifyActualPlayerCB: (possibleCards: ICard[]) => any;
    public dropCardsCB: () => any;
    public sendMeeplePositionsCB: (meeples: IMeeple[]) => any;


    constructor() {
        if (TurnService.instance) {
            // ReSharper disable once TsNotResolved
            throw new Error("Error: GameFieldService instantiation failed. Singleton module! Use .getInstance() instead of new.");
        }
        var gameHub = $.connection.gameHub;

        gameHub.client.notifyActualPlayer = (possibleCards) => {
            this.notifyActualPlayerCB(possibleCards);
        }
        gameHub.client.sendMeeplePositions = (meeples) => {
            this.sendMeeplePositionsCB(meeples);
        }
        gameHub.client.dropCards = () => {
            this.dropCardsCB();
        }
        
        TurnService.instance = this;
    }

    public static getInstance() {
        // Create new instance if callback is given
        if (TurnService.instance === null) {
            TurnService.instance = new TurnService();
        }
        return TurnService.instance;
    }

    public validateMove(meepleMove: IMeepleMove, cardMove: ICardMove) {
        var gameHub = $.connection.gameHub;
        $.connection.hub.start().done(() => {
            var test = gameHub.server.ValidateMove(meepleMove, cardMove);
        });
    }
}

