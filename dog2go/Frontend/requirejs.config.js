require.config({
    baseUrl: "/Frontend/",
    paths: {
            "jquery": "Library/JQuery/jquery-2.2.3.min",
            "jquery-ui": "Library/JQuery/jquery-ui-1.11.4-dnd-only",
            "jquery-ui-touch": "Library/JQuery/jquery.ui.touch-punch.min",
            "signalr.core": "Library/SignalR/jquery.signalR-2.2.0",
            "signalr.hubs": "/signalr/hubs?",
            "domReady": "Library/domReady",
            "phaser": "Library/Phaser/phaser.min",
            "savecpu": "Library/Phaser/SaveCPU"
    },
    shim: {
        "savecpu": {
            deps: ["phaser"]
        },
        "jquery": {
            exports: "$"
        },
        "jquery-ui": {
            deps: ["jquery"],
            exports: "$"
        },
        "jquery-ui-touch": {
            deps: ["jquery-ui"],
            exports: "$"
        },
        "signalr.core": {
                deps: ["jquery"],
                exports: "$.connection"
        },
        "signalr.hubs": {
                deps: ["signalr.core"]
        }
    }
});

