/// <reference path="../../Scripts/definitions/phaser.comments.d.ts"/>

/// <reference path="Preloader.ts"/>


module Castelmania {
    export class Game extends Phaser.Game {
        constructor() {
            super(800, 600, Phaser.AUTO, 'content', null);
            this.state.add('Boot', Boot, false);
            this.state.add('PreLoader', Preloader, false);
            this.state.add('MainMenu', MainMenu, false);
            this.state.add('Level1', Level1, false);
            
            this.state.start('Boot');
        }
    }


}
window.onload = () => {
    var game = new Castelmania.Game();
};