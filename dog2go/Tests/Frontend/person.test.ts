import BuildUpTypes = require("../../Frontend/Classes/Services/buildUpTypes");
import Persontest = BuildUpTypes.Persontest;
import AreaColor = BuildUpTypes.AreaColor;
import PlayerFieldArea = BuildUpTypes.PlayerFieldArea;

describe("THE TEST", () => {
    
    var person: BuildUpTypes.Persontest;

    beforeEach(() => {
        person = new Persontest();
        person.setFirstName("Joe");
        person.setLastName("Smith");
    });

    it("should concatenate first and last names", () => {
        expect(person.getFullName()).toBe("Joe, Smith");
    });

    it("coloring works", () => {
        var col = AreaColor.Red;
        var area = new PlayerFieldArea(col);
        expect(area.color).toBe(AreaColor.Red);
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
