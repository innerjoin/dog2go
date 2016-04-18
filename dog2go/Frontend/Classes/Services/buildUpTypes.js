var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
define(["require", "exports"], function (require, exports) {
    "use strict";
    (function (AreaColor) {
        AreaColor[AreaColor["Red"] = 16711680] = "Red";
        AreaColor[AreaColor["Blue"] = 255] = "Blue";
        AreaColor[AreaColor["Green"] = 65280] = "Green";
        AreaColor[AreaColor["Yellow"] = 15582739] = "Yellow";
    })(exports.AreaColor || (exports.AreaColor = {}));
    var AreaColor = exports.AreaColor;
    var MoveDestinationField = (function () {
        function MoveDestinationField(previous) {
            this.previous = previous;
            var self = this;
            if (previous instanceof StartField && self instanceof EndField) {
                previous.setEndFieldEntry(self);
            }
            else if (previous != null) {
                previous.setNext(self);
            }
        }
        MoveDestinationField.prototype.setNext = function (next) {
            this.next = next;
        };
        return MoveDestinationField;
    }());
    exports.MoveDestinationField = MoveDestinationField;
    var KennelField = (function (_super) {
        __extends(KennelField, _super);
        function KennelField() {
            _super.call(this, null);
        }
        return KennelField;
    }(MoveDestinationField));
    exports.KennelField = KennelField;
    var EndField = (function (_super) {
        __extends(EndField, _super);
        function EndField(previous) {
            _super.call(this, previous);
        }
        return EndField;
    }(MoveDestinationField));
    exports.EndField = EndField;
    var StartField = (function (_super) {
        __extends(StartField, _super);
        function StartField(previous) {
            _super.call(this, previous);
        }
        StartField.prototype.setEndFieldEntry = function (next) {
            this.endFieldEntry = next;
        };
        return StartField;
    }(MoveDestinationField));
    exports.StartField = StartField;
    var PlayerFieldArea = (function () {
        function PlayerFieldArea(color) {
            this.kennelFields = [];
            this.gameFields = [];
            this.endFields = [];
            this.color = color;
            this.createFields();
        }
        PlayerFieldArea.prototype.createFields = function () {
            var prev = null;
            var field;
            var i;
            // create the 4 fields before the start field
            for (i = 0; i < 4; i++) {
                field = new MoveDestinationField(prev);
                this.gameFields.push(field);
                prev = field;
            }
            // create the start field itself
            var startField = new StartField(prev);
            this.gameFields.push(startField);
            // create the 11 fields after the start field
            prev = startField;
            for (i = 0; i < 11; i++) {
                field = new MoveDestinationField(prev);
                this.gameFields.push(field);
                prev = field;
            }
            // create the 4 end fields 
            prev = startField;
            for (i = 0; i < 4; i++) {
                field = new EndField(prev);
                this.endFields.push(field);
                prev = field;
            }
            // create the 4 kennel fields
            for (i = 0; i < 4; i++) {
                this.kennelFields.push(new KennelField());
            }
        };
        return PlayerFieldArea;
    }());
    exports.PlayerFieldArea = PlayerFieldArea;
});
//# sourceMappingURL=buildUpTypes.js.map