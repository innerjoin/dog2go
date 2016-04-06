/// <reference path="../Library/requirejs/require.d.ts"/>

require.config({
    baseUrl: '/Frontend',
    paths: {
        'jquery': 'Library/JQuery/jquery-2.2.1',
        'signalr.core': 'Library/SignalR/jquery.signalR-2.2.0.min',
        'signalr.hubs': '/signalr/hubs?',
        //'phaser': 'Library/Phaser/phaser'
    },
    shim: {
        'jquery': {
            exports: '$'
        },
        'signalr.core': {
            deps: ['jquery'],
           // exports: '$.connection'
        },
        'signalr.hubs': {
            deps: ['signalr.core']
        }
    }
});

// Starter Class
//import ga = require("Controllers/ChatController");

//import cs = require("Services/ChatService");
import tc = require("Controllers/TestController");
// load common used Libraries here and start app
require(['jquery', 'signalr.hubs'],
    ($) => {
        
        console.log('Halo i bi au no da!!!');
        //var gameArea = new ga.GameArea();
});
