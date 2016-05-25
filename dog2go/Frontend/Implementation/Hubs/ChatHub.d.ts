interface IChatHub extends HubProxy {
    client: IChatHubClient;
    server: IChatHubServer;
    qs: string;
}

interface IChatHubClient {
    broadcastMessage(name: string, message: string, tableId: number);
    broadcastSystemMessage(message: string, tableId: number, timeStamp: number);
}

interface IChatHubServer {
    sendMessage(message: string, tableId: number): void;
}