require.config({
    baseUrl: "/Frontend/",
    paths: {
        "jquery": "Library/JQuery/jquery-2.2.3.min",
        "jquery-ui": "Library/JQuery/jquery-ui-1.11.4-dnd-only",
        "signalr.core": "Library/SignalR/jquery.signalR-2.2.0",//jquery.signalR-2.2.0.min
        "signalr.hubs": "/signalr/hubs?",
        "domReady": "Library/domReady",
        "phaser": "Library/Phaser/phaser.min"
    },
    shim: {
        "jquery": {
            exports: "$"
        },
        "jquery-ui": {
            deps: ["jquery"],
            exports: "$"
        },
        "signalr.core": {
                deps: ["jquery"],
                exports: "$.connection"
        },
        "signalr.hubs": {
                deps: ["signalr.core"],
        }
    }
});

