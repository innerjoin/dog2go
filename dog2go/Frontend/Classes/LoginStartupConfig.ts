
import sc = require("./Controllers/SessionController");

require(["signalr.hubs"], () => {
    new sc.SessionController();
});

