import _phaser = require("phaser");
import ga = require("../../../Frontend/Classes/Controllers/GameArea");
import GameArea = ga.GameArea;
import gm = require("../../../Frontend/Classes/Model/GameModel");
import Coordinates = require("../../../Frontend/Classes/Controllers/FieldCoordinates");
import FieldCoordinatesData = Coordinates.FieldCoordinatesData;

describe("GameArea - ", () => {

    var gameFieldService;
    var timerCallback: jasmine.Spy;
    var gameMock;

    const gameTableId = 4474774;
    beforeAll(() => {
        gameFieldService = { getGameFieldData: (gameId) => { } };
        spyOn(gameFieldService, "getGameFieldData");

        gameMock = {
            load: { image: (name, url) => {} },
            scale: { refresh: () => { } },
            device: { desktop: true },
            plugins: {add: () => {} }
        };
        spyOn(gameMock.plugins, "add");

        spyOn(Phaser, "Game").and.callFake(() => {
            return gameMock;
        });
    });

    


    describe("Initialisation", () => {
        beforeAll(() => {
            
        });

        beforeEach(() => {
            timerCallback = jasmine.createSpy("timerCallback");
            jasmine.clock().install();
            
        });

        it("Initialize without error (in testMode)", () => {
            gameFieldService.getGameFieldData.calls.reset();
            GameArea.bind(gameFieldService);

            var gameArea = new GameArea(gameTableId, true);
            gameArea.gameFieldService = gameFieldService;

            gameMock.plugins.add.calls.reset();
            gameArea.init();
            expect(gameMock.plugins.add).toHaveBeenCalledWith(Phaser.Plugin.SaveCPU);

            gameArea.preload();
            expect(gameFieldService.getGameFieldData).toHaveBeenCalledWith(gameTableId);
            
            expect(true).toBe(true);

        }, 5000);




    });

    
    beforeEach(() => {

    });
    

    
});
