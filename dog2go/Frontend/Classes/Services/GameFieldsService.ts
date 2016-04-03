
class GameFieldService {
    static getGameFieldData(callback: (ev: PlayerFieldArea[])=> any):void {
        var gameHub = $.connection.gameHub;
        gameHub.client.createGameTable = function(areas: PlayerFieldArea[]) {
            callback(areas);
        }
        $.connection.hub.start().done(function() {
            gameHub.server.sendGameTable();
        });
    }
}

