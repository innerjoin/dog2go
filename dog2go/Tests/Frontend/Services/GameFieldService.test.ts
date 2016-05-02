
import $ = require("jquery");
import gfs = require("../../../Frontend/Classes/Services/GameFieldsService");
import GameFieldService = gfs.GameFieldService;

describe("GameFieldService - ", () => {
    var gameTable: IGameTable;
    var cards: ICard[];
    //var $ = null;
    var callbackCreate, callbackDone;
    beforeAll(() => {
        gameTable = <any>{ testdata: 12345 };
        cards = [<any>{ testdata: 88838 }];

        callbackDone = {
            done: (callback: any) => {
                callback();
            }
        };

        callbackCreate = {
            createGametable: (ev: IGameTable) => {
                gameTable = ev;
            },
            assign: (cards: ICard[]) => {
                
            }
        };
        spyOn(callbackCreate, "createGametable");
        spyOn(callbackCreate, "assign");
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
        var gameFieldService = GameFieldService.getInstance();

        expect(gameFieldService).toBe(GameFieldService.getInstance());
    });
    
    it("getGameFieldData", () => {
        
        var gameFieldService = GameFieldService.getInstance();
        gameFieldService.createGameTableCB = callbackCreate.createGametable;

        gameFieldService.getGameFieldData();

        // uppon calling server Methods: Allways check if Hub has been started correctly
        expect($.connection.hub.start).toHaveBeenCalled();

        expect(callbackCreate.createGametable).toHaveBeenCalled();
        expect(callbackCreate.createGametable).toHaveBeenCalledWith(gameTable);
    });

    it("Client: backToGame", () => {
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
