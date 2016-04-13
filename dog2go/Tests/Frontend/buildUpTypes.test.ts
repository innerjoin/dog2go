import BuildUpTypes = require("../../Frontend/Classes/Services/buildUpTypes");
import AreaColor = BuildUpTypes.AreaColor;
import PlayerFieldArea = BuildUpTypes.PlayerFieldArea;

describe("buildUpTypes", () => {

    it("sets coloring", () => {
        var col = AreaColor.Red;
        var area = new PlayerFieldArea(col);
        expect(area.color).toBe(AreaColor.Red);
    });
});
