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
        this.Fields = [];
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
            this.Fields.push(field);
            prev = field;
        }
        // create the start field itself
        var startField = new StartField(prev);
        this.Fields.push(startField);
        // create the 11 fields after the start field
        prev = startField;
        for (i = 0; i < 11; i++) {
            field = new MoveDestinationField(prev);
            this.Fields.push(field);
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
var GameArea = (function (_super) {
    __extends(GameArea, _super);
    function GameArea() {
        _super.call(this);
        this.areas = [];
        this.fields = [];
        var chat = new ChatController();
        this.gameFieldService = GameFieldService.getInstance(this.buildFields.bind(this));
        var gameStates = {
            preload: this.preload,
            create: this.create
        };
        this.game = new Phaser.Game(720, 720, Phaser.AUTO, "content", gameStates);
        this.game.state.add('GameArea', this, false);
        this.game.state.start('GameArea');
    }
    /* load game assets here, but not objects */
    GameArea.prototype.preload = function () {
        this.areas = addTestData();
        this.fields = [];
        this.game.load.image('meeple_blue', '../Frontend/Images/pawn_blue.png');
    };
    GameArea.getFieldById = function (id, fields) {
        console.log('Geeting fild:', id);
        for (var _i = 0; _i < fields.length; _i++) {
            var field = fields[_i];
            if (id == field.Identifier) {
                return field;
            }
        }
        console.log('No Field Found by ID in Area', id, fields);
    };
    GameArea.prototype.buildFields = function (areasPar) {
        var game = this.game;
        var cellSpan = 40;
        this.game.stage.backgroundColor = 0xddeeCC;
        var pos = 0;
        var xStart = [520, 40, 200, 680];
        var yStart = [40, 200, 680, 520];
        var x1 = [-cellSpan, 0, cellSpan, 0];
        var y1 = [0, cellSpan, 0, -cellSpan];
        var x2 = [0, cellSpan, 0, -cellSpan];
        var y2 = [cellSpan, 0, -cellSpan, 0];
        //let area = this.areas[2];
        /*for (let area of areasPar) {
            let el = area.Fields[0];
            let x = xStart[pos];
            let y = yStart[pos];
            for (let i = 0; i < area.Fields.length; i++) {
                var color = 0xeeeeee;
                if (el instanceof StartField) {
                    color = area.color;
                    let ex = x;
                    let ey = y;
                    let finEl = el.endFieldEntry;
                    for (let j = 0; j < area.endFields.length; j++) {
                        ex += x2[pos];
                        ey += y2[pos];
                        el.viewRepresentation = this.addField(game, ex, ey, color);
                        finEl = GameArea.getFieldById(finEl.NextIdentifier, area.Fields); //finEl.next;
                    }
                }
                if (el !== undefined) {
                    el.viewRepresentation = this.addField(game, x, y, color);
                }
                // Calculate Position for next field
                if (i < 8 || i > 11) {
                    x += x1[pos];
                    y += y1[pos];
                } else {
                    x += x2[pos];
                    y += y2[pos];
                }

                el = GameArea.getFieldById(el.NextIdentifier, area.Fields);
            }
            pos++;
        }*/
        var meepleBlue = this.game.add.sprite(this.game.world.centerX, this.game.world.centerY, 'meeple_blue');
        meepleBlue.anchor.setTo(0.5, 0.5);
        meepleBlue.scale.setTo(0.08, 0.08);
        meepleBlue.inputEnabled = true;
        meepleBlue.input.enableDrag();
        meepleBlue.input.enableSnap(40, 40, false, true);
        console.log("meeples parent is: ", meepleBlue.parent);
        meepleBlue.events.onDragStop.add(this.dropLimiter, this);
    };
    GameArea.prototype.addField = function (game, x, y, color) {
        var graphics = game.add.graphics(x, y); // positioning is relative to parent (in this case, to the game world as no parent is defined)
        graphics.beginFill(color, 1);
        graphics.drawCircle(0, 0, 20); //draw a circle relative to it's parent (in this case, the graphics object)
        graphics.endFill();
        this.fields.push(graphics);
        return graphics;
    };
    GameArea.prototype.dropLimiter = function (item) {
        var nearest;
        var smallest = 99999999;
        var pos = item.world;
        this.fields.forEach(function (field) {
            var fieldPos = field.world;
            var dist = fieldPos.distance(pos, true);
            if (!(smallest < dist)) {
                smallest = dist;
                nearest = field;
            }
        });
        if (nearest != null) {
            item.x = nearest.x;
            item.y = nearest.y;
        }
    };
    //// This function is called when a full screen request comes in
    //onGoFullScreen() {
    //    // tell Phaser how you want it to handle scaling when you go full screen
    //    this.game.scale.fullScreenScaleMode = Phaser.ScaleManager.SHOW_ALL;
    //    // and this causes it to actually do it
    //    this.game.scale.refresh();
    //}
    //goFullScreen() {
    //}
    GameArea.prototype.create = function () {
        this.gameFieldService.getGameFieldData();
        // would allow to go to fullscreen on desktop systems
        //this.game.scale.onFullScreenInit.add(GameArea.prototype.onGoFullScreen, this);        
        //this.game.input.onTap.add(() => { this.game.scale.startFullScreen(true); }, this);
    };
    return GameArea;
})(Phaser.State);
window.onload = function () {
    var gameArea = new GameArea();
};
