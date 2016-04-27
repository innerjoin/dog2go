/*import Area = require("./Controllers/GameArea");
new Area.GameArea();*/


import gm = require("./Controllers/GameMaster");

require(["signalr.hubs", "jquery-ui"], () => {
    new gm.GameMaster();
});

