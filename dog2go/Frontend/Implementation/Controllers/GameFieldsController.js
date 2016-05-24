///<reference path="../../Library/JQuery/jqueryui.d.ts"/>
define(["require", "exports", "./FieldCoordinates"], function (require, exports, coords) {
    "use strict";
    var FieldCoordinates = coords.FieldCoordinates;
    var GameFieldController = (function () {
        function GameFieldController(game, scaleFactor) {
            var _this = this;
            this.allFields = [];
            this.kennelFields = [];
            this.endFiedls = [];
            this.buildFields = function (gameTable) {
                var currentPos = 0;
                for (var k = 0; k < gameTable.PlayerFieldAreas.length; k++) {
                    var area = gameTable.PlayerFieldAreas[k];
                    var current = area.Fields[0];
                    var areaPos = _this.fieldCoordinates.getAreaCoordinates(currentPos);
                    // create kennel fields           
                    _this.addKennelFields(area.KennelFields, areaPos, area.ColorCode);
                    var fieldNr = 0;
                    var endFields = _this.getEndFields(area.Fields);
                    // create destination fields
                    while (current) {
                        var color = 0xeeeeee;
                        if (current.Identifier === area.StartField.Identifier) {
                            var startField = current;
                            color = area.ColorCode;
                            var ex = areaPos.x;
                            var ey = areaPos.y;
                            // Generate Endfields from startfield
                            var finalField = startField.EndFieldEntry;
                            for (var j = 0; j < endFields.length; j++) {
                                ex += areaPos.xAltOffset;
                                ey += areaPos.yAltOffset;
                                finalField.viewRepresentation = _this.addField(ex, ey, color, finalField.Identifier);
                                _this.allFields.push(finalField);
                                finalField = _this.getFieldById(finalField.NextIdentifier, area.Fields);
                            }
                        }
                        current.viewRepresentation = _this.addField(areaPos.x, areaPos.y, color, current.Identifier);
                        _this.allFields.push(current);
                        // Calculate Position for next field 
                        if (fieldNr < 8 || fieldNr > 11) {
                            areaPos.x += areaPos.xOffset;
                            areaPos.y += areaPos.yOffset;
                        }
                        else {
                            areaPos.x += areaPos.xAltOffset;
                            areaPos.y += areaPos.yAltOffset;
                        }
                        current = _this.getFieldById(current.NextIdentifier, area.Fields);
                        fieldNr++;
                    }
                    currentPos++;
                }
            };
            this.scaleFactor = scaleFactor;
            this.game = game;
            var fc = new FieldCoordinates(scaleFactor);
            this.fieldCoordinates = fc.fourPlAyerMode;
            this.allFields = [];
        }
        Object.defineProperty(GameFieldController.prototype, "getKennelFields", {
            get: function () {
                return this.kennelFields;
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(GameFieldController.prototype, "getAllFields", {
            get: function () {
                return this.allFields;
            },
            enumerable: true,
            configurable: true
        });
        GameFieldController.prototype.addKennelFields = function (kennelFields, areaPos, color) {
            var kennelX = areaPos.x + 11 * areaPos.xOffset;
            var kennelY = areaPos.y + 11 * areaPos.yOffset;
            for (var i = 0; i < kennelFields.length; i++) {
                var kennelField = kennelFields[i];
                var xx = 0;
                var yy = 0;
                switch (i % 4) {
                    case 1:
                        xx = areaPos.xOffset;
                        yy = areaPos.yOffset;
                        break;
                    case 2:
                        xx = areaPos.xAltOffset;
                        yy = areaPos.yAltOffset;
                        break;
                    case 3:
                        xx = areaPos.xOffset + areaPos.xAltOffset;
                        yy = areaPos.yOffset + areaPos.yAltOffset;
                        break;
                    default: // do nothing
                }
                kennelField.viewRepresentation = this.addField(kennelX + xx, kennelY + yy, color, kennelField.Identifier);
                this.allFields.push(kennelField);
                this.kennelFields.push(kennelField);
            }
        };
        GameFieldController.prototype.addField = function (x, y, color, id) {
            var graphics = this.game.add.graphics(x, y); // positioning is relative to parent (in this case, to the game world as no parent is defined)
            graphics.beginFill(color, 1);
            graphics.lineStyle(this.scaleFactor * 2, 0x222222, 1);
            graphics.drawCircle(0, 0, this.scaleFactor * 30); //draw a circle relative to it's parent (in this case, the graphics object)
            graphics.endFill();
            var style = { font: "20px Arial", fill: "#000000", align: "center" };
            var text = this.game.make.text(2, 2, id + "", style);
            graphics.addChild(text);
            return graphics;
        };
        GameFieldController.prototype.isValidTargetField = function (targetField) {
            if (this.kennelFields.indexOf(targetField) > -1) {
                return false;
            }
            return true;
        };
        GameFieldController.prototype.getFieldById = function (id, fields) {
            for (var _i = 0, fields_1 = fields; _i < fields_1.length; _i++) {
                var field = fields_1[_i];
                if (id === field.Identifier) {
                    return field;
                }
            }
            return null;
        };
        GameFieldController.prototype.getFieldByIdOfAll = function (id) {
            for (var _i = 0, _a = this.allFields; _i < _a.length; _i++) {
                var field = _a[_i];
                if (id === field.Identifier) {
                    return field;
                }
            }
            return null;
        };
        GameFieldController.prototype.getFieldPosition = function (fieldId) {
            var result;
            for (var _i = 0, _a = this.allFields; _i < _a.length; _i++) {
                var field = _a[_i];
                if (field.Identifier === fieldId) {
                    result = field.viewRepresentation.position;
                    break;
                }
            }
            return result;
        };
        GameFieldController.prototype.getEndFields = function (fields) {
            var result = [];
            for (var _i = 0, fields_2 = fields; _i < fields_2.length; _i++) {
                var field = fields_2[_i];
                if (field.FieldType.localeCompare("dog2go.Backend.Model.EndField") === 0) {
                    this.endFiedls.push(field);
                    result.push(field);
                }
            }
            return result;
        };
        return GameFieldController;
    }());
    exports.GameFieldController = GameFieldController;
});
//# sourceMappingURL=GameFieldsController.js.map