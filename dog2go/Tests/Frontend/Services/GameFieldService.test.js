define(["require", "exports", "jquery", "../../../Frontend/Classes/Services/GameFieldsService"], function (require, exports, $, gfs) {
    "use strict";
    describe("GameFieldService - ", function () {
        var gameTable;
        //var $ = null;
        var callbackCreate, callbackDone;
        beforeEach(function () {
            gameTable = { testdata: 12345 };
            callbackDone = {
                done: function (callback) {
                    callback();
                }
            };
            callbackCreate = {
                fn: function (ev) {
                    gameTable = ev;
                }
            };
            spyOn(callbackCreate, "fn");
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
            gfs.GameFieldService.bind($);
            gfs.GameFieldService.getInstance.bind($);
            var gameFieldService = gfs.GameFieldService.getInstance();
            gameFieldService.createGameTableCB = callbackCreate.fn;
            gameFieldService.getGameFieldData();
            // Allways check if Hub has been started correctly
            expect($.connection.hub.start).toHaveBeenCalled();
            expect(callbackCreate.fn).toHaveBeenCalled();
            expect(callbackCreate.fn).toHaveBeenCalledWith(gameTable);
        });
    });
});
//# sourceMappingURL=GameFieldService.test.js.map