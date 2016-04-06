var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
define(["require", "exports", "../Controllers/ChatController"], function (require, exports, ChatController) {
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
            var chat = new ChatController.ChatController();
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
                if (id == field.identifier) {
                    return field;
                }
            }
            console.log('No Field Found by ID in Area', id, fields);
        };
        GameArea.prototype.buildFields = function (areasPar) {
            // Source of Create() when good data comes from server
        };
        GameArea.prototype.addField = function (game, x, y, color) {
            var graphics = game.add.graphics(x, y); // positioning is relative to parent (in this case, to the game world as no parent is defined)
            graphics.beginFill(color, 1);
            graphics.drawCircle(0, 0, 30); //draw a circle relative to it's parent (in this case, the graphics object)
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
            var area = this.areas[2];
            for (var _i = 0, _a = this.areas; _i < _a.length; _i++) {
                var area_1 = _a[_i];
                var el = area_1.gameFields[0];
                var x = xStart[pos];
                var y = yStart[pos];
                for (var i = 0; i < area_1.gameFields.length; i++) {
                    var color = 0xeeeeee;
                    if (el instanceof StartField) {
                        color = area_1.color;
                        var ex = x;
                        var ey = y;
                        var finEl = el.endFieldEntry;
                        for (var j = 0; j < area_1.endFields.length; j++) {
                            ex += x2[pos];
                            ey += y2[pos];
                            el.viewRepresentation = this.addField(game, ex, ey, color);
                            finEl = finEl.next;
                        }
                    }
                    el.viewRepresentation = this.addField(game, x, y, color);
                    // Calculate Position for next field 
                    if (i < 8 || i > 11) {
                        x += x1[pos];
                        y += y1[pos];
                    }
                    else {
                        x += x2[pos];
                        y += y2[pos];
                    }
                    el = el.next;
                }
                pos++;
            }
            var meepleBlue = this.game.add.sprite(this.game.world.centerX, this.game.world.centerY, 'meeple_blue');
            meepleBlue.anchor.setTo(0.5, 0.5);
            meepleBlue.scale.setTo(0.08, 0.08);
            meepleBlue.inputEnabled = true;
            meepleBlue.input.enableDrag();
            meepleBlue.input.enableSnap(40, 40, false, true);
            console.log("meeples parent is: ", meepleBlue.parent);
            meepleBlue.events.onDragStop.add(this.dropLimiter, this);
            // would allow to go to fullscreen on desktop systems
            //this.game.scale.onFullScreenInit.add(GameArea.prototype.onGoFullScreen, this);        
            //this.game.input.onTap.add(() => { this.game.scale.startFullScreen(true); }, this);
        };
        return GameArea;
    })(Phaser.State);
    window.onload = function () {
        var gameArea = new GameArea();
    };
});
//# sourceMappingURL=game.js.map