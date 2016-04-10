/// <reference path="../Library/JQuery/jquery.d.ts"/>
import Area = require("./Controllers/GameArea");
import GameArea = Area.GameArea;

//var base_path = '../Frontend/';
//var library = base_path + 'Library/';
//var controllers = base_path + 'Classes/Controllers/';
//var services = base_path + 'Classes/Services/';

//var scriptArray = [
//    library + 'requirejs/require.js',
//    library + 'Phaser/phaser.js',
//    library + 'SignalR/jquery.signalR-2.2.0.min.js',
//    '/signalr/hubs',
//    services + 'ChatService.js',
//    controllers + 'ChatController.js',
//    controllers + 'testController.js',
//    services + 'GameFieldsService.js',
//    services + 'buildUpTypes.js',
//    controllers + 'FieldCoordinates.js',
//    controllers + 'GameArea.js'
    
//];


//var jq: any = $;
//// Function to load multiple scripts
//jq.getMultiScripts = function (arr, path) {
//    var _arr = $.map(arr, function (scr) {
//        return $.getScript((path || "") + scr);
//    });

//    _arr.push($.Deferred(function (deferred) {
//        $(deferred.resolve);
//    }));

//    return $.when.apply($, _arr);
//}

//jq.getMultiScripts(scriptArray).done(function () {
//    console.log('Loaded Phaser', Phaser);
alert("start game area");
    var game: Area.GameArea = new GameArea();
    //console.log('Loaded GameArea: ', game);
//}).fail((error) => {
//    console.error('Load Error occured:', error);
//}).always(() => {
//    console.log('Loader');
//});
