
export class TurnService {
    private static instance: TurnService = null;
    public notifyActualPlayerCB: (possibleCards: ICard[], meepleColor: number) => any;
    public dropCardsCB: () => any;
    public sendMeeplePositionsCB: (meeples: IMeeple[]) => any;
    public returnMoveCB: () => any;

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
        gameHub.client.returnMove = () => {
            this.returnMoveCB();
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
        mMoveReady.Meeple.CurrentFieldId = meepleMove.Meeple.CurrentPosition.Identifier;
        delete mMoveReady.Meeple.spriteRepresentation;
        delete mMoveReady.Meeple.CurrentPosition;

        mMoveReady.DestinationFieldId = meepleMove.MoveDestination.Identifier;
        delete mMoveReady.MoveDestination;
        
        console.log("Validating, Going to Send out: ", mMoveReady, cardMove);
        var gameHub = $.connection.gameHub;
        $.connection.hub.start().done(() => {
            gameHub.server.validateMove(mMoveReady, cardMove);
        });
    }
}

