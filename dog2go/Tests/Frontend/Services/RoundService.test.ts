
import $ = require("jquery");
import rs = require("../../../Frontend/Implementation/Services/RoundService");
import RoundService = rs.RoundService;

describe("RoundService - ", () => {
    var tableId = 0;
    var callbacks, callbackDone;
    beforeAll(() => {
        
        callbackDone = {
            done: (callback: any) => {
                callback();
            }
        };
        //spyOn(callbackDone, "done"); // Does not work, because of direct call

        callbacks = {
            assignHandCards: (cards: ICard[]) => { }
        };
        spyOn(callbacks, "assignHandCards");

        
        spyOn($.connection.hub, "start").and.callFake(() => {
            return callbackDone;
        });
        $.connection["gameHub"] = <any>{
            server: <any>{},
            client: <any>{}
        };
    });

    it("get Instance", () => {
        var roundService = RoundService.getInstance(tableId);

        expect(roundService).toBe(RoundService.getInstance(tableId));
    });

    it("Client: assignHandCards", () => {
        var roundService = RoundService.getInstance(tableId);
        roundService.assignHandCardsCb = callbacks.assignHandCards;

        callbacks.assignHandCards.calls.reset();

        var cards: ICard[] = [<any>{ testData: 48476 }];
        
        $.connection.gameHub.client.assignHandCards(cards, tableId);

        expect(callbacks.assignHandCards).toHaveBeenCalled();
        expect(callbacks.assignHandCards).toHaveBeenCalledWith(cards);
    });
});
