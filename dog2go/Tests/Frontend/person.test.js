/// <reference path="../../Scripts/typings/jasmine/jasmine.d.ts"/>
/// <reference path="../../Frontend/Classes/Services/game.ts"/>
describe("THE TEST", function () {
    var person;
    beforeEach(function () {
        person = new Persontest();
        person.setFirstName("Joe");
        person.setLastName("Smith");
    });
    it("should concatenate first and last names", function () {
        expect(person.getFullName()).toBe("Joe, Smith");
    });
    it("loads game", function () {
        //var p = new Persontest();
        //p.setFirstName("Lukas");
        //p.setLastName("Steiger");
        //expect(p.getFullName()).toBe("Lukas, Steiger");
        var col = AreaColor.Red;
        var area = new PlayerFieldArea(col);
        expect(area.color).toBe(col);
    });
    it("loads area color", function () {
        expect(AreaColor.Red).toBe(0xff0000);
    });
    it("should concatenate first and last names - incorrect", function () {
        expect(person.getFullName()).not.toBe("Joe, Doe");
    });
    it("should concatenate lastname first", function () {
        expect(person.getFullName(true)).toBe("Smith, Joe");
    });
    it("should not concatinate firstname first", function () {
        expect(person.getFullName(true)).not.toBe("Joe, Smith");
    });
});
//# sourceMappingURL=person.test.js.map