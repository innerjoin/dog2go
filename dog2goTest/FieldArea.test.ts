/// <reference path="jasmine.d.ts"/>
/// <reference path="../dog2go/Game/Logic/file1.ts"/>
/// <reference path="../dog2go/Game/Logic/game.ts"/>

describe("A PlayerFieldArea", () => {
    it("can be tested", () => {
        expect(true).toBe(true);
    });

    it("can be tested", () => {
        var f = new MyTest(5);
        expect(f.a).toBe(5);
    });

    it("was created successfully", () => {
        var area = new PlayerFieldArea(AreaColor.Red);
        //area.gameFields
        expect(area.gameFields[0]).toBe(area.gameFields[1].previous);
    });
});


