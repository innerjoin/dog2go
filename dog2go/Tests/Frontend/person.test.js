define(["require", "exports", "../../Frontend/Classes/Services/buildUpTypes"], function (require, exports, BuildUpTypes) {
    var Persontest = BuildUpTypes.Persontest;
    var AreaColor = BuildUpTypes.AreaColor;
    var PlayerFieldArea = BuildUpTypes.PlayerFieldArea;
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
        it("coloring works", function () {
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
});
//# sourceMappingURL=person.test.js.map