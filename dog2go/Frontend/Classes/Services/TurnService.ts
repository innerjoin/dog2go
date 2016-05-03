
export class TurnService {
    private static instance: TurnService = null;
    public notifyActualPlayerCB: (possibleCards: ICard[], meepleColor: number) => any;
    public dropCardsCB: () => any;
    public sendMeeplePositionsCB: (meeples: IMeeple[]) => any;


    constructor() {
        if (TurnService.instance) {
            // ReSharper disable once TsNotResolved
            throw new Error("Error: GameFieldService instantiation failed. Singleton module! Use .getInstance() instead of new.");
        }
        var gameHub = $.connection.gameHub;

        gameHub.client.notifyActualPlayer = (possibleCards, meepleColor) => {
            this.notifyActualPlayerCB(possibleCards, meepleColor);
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
        var mMoveReady: any = $.extend({}, meepleMove);
        mMoveReady.Meeple = $.extend({}, meepleMove.Meeple);
        mMoveReady.Meeple.CurrentPosition = $.extend({}, meepleMove.Meeple.CurrentPosition);
        mMoveReady.Meeple.spriteRepresentation = null;
        mMoveReady.MoveDestination = $.extend({}, meepleMove.MoveDestination);
        mMoveReady.MoveDestination.viewRepresentation = null;
        if (mMoveReady.MoveDestination.EndFieldEntry) {
            mMoveReady.MoveDestination.EndFieldEntry = null;
        }
        console.log("Going to Send out: ", mMoveReady, cardMove);
        var gameHub = $.connection.gameHub;
        $.connection.hub.start().done(() => {
            var test = gameHub.server.validateMove(mMoveReady, cardMove);
        });
    }
}

