define(["require", "exports", "jquery", "../../../Frontend/Classes/Services/GameFieldsService"], function (require, exports, $, gfs) {
    "use strict";
    var GameFieldService = gfs.GameFieldService;
    describe("GameFieldService - ", function () {
        var gameTable;
        var cards;
        //var $ = null;
        var callbackCreate, callbackDone;
        beforeAll(function () {
            gameTable = { testdata: 12345 };
            cards = [{ testdata: 88838 }];
            callbackDone = {
                done: function (callback) {
                    callback();
                }
            };
            callbackCreate = {
                createGametable: function (ev) {
                    gameTable = ev;
                },
                assign: function (cards) {
                }
            };
            spyOn(callbackCreate, "createGametable");
            spyOn(callbackCreate, "assign");
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
        it("getGameFieldData", function () {
            var gameFieldService = GameFieldService.getInstance();
            gameFieldService.createGameTableCB = callbackCreate.createGametable;
            gameFieldService.getGameFieldData();
            // uppon calling server Methods: Allways check if Hub has been started correctly
            expect($.connection.hub.start).toHaveBeenCalled();
            expect(callbackCreate.createGametable).toHaveBeenCalled();
            expect(callbackCreate.createGametable).toHaveBeenCalledWith(gameTable);
        });
        it("Client: backToGame", function () {
            var gameFieldService = GameFieldService.getInstance();
            gameFieldService.createGameTableCB = callbackCreate.createGametable;
            gameFieldService.assignHandCardsCB = callbackCreate.assign;
            callbackCreate.createGametable.calls.reset();
            callbackCreate.assign.calls.reset();
            $.connection.gameHub.client.backToGame(gameTable, cards);
            expect(callbackCreate.createGametable).toHaveBeenCalled();
            expect(callbackCreate.createGametable).toHaveBeenCalledWith(gameTable);
            expect(callbackCreate.assign).toHaveBeenCalled();
            expect(callbackCreate.assign).toHaveBeenCalledWith(cards);
        });
    });
});
//# sourceMappingURL=GameFieldService.test.js.map