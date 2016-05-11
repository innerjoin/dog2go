define(["require", "exports", "jquery", "../../../Frontend/Classes/Services/RoundService"], function (require, exports, $, rs) {
    "use strict";
    var RoundService = rs.RoundService;
    describe("RoundService - ", function () {
        var callbacks, callbackDone;
        beforeAll(function () {
            callbackDone = {
                done: function (callback) {
                    callback();
                }
            };
            //spyOn(callbackDone, "done"); // Does not work, because of direct call
            callbacks = {
                assignHandCards: function (cards) { }
            };
            spyOn(callbacks, "assignHandCards");
            spyOn($.connection.hub, "start").and.callFake(function () {
                return callbackDone;
            });
            $.connection["gameHub"] = {
                server: {},
                client: {}
            };
        });
        it("get Instance", function () {
            var roundService = RoundService.getInstance();
            expect(roundService).toBe(RoundService.getInstance());
        });
        it("Client: assignHandCards", function () {
            var roundService = RoundService.getInstance();
            roundService.assignHandCardsCB = callbacks.assignHandCards;
            callbacks.assignHandCards.calls.reset();
            var cards = [{ testData: 48476 }];
            $.connection.gameHub.client.assignHandCards(cards);
            expect(callbacks.assignHandCards).toHaveBeenCalled();
            expect(callbacks.assignHandCards).toHaveBeenCalledWith(cards);
        });
    });
});
//# sourceMappingURL=RoundService.test.js.map