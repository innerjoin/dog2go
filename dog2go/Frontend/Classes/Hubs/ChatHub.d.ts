interface IChatHub extends HubProxy {
    client: IChatHubClient;
    server: IChatHubServer;
}


interface IChatHubClient {
    broadcastMessage(name: string, message: string);
    broadcastSystemMessage(message: string);
    doSomeShit();
}

interface IChatHubServer {
    sendMessage(message: string): void;
}