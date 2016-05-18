
import $ = require("jquery");
import ts = require("../../../Frontend/Classes/Services/TurnService");
import TurnService = ts.TurnService;

describe("TurnService - ", () => {
    var callbacks, callbackDone;
    var tableId: number;
    beforeAll(() => {
        callbackDone = {
            done: (callback: any) => {
                callback();
            }
        };

        callbacks = {
            notifyActualPlayer: (possibleCards: IHandCard[], meepleColor: number) => { },
            notifyActualPlayerCards: (possibleCards: IHandCard[], meepleColor: number) => { },
            dropCards: (ev: IGameTable) => { },
            sendMeeplePositions: (meeples: IMeeple[]) => { }
        };
        spyOn(callbacks, "notifyActualPlayer");
        spyOn(callbacks, "notifyActualPlayerCards");
        spyOn(callbacks, "dropCards");
        spyOn(callbacks, "sendMeeplePositions");

        spyOn($.connection.hub, "start").and.callFake(() => {
            return callbackDone;
        });
        $.connection["gameHub"] = <any>{
            server: <any>{
                validateMove: (meepleMove: IMeepleMove, cardMove: ICardMove) => { }
            }, client: <any>{}
        };
        spyOn($.connection.gameHub.server, "validateMove");
    });

    it("get Instance", () => {
        var turnService = TurnService.getInstance(tableId);
        expect(turnService).toBe(TurnService.getInstance(tableId));
    });

    it("Server: validateMove", () => {
        var turnService = TurnService.getInstance(tableId);
        var meepleMove: IMeepleMove = <any>{ testData: 83737, Meeple: { CurrentPosition: { Identifier: 83837 } }, MoveDestination: { Identiier: 88387} };
        var cardMove: ICardMove= <any>{ testData: 43211 };
        // no Callbacks yet awaiten from validate
        turnService.validateMove(meepleMove, cardMove, tableId);

        // uppon calling server Methods: Allways check if Hub has been started correctly
        expect($.connection.hub.start).toHaveBeenCalled();

        meepleMove.Meeple.CurrentFieldId = meepleMove.Meeple.CurrentPosition.Identifier;
        meepleMove.DestinationFieldId = meepleMove.MoveDestination.Identifier;
        delete meepleMove.Meeple.CurrentPosition;
        delete meepleMove.MoveDestination;

        expect($.connection.gameHub.server.validateMove).toHaveBeenCalled();
        expect($.connection.gameHub.server.validateMove).toHaveBeenCalledWith(meepleMove, cardMove);
    });

    it("Client: CallbackMethod: NotifayActualPlayer", () => {
        var turnService = TurnService.getInstance(tableId);
        turnService.notifyActualPlayerCb = callbacks.notifyActualPlayer;
        turnService.notifyActualPlayerCardsCb = callbacks.notifyActualPlayerCards;

        var cards: IHandCard[] = [<any>{ testData: 23883 }];
        var color: number = 844747;

        callbacks.notifyActualPlayer.calls.reset();
        callbacks.notifyActualPlayerCards.calls.reset();

        $.connection.gameHub.client.notifyActualPlayer(cards, color, tableId);

        expect(callbacks.notifyActualPlayer).toHaveBeenCalled();
        expect(callbacks.notifyActualPlayer).toHaveBeenCalledWith(cards, color);

        expect(callbacks.notifyActualPlayerCards).toHaveBeenCalled();
        expect(callbacks.notifyActualPlayerCards).toHaveBeenCalledWith(cards, color);
    });

    it("Client: CallbackMethod: DropCards", () => {
        var turnService = TurnService.getInstance(tableId);

        turnService.dropCardsCb = callbacks.dropCards;
        
        callbacks.dropCards.calls.reset();

        $.connection.gameHub.client.dropCards(tableId);

        expect(callbacks.dropCards).toHaveBeenCalledWith();
        //expect(callbacks.notifyActualPlayer).toHaveBeenCalledWith();
    });

    it("Client: CallbackMethod: NotifayActualPlayer", () => {
        var turnService = TurnService.getInstance(tableId);
        turnService.sendMeeplePositionsCb = callbacks.sendMeeplePositions;

        var meeples: IMeeple[] = [<any>{ testData: 23883 }];

        callbacks.sendMeeplePositions.calls.reset();

        $.connection.gameHub.client.sendMeeplePositions(meeples, tableId);

        expect(callbacks.sendMeeplePositions).toHaveBeenCalled();
        expect(callbacks.sendMeeplePositions).toHaveBeenCalledWith(meeples);
    });
});
