import Service = require("../Services/ChatService");
import ChatService = Service.ChatService;

export class ChatController {
    private chatService: ChatService;
    constructor(tableId: number) {
        this.chatService = ChatService.getInstance(tableId, this.putMessage.bind(this), this.putSystemMessage.bind(this));
        $("#message").keypress((e) => {
            if (e.which === 13) this.sendChat(tableId);
        });
        $("#sendmessage").click(() => {
            this.sendChat(tableId);
        });
        $("#gameContent").focus();
    }

    private sendChat(tableId: number) {
        this.chatService.sendMessage($("#message").val(), tableId);
        $("#message").val("");
        $("#gameContent").focus();
    }

    public putMessage(name: string, message: string) {
        const encodedName = $("<div />").text(name).html();
        const encodedMsg = $("<div />").text(message).html();
        // Add the message to the page. 
        $("#chatBox").append(`<li><strong>${encodedName}</strong>:&nbsp;&nbsp;${encodedMsg}</li>`);
        $("#chatBox").animate({ scrollTop: $(document).height() }, "slow");     
    }

    public putSystemMessage(message: string) {
        const encodedMsg = $("<div />").text(message).html();
        // Add the message to the page. 
        $("#systemMessage").text(encodedMsg);
    }
}