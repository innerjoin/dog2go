define(["require", "exports", "jquery", "../../../Frontend/Classes/Services/TurnService"], function (require, exports, $, ts) {
    "use strict";
    var TurnService = ts.TurnService;
    describe("TurnService - ", function () {
        var callbacks, callbackDone;
        beforeAll(function () {
            callbackDone = {
                done: function (callback) {
                    callback();
                }
            };
            callbacks = {
                notifyActualPlayer: function (possibleCards, meepleColor) { },
                dropCards: function (ev) { },
                sendMeeplePositions: function (meeples) { }
            };
            spyOn(callbacks, "notifyActualPlayer");
            spyOn(callbacks, "dropCards");
            spyOn(callbacks, "sendMeeplePositions");
            spyOn($.connection.hub, "start").and.callFake(function () {
                return callbackDone;
            });
            $.connection["gameHub"] = {
                server: {
                    validateMove: function (meepleMove, cardMove) { }
                }, client: {}
            };
            spyOn($.connection.gameHub.server, "validateMove");
        });
        it("get Instance", function () {
            var turnService = TurnService.getInstance();
            expect(turnService).toBe(TurnService.getInstance());
        });
        it("Server: validateMove", function () {
            var turnService = TurnService.getInstance();
            var meepleMove = { testData: 83737, Meeple: { CurrentPosition: { Identifier: 83837 } }, MoveDestination: { Identiier: 88387 } };
            var cardMove = { testData: 43211 };
            // no Callbacks yet awaiten from validate
            turnService.validateMove(meepleMove, cardMove);
            // uppon calling server Methods: Allways check if Hub has been started correctly
            expect($.connection.hub.start).toHaveBeenCalled();
            meepleMove.Meeple.CurrentFieldId = meepleMove.Meeple.CurrentPosition.Identifier;
            meepleMove.DestinationFieldId = meepleMove.MoveDestination.Identifier;
            delete meepleMove.Meeple.CurrentPosition;
            delete meepleMove.MoveDestination;
            expect($.connection.gameHub.server.validateMove).toHaveBeenCalled();
            expect($.connection.gameHub.server.validateMove).toHaveBeenCalledWith(meepleMove, cardMove);
        });
        it("Client: CallbackMethod: NotifayActualPlayer", function () {
            var turnService = TurnService.getInstance();
            turnService.notifyActualPlayerCB = callbacks.notifyActualPlayer;
            var cards = [{ testData: 23883 }];
            var color = 844747;
            callbacks.notifyActualPlayer.calls.reset();
            $.connection.gameHub.client.notifyActualPlayer(cards, color);
            expect(callbacks.notifyActualPlayer).toHaveBeenCalled();
            expect(callbacks.notifyActualPlayer).toHaveBeenCalledWith(cards, color);
        });
        it("Client: CallbackMethod: DropCards", function () {
            var turnService = TurnService.getInstance();
            turnService.dropCardsCB = callbacks.dropCards;
            callbacks.dropCards.calls.reset();
            $.connection.gameHub.client.dropCards();
            expect(callbacks.dropCards).toHaveBeenCalledWith();
            //expect(callbacks.notifyActualPlayer).toHaveBeenCalledWith();
        });
        it("Client: CallbackMethod: NotifayActualPlayer", function () {
            var turnService = TurnService.getInstance();
            turnService.sendMeeplePositionsCB = callbacks.sendMeeplePositions;
            var meeples = [{ testData: 23883 }];
            callbacks.sendMeeplePositions.calls.reset();
            $.connection.gameHub.client.sendMeeplePositions(meeples);
            expect(callbacks.sendMeeplePositions).toHaveBeenCalled();
            expect(callbacks.sendMeeplePositions).toHaveBeenCalledWith(meeples);
        });
    });
});
//# sourceMappingURL=TurnService.test.js.map