interface IChatHub extends HubProxy {
    client: IChatHubClient;
    server: IChatHubServer;
}

interface IChatHubClient {
    broadcastMessage(name:string, message: IMessage);
    doSomeShit();
}

interface IChatHubServer {
    sendMessage(message: IMessage): void;
}