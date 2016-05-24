/*import Area = require("./Controllers/GameArea");
new Area.GameArea();*/
define(["require", "exports", "./Controllers/GameMaster"], function (require, exports, gm) {
    "use strict";
    require(["signalr.hubs", "jquery-ui-touch"], function () {
        var id = $("#gameIdentifier").val();
        new gm.GameMaster(id);
    });
});
//# sourceMappingURL=GameStartupConfig.js.map