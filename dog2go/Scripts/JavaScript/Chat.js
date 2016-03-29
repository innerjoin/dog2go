/// <reference path="../Scripts/jquery-2.2.1.js" />
/// <reference path="../Scripts/jquery.signalR-2.2.0.js" />
$(function () {
    // Declare a proxy to reference the hub.
    var chat = $.connection.chatHub;
    // Create a function that the hub can call to broadcast messages.
    chat.client.broadcastMessage = function (name, message) {
        // Html encode display name and message.
        alert(name + " " + message);
        //var encodedName = $('<div />').text(name).html();
        //var encodedMsg = $('<div />').text(message).html();
        // Add the message to the page.
        //$('#discussion').append('<li><strong>' + encodedName
        //    + '</strong>:&nbsp;&nbsp;' + encodedMsg + '</li>');
    };

    chat.client.addChatMessage = function (name, message) {
        alert("addChatMessage mit name=" + name);
        $('#discussion').append('<li>' + name + '</li>');
    };
    // Get the user name and store it to prepend to messages.
    $('#displayname').val(prompt('Enter your name:', ''));
    // Set initial focus to message input box.
    $('#message').focus();
    // Start the connection.
    $.connection.hub.start().done(function () {
        $('#sendmessage').click(function () {
            // Call the Send method on the hub.
            chat.server.sendMessageFrom($('#displayname').val(), $('#message').val());
            // Clear text box and reset focus for next comment.
            $('#message').val('').focus();
        });
    });
});

    /*$(function() {
    var chatHubProxy = $.connection.chatHub;

    chatHubProxy.client.addNewMessageToPage = function(name, message) {
        // Add the message to the page. 
        /*$('#chatBox').append('<li><strong>' + htmlEncode(name)
            + '</strong>: ' + htmlEncode(message) + '<li>');
        $('#chatBox').append('<li><strong>' + name
            + '</strong>: ' + message + '<li>');
    };

    chatHubProxy.client.addChatMessage = function(message) {
        // Add the message to the page. 
        alert(message);
    };

    $.connection.hub.start().done(function() {
        $('#sendmessage').click(function() {
            // Call the Send method on the hub.
            chatHubProxy.server.sendMessageTo( /*$('#displayname').val()"test_enja", $('#message').val());
            // Clear text box and reset focus for next comment.
            $('#message').val('').focus();
        });
    });

    chatHubProxy.server.newContosoChatMessage(userName, message).done(function() {

    });
});*/