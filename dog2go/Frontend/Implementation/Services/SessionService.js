define(["require", "exports"], function (require, exports) {
    "use strict";
    var SessionService = (function () {
        function SessionService(newSession, updateOpenGames) {
            if (SessionService.instance) {
                throw new Error("Error: SessionService instantiation failed. Singleton module! Use .getInstance() instead of new.");
            }
            //var sessionHub = $.connection.sessionHub;
            //var gameHub = $.connection.gameHub;
            //gameHub.client.newSession = function (cookie: string){
            //    newSession(cookie);
            //}
            //gameHub.client.updateOpenGames = function(games: GameTable[]) {
            //    console.log(games);
            //    updateOpenGames(games);
            //}
            //gameHub.client.backToGame = function(table: IGameTable, cards: Card[]) {
            //    console.log(table, cards);
            //}
            SessionService.instance = this;
        }
        SessionService.getInstance = function (newSession, updateOpenGames) {
            // Create new instance if callback is given
            if (SessionService.instance === null && newSession !== null) {
                SessionService.instance = new SessionService(newSession, updateOpenGames);
            }
            else if (SessionService.instance === null) {
                throw new Error("Error: First call needs a callback!");
            }
            return SessionService.instance;
        };
        SessionService.instance = null;
        return SessionService;
    }());
    exports.SessionService = SessionService;
});
//# sourceMappingURL=SessionService.js.map