/// <reference path="../../Frontend/Library/jasmine/jasmine.d.ts"/>
/// <reference path="../../Frontend/Classes/Controllers/GameArea.ts"/>

describe("THE TEST", () => {

    var person: Persontest;

    beforeEach(() => {
        person = new Persontest();
        person.setFirstName("Joe");
        person.setLastName("Smith");
    });

    it("should concatenate first and last names", () => {
        expect(person.getFullName()).toBe("Joe, Smith");
    });

    it("loads game", () => {
        //var p = new Persontest();
        //p.setFirstName("Lukas");
        //p.setLastName("Steiger");
        //expect(p.getFullName()).toBe("Lukas, Steiger");
        var col = AreaColor.Red;
        var area = new PlayerFieldArea(col);
        expect(area.color).toBe(col);
    });

    it("loads area color", () => {
        expect(AreaColor.Red).toBe(0xff0000);
    });

    it("should concatenate first and last names - incorrect", () => {
        expect(person.getFullName()).not.toBe("Joe, Doe");
    });

    it("should concatenate lastname first", () => {
        expect(person.getFullName(true)).toBe("Smith, Joe");
    });

    it("should not concatinate firstname first", () => {
        expect(person.getFullName(true)).not.toBe("Joe, Smith");
    });
});
