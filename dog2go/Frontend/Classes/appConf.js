var base_path = '../Frontend/';
var library = base_path + 'Library/';
var controllers = base_path + 'Classes/Controllers/';
var services = base_path + 'Classes/Services/';

var script_arr = [
    library + 'Phaser/phaser.js',
    library + 'SignalR/jquery.signalR-2.2.0.min.js',
    '/signalr/hubs',
    //controllers + 'GameArea.js'
];

// Function to load multiple scripts
$.getMultiScripts = function (arr, path) {
    var _arr = $.map(arr, function (scr) {
        return $.getScript((path || "") + scr);
    });

    _arr.push($.Deferred(function (deferred) {
        $(deferred.resolve);
    }));

    return $.when.apply($, _arr);
}

$.getMultiScripts(script_arr).done(function () {
    console.log('Loaded Phaser', Phaser);
}).fail((error) => {
    console.error('Load Error occured:', error);
}).always(() => {
    console.log('Loader');
});
