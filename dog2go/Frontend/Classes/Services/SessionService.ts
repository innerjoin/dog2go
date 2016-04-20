export class SessionService {
    private static instance: SessionService = null;
    constructor(newSession: (cookie: string) => any,
        updateOpenGames: (games: any) => any) { // TODO: Change to Typed
        if (SessionService.instance) {
            throw new Error("Error: SessionService instantiation failed. Singleton module! Use .getInstance() instead of new.");
        }
        //var sessionHub = $.connection.sessionHub;
        var gameHub = $.connection.gameHub;
        gameHub.client.newSession = function (cookie: string){
            newSession(cookie);
        }
        
        gameHub.client.updateOpenGames = function(games: GameTable[]) {
            console.log(games);
            updateOpenGames(games);
        }

        gameHub.client.backToGame = function(table: IGameTable, cards: Card[]) {
            console.log(table, cards);
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

    //public login(name: string, cookie: string): void {
    //    var gameHub = $.connection.gameHub;
    //    $.connection.hub.start().done(() => {
    //        //gameHub.server.login(name, null);
    //    });
    //}
    
}
