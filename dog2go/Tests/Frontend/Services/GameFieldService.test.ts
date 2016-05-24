
import $ = require("jquery");
import gfs = require("../../../Frontend/Implementation/Services/GameFieldsService");
import GameFieldService = gfs.GameFieldService;

describe("GameFieldService - ", () => {
    var gameTable: IGameTable;
    var cards: ICard[];
    var tableId = 0;
    var callbacks, callbackDone;
    beforeAll(() => {
        gameTable = <any>{ testdata: 12345 };
        cards = [<any>{ testdata: 88838 }];

        callbackDone = {
            done: (callback: any) => {
                callback();
            }
        };

        callbacks = {
            createGametable: (ev: IGameTable) => {
                //gameTable = ev;
            },
            assignHandCards: (cards: ICard[]) => {
                
            }
        };
        spyOn(callbacks, "createGametable");
        spyOn(callbacks, "assignHandCards");
        //spyOn(callbackDone, "done"); // Does not work, because of direct call

        spyOn($.connection.hub, "start").and.callFake(() => {
            return callbackDone;
        });
        $.connection["gameHub"] = <any>{server: <any>{
            connectToTable: () => { $.connection.gameHub.client.createGameTable(gameTable, tableId); }
            }, client: <any>{}
        };
    });
    
    it("get Instance", () => {
        var gameFieldService = GameFieldService.getInstance(tableId);

        expect(gameFieldService).toBe(GameFieldService.getInstance(tableId));
    });
    
    it("Server: getGameFieldData", () => {
        
        var gameFieldService = GameFieldService.getInstance(tableId);
        gameFieldService.createGameTableCb = callbacks.createGametable;

        gameFieldService.getGameFieldData(1);

        // uppon calling server Methods: Allways check if Hub has been started correctly
        expect($.connection.hub.start).toHaveBeenCalled();

        expect(callbacks.createGametable).toHaveBeenCalled();
        expect(callbacks.createGametable).toHaveBeenCalledWith(gameTable);
    });

    it("Client: backToGame", () => {
        var gameFieldService = GameFieldService.getInstance(tableId);
        gameFieldService.createGameTableCb = callbacks.createGametable;
        gameFieldService.assignHandCardsCb = callbacks.assignHandCards;

        callbacks.createGametable.calls.reset();
        callbacks.assignHandCards.calls.reset();

        $.connection.gameHub.client.backToGame(gameTable, cards, tableId);

        expect(callbacks.createGametable).toHaveBeenCalled();
        expect(callbacks.createGametable).toHaveBeenCalledWith(gameTable);

        expect(callbacks.assignHandCards).toHaveBeenCalled();
        expect(callbacks.assignHandCards).toHaveBeenCalledWith(cards);
    });
});
