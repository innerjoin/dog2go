
import ss = require("../Services/SessionService");
import SessionService = ss.SessionService;


export class SessionController {
    private sessionService: SessionService;
    constructor() {
        this.sessionService = new SessionService(this.newSession, this.updateOpenGames);
    }

    newSession(cookie: string) {
        console.log('SessionController: newSession');
        document.cookie = cookie;
    }
    
    updateOpenGames(games: any) { // TODO: Change to typed
        console.log('SessionController: updateOpenGamese', games);
    }
}