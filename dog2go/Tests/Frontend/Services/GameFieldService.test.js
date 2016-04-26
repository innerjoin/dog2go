define(["require", "exports", "jquery"], function (require, exports, $) {
    //require("signalr.hubs");
    describe("GameFieldService - ", function () {
        var gameTable;
        //var $ = null;
        var callbackCreate, callbackDone;
        beforeEach(function () {
            gameTable = null;
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
            //$.connection["gameHub"] = {server: {
            //    sendGameTable: () => { $.connection.gameHub.client.createGameTable(gameTable); }
            //}, client: {}
            //};
        });
        //it("get Instance", () => {
        //    gfs.GameFieldService.bind($);
        //    gfs.GameFieldService.getInstance.bind($);
        //    var gameFieldService = gfs.GameFieldService.getInstance(callbackCreate.fn);
        //    gameFieldService.getGameFieldData();
        //    // Allways if Hub has been started correctly
        //    expect($.connection.hub.start).toHaveBeenCalled();
        //    expect(callbackCreate.fn).toHaveBeenCalled();
        //    expect(callbackCreate.fn).toHaveBeenCalledWith(gameTable);
        //});
    });
});
//# sourceMappingURL=GameFieldService.test.js.map