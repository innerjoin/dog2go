$(function () {
    // Declare a proxy to reference the hub. 
    var game = $.connection.gameHub;
    // Create a function that the hub can call to broadcast messages.
    //game.client.createGameTable = function (areas) {
    //   console.log('GameHub.js got Areas!');
    //    console.log(areas);
    //};
    
    // Start the connection.
    /*$.connection.hub.start().done(function () {
        // Call the Send game table method on the hub. 
        //game.server.sendGameTable();
    });*/
});