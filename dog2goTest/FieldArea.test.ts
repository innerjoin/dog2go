/// <reference path="Scripts/typings/jasmine/jasmine.d.ts"/>
/// <reference path="../dog2go/Frontend/Classes/Services/game.ts" />
/* <reference path="../dog2go/Game/Logic/file1.ts"/>*/

describe("A PlayerFieldArea", () => {
    it("can be tested", () => {
        expect(true).toBe(true);
    });
    /*
    //file not found
    it("can be tested", () => {
        var f = new MyTest(5);
        expect(f.a).toBe(5);
    });
    */

    it("was created successfully", () => {
        var area = new PlayerFieldArea(AreaColor.Red);
        //area.gameFields
        expect(area.gameFields[0]).toBe(area.gameFields[1].previous);
    });
});


