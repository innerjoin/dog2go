import Service = require("../Services/ChatService");

export class ChatController {
    private chatService: Service.ChatService;
    constructor() {
        this.chatService = Service.ChatService.getInstance(this.putMessage);
        $('#sendmessage').click(() => {
            this.chatService.sendMessage($('#displayname').val(), $('#message').val());
            $('#message').val('').focus();
        });

        $('#displayname').val('EveryBody heisst Hans');//(prompt('Enter your name:', ''));
        // Set initial focus to message input box.  
        $('#message').focus();
    }

    public putMessage(name: string, message: string) {
        var encodedName = $('<div />').text(name).html();
        var encodedMsg = $('<div />').text(message).html();
        // Add the message to the page. 
        $('#chatBox').append('<li><strong>' + encodedName
            + '</strong>:&nbsp;&nbsp;' + encodedMsg + '</li>');
    }
}