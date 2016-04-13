
import ss = require("../Services/SessionService");
import SessionService = ss.SessionService;



export class SessionController {
    private sessionService: SessionService;
    constructor() {
        this.sessionService = new SessionService(this.newSession, this.updateOpenGames);
    }

    newSession() {
        console.log('SessionController: newSession');
    }
    
    updateOpenGames(games: any) { // TODO: Change to typed
        console.log('SessionController: updateOpenGamese', games);
    }
}