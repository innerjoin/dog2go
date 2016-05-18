import Service = require("../Services/ChatService");
import ChatService = Service.ChatService;

export class ChatController {
    private chatService: ChatService;
    constructor() {
        this.chatService = ChatService.getInstance(this.putMessage.bind(this), this.putSystemMessage.bind(this));
        $("#message").keypress((e) => {
            if (e.which === 13) this.sendChat();
        });
        $("#sendmessage").click(() => {
            this.sendChat();
        });
        $("#gameContent").focus();
    }

    private sendChat() {
        this.chatService.sendMessage($("#message").val());
        $("#message").val("");
        $("#gameContent").focus();
    }

    public putMessage(name: string, message: string) {
        const encodedName = $("<div />").text(name).html();
        const encodedMsg = $("<div />").text(message).html();
        // Add the message to the page. 
        $("#chatBox").append(`<li><strong>${encodedName}</strong>:&nbsp;&nbsp;${encodedMsg}</li>`);
        this.scrollToBottom();
    }

    public putSystemMessage(message: string) {
        const encodedMsg = $("<div />").text(message).html();
        // Add the message to the page. 
        $("#chatBox").append(`<li><strong>${encodedMsg}</strong></li>`);
        this.scrollToBottom();
    }

    public scrollToBottom() {
        // auto scroll chat to bottom
        $('#chatBox').animate({ scrollTop: $(document).height() }, "slow");     
    }
}