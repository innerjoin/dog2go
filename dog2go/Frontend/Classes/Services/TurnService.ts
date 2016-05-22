
export class TurnService {
    private static instance: TurnService = null;
    public notifyActualPlayerCb: (possibleCards: IHandCard[], meepleColor: number) => any;
    public notifyActualPlayerCardsCb: (possibleCards: IHandCard[], meepleColor: number) => any;
    public dropCardsCb: () => any;
    public sendMeeplePositionsCb: (meeples: IMeeple[]) => any;
    public returnMoveCb: () => any;
    private tableId; 

    constructor(gameTableId: number) {
        this.tableId = gameTableId;
        if (TurnService.instance) {
            // ReSharper disable once TsNotResolved
            throw new Error("Error: GameFieldService instantiation failed. Singleton module! Use .getInstance(_tableId) instead of new.");
        }
        const gameHub = $.connection.gameHub;

        gameHub.client.notifyActualPlayer = (possibleCards, meepleColor, tableId) => {
            // will autoconvert string to int
            // ReSharper disable once CoercedEqualsUsing
            if (gameTableId == tableId) {
                this.notifyActualPlayerCb(possibleCards, meepleColor);
                this.notifyActualPlayerCardsCb(possibleCards, meepleColor);
            }
        }
        gameHub.client.sendMeeplePositions = (meeples, tableId) => {
            // will autoconvert string to int
            // ReSharper disable once CoercedEqualsUsing
            if (gameTableId == tableId) {
                this.sendMeeplePositionsCb(meeples);
            }
        }
        gameHub.client.dropCards = (tableId) => {
            // will autoconvert string to int
            // ReSharper disable once CoercedEqualsUsing
            if (gameTableId == tableId) {
                this.dropCardsCb();
            }
        }
        gameHub.client.returnMove = (tableId) => {
            // will autoconvert string to int
            // ReSharper disable once CoercedEqualsUsing
            if (gameTableId == tableId) {
                this.returnMoveCb();
            }
        }
        
        TurnService.instance = this;
    }

    public static getInstance(tableId: number) {
        // Create new instance if callback is given
        if (TurnService.instance === null || tableId !== TurnService.instance.tableId) {
            TurnService.instance = new TurnService(tableId);
        }
        return TurnService.instance;
    }

    public validateMove(meepleMove: IMeepleMove, cardMove: ICardMove, tableId: number) {
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
            gameHub.server.validateMove(mMoveReady, cardMove, tableId);
        });
    }
}

