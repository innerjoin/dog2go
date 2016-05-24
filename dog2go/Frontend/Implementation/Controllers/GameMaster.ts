import cc = require("./ChatController");
import ChatController = cc.ChatController;
import gac = require("./GameArea");
import GameArea = gac.GameArea;

export class GameMaster {
    private chatController: ChatController;
    private gameArea: GameArea;

    constructor(tableId: number) {
        this.chatController = new ChatController(tableId);
        this.gameArea = new GameArea(tableId);
    }   
}