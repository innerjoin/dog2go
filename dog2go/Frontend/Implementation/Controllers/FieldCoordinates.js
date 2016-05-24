define(["require", "exports"], function (require, exports) {
    "use strict";
    var FieldCoordinates = (function () {
        function FieldCoordinates(scaleFactor) {
            this.fourPlAyerMode = new FieldCoordinatesData(scaleFactor * 40, [scaleFactor * 510, scaleFactor * 30, scaleFactor * 190, scaleFactor * 670], [scaleFactor * 30, scaleFactor * 190, scaleFactor * 670, scaleFactor * 510]);
        }
        return FieldCoordinates;
    }());
    exports.FieldCoordinates = FieldCoordinates;
    var AreaCoordinates = (function () {
        function AreaCoordinates(position, coordinates) {
            this.x = coordinates.xStart[position];
            this.y = coordinates.yStart[position];
            this.xOffset = coordinates.xOffsetDefinitions[position];
            this.yOffset = coordinates.yOffsetDefinitions[position];
            this.xAltOffset = coordinates.xAlternativeOffsetDefinitions[position];
            this.yAltOffset = coordinates.yAlternativeOffsetDefinitions[position];
        }
        return AreaCoordinates;
    }());
    exports.AreaCoordinates = AreaCoordinates;
    var FieldCoordinatesData = (function () {
        function FieldCoordinatesData(cellSpan, xStart, yStart) {
            this.cellSpan = cellSpan;
            this.xStart = xStart;
            this.yStart = yStart;
            this.xOffsetDefinitions = [-cellSpan, 0, cellSpan, 0];
            this.yOffsetDefinitions = [0, cellSpan, 0, -cellSpan];
            this.xAlternativeOffsetDefinitions = [0, cellSpan, 0, -cellSpan];
            this.yAlternativeOffsetDefinitions = [cellSpan, 0, -cellSpan, 0];
        }
        FieldCoordinatesData.prototype.getAreaCoordinates = function (position) {
            return new AreaCoordinates(position, this);
        };
        return FieldCoordinatesData;
    }());
    exports.FieldCoordinatesData = FieldCoordinatesData;
});
//# sourceMappingURL=FieldCoordinates.js.map