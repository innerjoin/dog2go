
import ss = require("../Services/SessionService");
import SessionService = ss.SessionService;



export class SessionController {
    private sessionService: SessionService;
    constructor() {
        this.sessionService = new SessionService(this.newSession, this.updateOpenGames);
        this.checkConnection();
    }

    public checkConnection = () => {
        var cookie: string = document.cookie;
        var gameHub = $.connection.gameHub;
        if (cookie.length == 0) {
            var name = "Hallo ich bins";
            this.sessionService.login(name, null);
        } else {
            var name = "Hallo ich bins";
            this.sessionService.login(name, cookie);
        }
        
    }

    newSession(cookie: string) {
        document.cookie = cookie;
        console.log('SessionController: newSession');
    }
    
    updateOpenGames(games: any) { // TODO: Change to typed
        console.log('SessionController: updateOpenGamese', games);
    }
}