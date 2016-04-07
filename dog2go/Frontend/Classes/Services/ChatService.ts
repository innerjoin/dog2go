class ChatService {
    private static instance: ChatService = null;
    constructor(callback: (name: string, message: string) => any) {
        if (ChatService.instance) {
            throw new Error("Error: ChatService instantiation failed. Singleton module! Use .getInstance() instead of new.");
        }
        var chatHub = $.connection.chatHub;

        chatHub.client.broadcastMessage = function (name: string, message: string) {
            callback(name, message);
            // Html encode display name and message. 
            /*var encodedName = $('<div />').text(name).html();
            var encodedMsg = $('<div />').text(message).html();
            // Add the message to the page. 
            $('#chatBox').append('<li><strong>' + encodedName
                + '</strong>:&nbsp;&nbsp;' + encodedMsg + '</li>');*/
        };
        
        ChatService.instance = this;
    }

    public static getInstance(callback: (name: string, message: string) => any) {
        // Create new instance if callback is given
        if (ChatService.instance === null && callback !== null) {
            ChatService.instance = new ChatService(callback);
        } else if (ChatService.instance === null) {
            throw new Error("Error: First call needs a callback!");
        }
        return ChatService.instance;
    }

    public sendMessage(name: string, message: string): void {
        var chatHub = $.connection.chatHub;
        
        $.connection.hub.start().done(() => {
            chatHub.server.sendTo(name, message);
        });
    }
}
