
import $ = require("jquery");
import gfs = require("../../../Frontend/Classes/Services/GameFieldsService");
import Coordinates = require("../../../Frontend/Classes/Controllers/FieldCoordinates");

describe("GameFieldService - ", () => {
    var gameTable: IGameTable;
    //var $ = null;
    var callbackCreate, callbackDone;
    beforeEach(() => {
        gameTable = <any>{testdata: 12345};
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
        $.connection["gameHub"] = <any>{server: <any>{
            connectToTable: () => { $.connection.gameHub.client.createGameTable(gameTable); }
        }, client: <any>{}
        };
    });
    
    it("get Instance", () => {
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
