import _ = require("phaser");
import Area = require("../../Frontend/Classes/Controllers/GameArea");
import BuildUpTypes = require("../../Frontend/Classes/Services/buildUpTypes");
import Coordinates = require("../../Frontend/Classes/Controllers/FieldCoordinates");
import FieldCoordinatesData = Coordinates.FieldCoordinatesData;

describe("GameArea", () => {
    var timerCallback: jasmine.Spy;
    var game: Phaser.Game;
    //var el = new HTMLElement();
    beforeEach(() => {
        timerCallback = jasmine.createSpy("timerCallback");
        jasmine.clock().install();
        //game = new Phaser.Game(200, 200, Phaser.AUTO, el);
        
        game = new Phaser.Game();
        ///setTimeout(() => {
        //    expect(game.width).toBe(800);
        //    expect(game.height).toBe(600);
        //    expect(game).not.toBe(null);
        //    expect(game.add).not.toBe(null);
        //    expect(game.add.graphics).not.toBe(null);
        ///}, 0);
    });

    afterEach(() => {
        jasmine.clock().uninstall();
    });

    it("creates Kennel fields at the right position", () => {
        var area = new Area.GameArea();
        var data = [
            new BuildUpTypes.KennelField(),
            new BuildUpTypes.KennelField(),
            new BuildUpTypes.KennelField(),
            new BuildUpTypes.KennelField()
        ];
        var span = 30;
        var xStart = [380, 20, 140, 500];
        var yStart = [20, 140, 500, 380];
        var fc = new FieldCoordinatesData(span, xStart, yStart);
        var ac = new Coordinates.AreaCoordinates(1, fc);
        setTimeout(() => {
            area.addKennelFields(game, data, ac, 0xFF00CC);
            //timerCallback();
        }, 0);
        jasmine.clock().tick(0);
        //expect(timerCallback).toHaveBeenCalled();
        var pos0 = data[0].viewRepresentation.position;
        var pos1 = data[1].viewRepresentation.position;
        var pos2 = data[2].viewRepresentation.position;
        var pos3 = data[3].viewRepresentation.position;
        expect(pos0.x).toEqual(20);
        expect(pos0.y).toEqual(20);
        expect(pos1.x).toEqual(20);
        expect(pos1.y).toEqual(50);
        expect(pos2.x).toEqual(50);
        expect(pos2.y).toEqual(20);
        expect(pos3.x).toEqual(50);
        expect(pos3.y).toEqual(50);
    }, 10000);
});
