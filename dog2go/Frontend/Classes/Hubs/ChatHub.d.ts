interface IChatHub extends HubProxy {
    client: IChatHubClient;
    server: IChatHubServer;
    qs: string;
}

interface IChatHubClient {
    broadcastMessage(name: string, message: string);
    broadcastSystemMessage(message: string);
}

interface IChatHubServer {
    assignChatToTable(tableId: number): void;
    sendMessage(message: string, tableId: number): void;
}