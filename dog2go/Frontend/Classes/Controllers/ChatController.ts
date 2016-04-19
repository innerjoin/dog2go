/// <reference path="../Services/ChatService.ts"/>
import Service = require("../Services/ChatService");
import ChatService = Service.ChatService;

export class ChatController {
    private chatService: ChatService;
    constructor() {
        this.chatService = ChatService.getInstance(this.putMessage);
        $("#message").keypress((e) => {
            if (e.which === 13) this.sendChat();
        });
        $("#sendmessage").click(() => {
            this.sendChat();
        });

        $("#displayname").val("TEST");
        // Set initial focus to message input box.  
        //$('#message').focus();
        $("#gameContent").focus();

    }

    private sendChat() {
        this.chatService.sendMessage($("#displayname").val(), $("#message").val());
        $("#message").val("");//.focus();
        $("#gameContent").focus();
    }

    public putMessage(name: string, message: string) {
        const encodedName = $("<div />").text(name).html();
        const encodedMsg = $("<div />").text(message).html();
        // Add the message to the page. 
        $("#chatBox").append(`<li><strong>${encodedName}</strong>:&nbsp;&nbsp;${encodedMsg}</li>`);
    }
}