define(["require", "exports", "jquery", "../../../Frontend/Classes/Services/GameFieldsService"], function (require, exports, $, gfs) {
    var GameFieldService = gfs.GameFieldService;
    describe("GameFieldService - ", function () {
        var gameTable;
        var cards;
        //var $ = null;
        var callbacks, callbackDone;
        beforeAll(function () {
            gameTable = { testdata: 12345 };
            cards = [{ testdata: 88838 }];
            callbackDone = {
                done: function (callback) {
                    callback();
                }
            };
            callbacks = {
                createGametable: function (ev) {
                    //gameTable = ev;
                },
                assignHandCards: function (cards) {
                }
            };
            spyOn(callbacks, "createGametable");
            spyOn(callbacks, "assignHandCards");
            //spyOn(callbackDone, "done"); // Does not work, because of direct call
            spyOn($.connection.hub, "start").and.callFake(function () {
                return callbackDone;
            });
            $.connection["gameHub"] = { server: {
                    connectToTable: function () { $.connection.gameHub.client.createGameTable(gameTable); }
                }, client: {}
            };
        });
        it("get Instance", function () {
            var gameFieldService = GameFieldService.getInstance();
            expect(gameFieldService).toBe(GameFieldService.getInstance());
        });
        it("Server: getGameFieldData", function () {
            var gameFieldService = GameFieldService.getInstance();
            gameFieldService.createGameTableCB = callbacks.createGametable;
            gameFieldService.getGameFieldData();
            // uppon calling server Methods: Allways check if Hub has been started correctly
            expect($.connection.hub.start).toHaveBeenCalled();
            expect(callbacks.createGametable).toHaveBeenCalled();
            expect(callbacks.createGametable).toHaveBeenCalledWith(gameTable);
        });
        it("Client: backToGame", function () {
            var gameFieldService = GameFieldService.getInstance();
            gameFieldService.createGameTableCB = callbacks.createGametable;
            gameFieldService.assignHandCardsCB = callbacks.assignHandCards;
            callbacks.createGametable.calls.reset();
            callbacks.assignHandCards.calls.reset();
            $.connection.gameHub.client.backToGame(gameTable, cards);
            expect(callbacks.createGametable).toHaveBeenCalled();
            expect(callbacks.createGametable).toHaveBeenCalledWith(gameTable);
            expect(callbacks.assignHandCards).toHaveBeenCalled();
            expect(callbacks.assignHandCards).toHaveBeenCalledWith(cards);
        });
    });
});
//# sourceMappingURL=GameFieldService.test.js.map