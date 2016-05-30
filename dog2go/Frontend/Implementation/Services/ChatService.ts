interface IChatMessage {
    [timeStamp: number]: string;
}

export class ChatService {
    private static instance: ChatService = null;
    private tableId: number;
    private systemMessages: IChatMessage = {};
    constructor(tableId: number, callback: (name: string, message: string) => any,
                systemCallback: (message: string) => any, stateCallback:(message:string)=> any) {
        this.tableId = tableId;
        if (ChatService.instance) {
            throw new Error("Error: ChatService instantiation failed. Singleton module! Use .getInstance() instead of new.");
        }
        const chatHub = $.connection.chatHub;
        $.connection.hub.qs = `tableId=${tableId}`;
        chatHub.client.broadcastMessage = (name: string, message: string, tableId: number) => {
            // will autoconvert string to int
            // ReSharper disable once CoercedEqualsUsing
            if (tableId == this.tableId) {
                callback(name, message);
            }
        };

        chatHub.client.broadcastSystemMessage = (message: string, tableId: number, timeStamp: number) => {
            // will autoconvert string to int
            // ReSharper disable once CoercedEqualsUsing
            if (tableId == this.tableId) {
                this.systemMessages[timeStamp] = message;
                let lastMessage = this.getLastMessage();
                if (lastMessage !== "") {
                    systemCallback(message);
                }
            }
        };

        chatHub.client.broadcastStateMessage = (message: string, tableId: number) => {
            // will autoconvert string to int
            // ReSharper disable once CoercedEqualsUsing
            if (tableId == this.tableId) {
                    stateCallback(message);
            }
        };
        
        ChatService.instance = this;
    }

    public getLastMessage(): string {
        var latestTS: number = 0;
        //this.systemMessages.
        for (let key in this.systemMessages) {
            let keyInt = parseInt(key);
            if (this.systemMessages.hasOwnProperty(key) && keyInt !== NaN && latestTS < keyInt) {
                latestTS = keyInt;
            }
        }
        if (latestTS !== 0) {
            return this.systemMessages[latestTS];
        } else {
            return "";
        }
    }

    public static getInstance(tableId: number, callback: (name: string, message: string) => any, systemCallback: (message: string) => any, stateCallback: (message: string) => any ) {
        // Create new instance if callback is given
        if (ChatService.instance === null && callback !== null || tableId !== ChatService.instance.tableId) {
            ChatService.instance = new ChatService(tableId, callback, systemCallback, stateCallback);
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
