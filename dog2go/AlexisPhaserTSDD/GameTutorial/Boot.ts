
module Castelmania {
    export class Boot extends Phaser.State{
        preload() {
            this.load.image('preloadBar', 'gameStuff/loader.png'); 
        }

        create() {
            this.input.maxPointers = 1;

            this.stage.disableVisibilityChange = true;

            if (this.game.device.desktop) {
                this.scale.pageAlignHorizontally = true;
            } else {
                // MObile Settings
                //this.scale.scaleMode = Phaser.StageScaleMode.SHOW_ALL;
                
                this.scale.minWidth = 480;
                this.scale.minHeight = 260;
                this.scale.maxWidth = 1024;
                this.scale.maxHeight = 768;
                this.scale.forceLandscape = true;
                this.scale.pageAlignHorizontally = true;
                //this.stage.
                //this.scale.setScreenSize(true);
            }
            this.game.state.start('PreLoader', true, false);
        }
    }
}