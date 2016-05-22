export class ChatService {
    private static instance: ChatService = null;
    private tableId: number;
    constructor(tableId: number, callback: (name: string, message: string) => any,
                systemCallback: (message: string) => any) {
        this.tableId = tableId;
        if (ChatService.instance) {
            throw new Error("Error: ChatService instantiation failed. Singleton module! Use .getInstance() instead of new.");
        }
        const chatHub = $.connection.chatHub;
        $.connection.hub.qs = `tableId=${tableId}`;
        console.log("chatHub: ", $.connection.hub.qs);
        chatHub.client.broadcastMessage = (name: string, message: string, tableId: number) => {
            // will autoconvert string to int
            // ReSharper disable once CoercedEqualsUsing
            if (tableId == this.tableId) {
                callback(name, message);
            }
        };

        chatHub.client.broadcastSystemMessage = (message: string, tableId: number) => {
            // will autoconvert string to int
            // ReSharper disable once CoercedEqualsUsing
            if (tableId == this.tableId) {
                systemCallback(message);
            }
        };
        
        ChatService.instance = this;
    }

    public static getInstance(tableId: number, callback: (name: string, message: string) => any, systemCallback: (message: string) => any) {
        // Create new instance if callback is given
        if (ChatService.instance === null && callback !== null || tableId !== ChatService.instance.tableId) {
            ChatService.instance = new ChatService(tableId, callback, systemCallback);
        } else if (ChatService.instance === null) {
            throw new Error("Error: First call needs a callback!");
        }
        return ChatService.instance;
    }

    public sendMessage(message: string, tableId: number): void {
        var chatHub = $.connection.chatHub;
        $.connection.hub.start().done(() => {
            chatHub.server.sendMessage(message, tableId);
        });
    }
}
