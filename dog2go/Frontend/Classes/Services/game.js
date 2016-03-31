/// <reference path="../../Library/Phaser/phaser.comments.d.ts"/>
var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
var AreaColor;
(function (AreaColor) {
    AreaColor[AreaColor["Red"] = 16711680] = "Red";
    AreaColor[AreaColor["Blue"] = 255] = "Blue";
    AreaColor[AreaColor["Green"] = 65280] = "Green";
    AreaColor[AreaColor["Yellow"] = 15582739] = "Yellow";
})(AreaColor || (AreaColor = {}));
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
})();
var EndField = (function (_super) {
    __extends(EndField, _super);
    function EndField(previous) {
        _super.call(this, previous);
    }
    return EndField;
})(MoveDestinationField);
var StartField = (function (_super) {
    __extends(StartField, _super);
    function StartField(previous) {
        _super.call(this, previous);
    }
    StartField.prototype.setEndFieldEntry = function (next) {
        this.endFieldEntry = next;
    };
    return StartField;
})(MoveDestinationField);
var Persontest = (function () {
    function Persontest() {
    }
    Persontest.prototype.setFirstName = function (value) {
        this.firstName = value;
    };
    Persontest.prototype.setLastName = function (value) {
        this.lastName = value;
    };
    Persontest.prototype.getFullName = function (lastNameFirst) {
        if (lastNameFirst === void 0) { lastNameFirst = false; }
        if (lastNameFirst) {
            return this.lastName + ", " + this.firstName;
        }
        return this.firstName + ", " + this.lastName;
    };
    return Persontest;
})();
var PlayerFieldArea = (function () {
    function PlayerFieldArea(color) {
        //kennelFields: MoveDestinationField[];
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
    };
    return PlayerFieldArea;
})();
function addTestData() {
    var areas = [];
    var colors = [AreaColor.Red, AreaColor.Blue, AreaColor.Yellow, AreaColor.Green];
    for (var i = 0; i < 4; i++) {
        var area = new PlayerFieldArea(colors[i]);
        areas.push(area);
    }
    return areas;
}
function addField(game, x, y, color) {
    var graphics = game.add.graphics(x, y);
    graphics.beginFill(color, 1);
    graphics.drawCircle(graphics.x, graphics.y, 20);
    graphics.endFill();
    return graphics;
}
var GameArea = (function () {
    function GameArea() {
        this.areas = [];
        var gameStates = {
            preload: this.preload,
            create: this.create
        };
        this.game = new Phaser.Game(720, 720, Phaser.AUTO, "content", gameStates);
    }
    /* load game assets here, but not objects */
    GameArea.prototype.preload = function () {
        this.areas = addTestData();
    };
    GameArea.prototype.create = function () {
        this.game.stage.backgroundColor = 0xddeeCC;
        var pos = 0;
        var xStart = [260, 20, 100, 340];
        var yStart = [20, 100, 340, 260];
        var x1 = [-20, 0, 20, 0];
        var y1 = [0, 20, 0, -20];
        var x2 = [0, 20, 0, -20];
        var y2 = [20, 0, -20, 0];
        for (var _i = 0, _a = this.areas; _i < _a.length; _i++) {
            var area = _a[_i];
            console.log(area);
            var el = area.gameFields[0];
            var x = xStart[pos]; //right 360; //left 40; //top 280; // bottom 120;
            var y = yStart[pos]; // right 260;// left 100; //top 20; // bottom 250;
            for (var i = 0; i < area.gameFields.length; i++) {
                console.log(x, y, el);
                var color = 0xeeeeee;
                if (el instanceof StartField) {
                    color = area.color;
                    var ex = x;
                    var ey = y;
                    var finEl = el.endFieldEntry;
                    for (var j = 0; j < area.endFields.length; j++) {
                        ex += x2[pos];
                        ey += y2[pos];
                        el.viewRepresentation = addField(this.game, ex, ey, color);
                        finEl = finEl.next;
                    }
                }
                el.viewRepresentation = addField(this.game, x, y, color);
                if (i < 8 || i > 11) {
                    x += x1[pos]; //x += 0; //left x += 0; //top x -= 20; //bottom x += 20;
                    y += y1[pos]; //y += -20;  //left y += 20; //top y += 0; //bottom y += 20;
                }
                else {
                    x += x2[pos]; //x += -20; //left x += 20; //top  x += 0; // bottom x += 20;
                    y += y2[pos]; //y += 0;  //left y += 0;  //top y += 20; // bottom y += -20;
                }
                el = el.next;
            } //while (el.next);
            pos++;
        }
    };
    return GameArea;
})();
window.onload = function () {
    var gameArea = new GameArea();
};
//# sourceMappingURL=game.js.map