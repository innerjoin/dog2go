/// <reference path="../../Library/Phaser/phaser.comments.d.ts"/>

class GameArea {
    constructor() {
       
        var chat = new ChatController();
        this.gameFieldService = GameFieldService.getInstance(this.buildFields.bind(this));
        const gameStates = {
            preload: this.preload.bind(this),
            create: this.create.bind(this)
        };
        this.game = new Phaser.Game(720, 720, Phaser.AUTO, "content", gameStates);

    }

    gameFieldService: GameFieldService;
    game:Phaser.Game;
    areas: PlayerFieldArea[] = [];
    fields: Phaser.Graphics[] = [];

    public buildFields(areasPar: PlayerFieldArea[]) {
        // Source of Create() when good data comes from server
    }
    // Remove this function when GameAreaData comes from server!
    static addTestData(): PlayerFieldArea[] {
        let areas: PlayerFieldArea[] = [];
        const colors: AreaColor[] = [AreaColor.Red, AreaColor.Blue, AreaColor.Yellow, AreaColor.Green];
        for (let i = 0; i < 4; i++) {
            const area = new PlayerFieldArea(colors[i]);
            areas.push(area);
        }
        return areas;
    }

    /* load game assets here, but not objects */
    preload() {
        this.areas = GameArea.addTestData();
        this.fields = [];
        
        this.game.load.image('meeple_blue', '../Frontend/Images/pawn_blue.png');

    }

    public static getFieldById(id: number, fields: MoveDestinationField[]) {
        console.log('Geeting fild:', id);
        for (var field of fields) {
            if (id == field.identifier) {
                return field;
            }
        }
        console.log('No Field Found by ID in Area', id, fields);
    }


    public addField(game: Phaser.Game, x: number, y: number, color: number): Phaser.Graphics {
        let graphics = game.add.graphics(x, y); // positioning is relative to parent (in this case, to the game world as no parent is defined)
        graphics.beginFill(color, 1);
        graphics.drawCircle(0, 0, 30); //draw a circle relative to it's parent (in this case, the graphics object)
        graphics.endFill();
        this.fields.push(graphics);
        return graphics;
    }
    public dropLimiter(item: Phaser.Sprite) {
        var nearest: Phaser.Graphics;
        var smallest: number = 99999999;
        var pos = item.world;
        this.fields.forEach((field) => {
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
    }

    //// This function is called when a full screen request comes in
    //onGoFullScreen() {
    //    // tell Phaser how you want it to handle scaling when you go full screen
    //    this.game.scale.fullScreenScaleMode = Phaser.ScaleManager.SHOW_ALL;
    //    // and this causes it to actually do it
    //    this.game.scale.refresh();
    //}
    //goFullScreen() {

    //}

    public create() {
        this.gameFieldService.getGameFieldData();
        
        var game = this.game;
        var cellSpan = 40;
        this.game.stage.backgroundColor = 0xddeeCC;
        let pos = 0;
        const xStart = [520, 40, 200, 680];
        const yStart = [40, 200, 680, 520];
        const x1 = [-cellSpan, 0, cellSpan, 0];
        const y1 = [0, cellSpan, 0, -cellSpan];
        const x2 = [0, cellSpan, 0, -cellSpan];
        const y2 = [cellSpan, 0, -cellSpan, 0];

        let area = this.areas[2];
        for (let area of this.areas) {
            let el = area.gameFields[0];
            let x = xStart[pos];
            let y = yStart[pos];
            for (let i = 0; i < area.gameFields.length; i++) {
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
                        finEl = finEl.next;
                    }
                }
                el.viewRepresentation = this.addField(game, x, y, color);
                // Calculate Position for next field 
                if (i < 8 || i > 11) {
                    x += x1[pos];
                    y += y1[pos];
                } else {
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
    }
    
}

/*window.onload = () => {
    console.log('GameArea Loaded');
    //var gameArea = new GameArea();
};*/