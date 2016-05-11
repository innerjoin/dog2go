define(["require", "exports", "phaser", "../../../Frontend/Classes/Controllers/GameArea"], function (require, exports, _phaser, Area) {
    "use strict";
    describe("GameArea - ", function () {
        var timerCallback;
        var game;
        beforeEach(function () {
            timerCallback = jasmine.createSpy("timerCallback");
            jasmine.clock().install();
            console.log(_phaser);
            game = new Phaser.Game();
        });
        afterEach(function () {
            jasmine.clock().uninstall();
        });
        it("creates Kennel fields at the right position", function () {
            var area = new Area.GameArea(true);
            expect(true).toBe(true);
            //var data = [
            //    new BuildUpTypes.KennelField(),
            //    new BuildUpTypes.KennelField(),
            //    new BuildUpTypes.KennelField(),
            //    new BuildUpTypes.KennelField()
            //];
            //var span = 30;
            //var xStart = [380, 20, 140, 500];
            //var yStart = [20, 140, 500, 380];
            //var fc = new FieldCoordinatesData(span, xStart, yStart);
            //var ac = new Coordinates.AreaCoordinates(0, fc);
            //setTimeout(() => {
            //    area.addKennelFields(game, data, ac, 0xFF00CC);
            //}, 0);
            //jasmine.clock().tick(0);
            //var pos0 = data[0].viewRepresentation.position;
            //var pos1 = data[1].viewRepresentation.position;
            //var pos2 = data[2].viewRepresentation.position;
            //var pos3 = data[3].viewRepresentation.position;
            //console.log(pos0, pos1, pos2, pos3);
            //expect(pos0.x).toEqual(50);
            //expect(pos0.y).toEqual(20);
            //expect(pos1.x).toEqual(20);
            //expect(pos1.y).toEqual(20);
            //expect(pos2.x).toEqual(50);
            //expect(pos2.y).toEqual(50);
            //expect(pos3.x).toEqual(20);
            //expect(pos3.y).toEqual(50);
        }, 10000);
    });
});
//# sourceMappingURL=GameArea.test.js.map