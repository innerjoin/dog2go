
//import sc = require("./SessionController");
//import SessionController = sc.SessionController;
import cc = require("./ChatController");
import ChatController = cc.ChatController;
import gac = require("./GameArea");
import GameArea = gac.GameArea;


export class GameMaster {
    //private sessionController: SessionController;
    private chatController: ChatController;
    private gameArea: GameArea;


    constructor() {
        //this.sessionController = new SessionController();
        this.chatController = new ChatController();
        this.gameArea = new GameArea();
    }
}