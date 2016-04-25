
import $ = require("jquery");
import gfs = require("../../../Frontend/Classes/Services/GameFieldsService");
import BuildUpTypes = require("../../../Frontend/Classes/Services/buildUpTypes");
import Coordinates = require("../../../Frontend/Classes/Controllers/FieldCoordinates");
import FieldCoordinatesData = Coordinates.FieldCoordinatesData;
//require("signalr.hubs");

describe("GameFieldService - ", () => {
    var gameTable: IGameTable;
    //var $ = null;
    var callbackCreate, callbackDone;
    beforeEach(() => {
        gameTable = null;
        callbackDone = {
            done: (callback: any) => {
                callback();
            }
        };

        callbackCreate = {
            fn: (ev: IGameTable) => {
                gameTable = ev;
            }
        };
        spyOn(callbackCreate, "fn");
        //spyOn(callbackDone, "done"); // Does not work, because of direct call

        spyOn($.connection.hub, "start").and.callFake(() => {
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
