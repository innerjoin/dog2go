import _ = require("phaser");
import Area = require("../../Frontend/Classes/Controllers/GameArea");
import BuildUpTypes = require("../../Frontend/Classes/Services/buildUpTypes");
import Coordinates = require("../../Frontend/Classes/Controllers/FieldCoordinates");
import FieldCoordinatesData = Coordinates.FieldCoordinatesData;

describe("GameArea", () => {
    var game: Phaser.Game;
    var el = new HTMLElement();
    beforeEach(() => {
        game = new Phaser.Game(200, 200, Phaser.AUTO, el);
        setTimeout(() => {
            expect(game.width).toBe(800);
            expect(game.height).toBe(600);
            expect(game).not.toBe(null);
            expect(game.add).not.toBe(null);
            expect(game.add.graphics).not.toBe(null);
        }, 0);
    });

    it("creates Kennel fields at the right position", () => {
        jasmine.clock().install();
        var area = new Area.GameArea();
        var data = [
            new BuildUpTypes.KennelField(),
            new BuildUpTypes.KennelField(),
            new BuildUpTypes.KennelField(),
            new BuildUpTypes.KennelField()
        ];
        var span = 30;
        var xStart = [380, 20, 150, 510];
        var yStart = [20, 150, 510, 380];
        var fc = new FieldCoordinatesData(span, xStart, yStart);
        var ac = new Coordinates.AreaCoordinates(1, fc);
        setTimeout(() => {
            area.addKennelFields(game, data, ac, 0xFF00CC);
        }, 100);
        jasmine.clock().tick(150);
        var pos = data[0].viewRepresentation.position;
        console.log("poooooos, ", pos);
        expect(pos.x).toEqual(220);
        expect(pos.y).toEqual(2150);
        jasmine.clock().uninstall();
    }, 10000);
});
