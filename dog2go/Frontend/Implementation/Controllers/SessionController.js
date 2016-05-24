define(["require", "exports", "../Services/SessionService"], function (require, exports, ss) {
    "use strict";
    var SessionService = ss.SessionService;
    var SessionController = (function () {
        function SessionController() {
            var _this = this;
            this.doSmthing = function () {
                console.log("doSmthing", _this);
            };
            this.sessionService = new SessionService(this.newSession, this.updateOpenGames);
            //this.checkConnection = this.checkConnection;
            //this.doSmthing = this.doSmthing;
        }
        //public checkConnection = () => {
        //    console.log("CheckConnection", this);
        //    var cookie: string = document.cookie;
        //    var gameHub = $.connection.gameHub;
        //    if (cookie.length == 0) {
        //        var name = "Hallo ich bins";
        //        this.sessionService.login(name, null);
        //    } else {
        //        var name = "Hallo ich bins";
        //        this.sessionService.login(name, cookie);
        //    }
        //}
        SessionController.prototype.newSession = function (cookie) {
            document.cookie = cookie;
            console.log('SessionController: newSession');
            document.cookie = cookie;
        };
        SessionController.prototype.updateOpenGames = function (games) {
            console.log('SessionController: updateOpenGamese', games);
        };
        return SessionController;
    }());
    exports.SessionController = SessionController;
});
//# sourceMappingURL=SessionController.js.map