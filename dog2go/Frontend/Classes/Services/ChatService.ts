
import ChatModel = require("../Model/ChatModel");

export class ChatService {
    private static instance: ChatService = null;
    constructor(callback: (name: string, message: string) => any, systemCallback: (message: string) => any) {
        if (ChatService.instance) {
            throw new Error("Error: ChatService instantiation failed. Singleton module! Use .getInstance() instead of new.");
        }
        var chatHub = $.connection.chatHub;

        chatHub.client.broadcastMessage = function (name: string, message: string) {
            callback(name, message);
        };

        chatHub.client.broadcastSystemMessage = function (message: string) {
            systemCallback(message);
        };
        
        ChatService.instance = this;
    }

    public static getInstance(callback: (name: string, message: string) => any, systemCallback: (message: string) => any) {
        // Create new instance if callback is given
        if (ChatService.instance === null && callback !== null) {
            ChatService.instance = new ChatService(callback, systemCallback);
        } else if (ChatService.instance === null) {
            throw new Error("Error: First call needs a callback!");
        }
        return ChatService.instance;
    }

    public sendMessage(message: string): void {
        var chatHub = $.connection.chatHub;
        $.connection.hub.start().done(() => {
            chatHub.server.sendMessage(message);
        });
    }
}
