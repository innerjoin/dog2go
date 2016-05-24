/*import Area = require("./Controllers/GameArea");
new Area.GameArea();*/


import gm = require("./Controllers/GameMaster");

require(["signalr.hubs", "jquery-ui-touch"], () => {
    var id = $("#gameIdentifier").val();
    new gm.GameMaster(id);
});

