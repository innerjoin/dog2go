/*import Area = require("./Controllers/GameArea");
new Area.GameArea();*/


import gm = require("./Controllers/GameMaster");

require(["signalr.hubs"], () => {
    new gm.GameMaster();
});

