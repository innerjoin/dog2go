export class SessionService {
    private static instance: SessionService = null;
    constructor(newSession: (cookie: string) => any,
        updateOpenGames: (games: any) => any) {// TODO: Change to Typed
        if (SessionService.instance) {
            throw new Error("Error: SessionService instantiation failed. Singleton module! Use .getInstance() instead of new.");
        }
        var sessionHub = $.connection.sessionHub;

        sessionHub.client.newSession = function (cookie: string) {
            newSession(cookie);
        };
        
        sessionHub.client.updateOpenGames = function (games: any) {// TODO: Change to Typed
            updateOpenGames(games);
        }
        SessionService.instance = this;
    }

    public static getInstance(newSession: (cookie: string) => any,
        updateOpenGames: (games: any) => any) { // TODO: Change to Typed
        // Create new instance if callback is given
        if (SessionService.instance === null && newSession !== null) {
            SessionService.instance = new SessionService(newSession, updateOpenGames);
        } else if (SessionService.instance === null) {
            throw new Error("Error: First call needs a callback!");
        }
        return SessionService.instance;
    }

    public login(name: string, cookie: string): void {
        var sessionHub = $.connection.sessionHub;
        $.connection.hub.start().done(() => {
            sessionHub.server.login(name, cookie);
        });
    }

    public createGame(): void {
        var sessionHub = $.connection.sessionHub;

        $.connection.hub.start().done(() => {
            sessionHub.server.createGame();
        });
    }
}
