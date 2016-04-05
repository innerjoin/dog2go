﻿interface IChatHub extends HubProxy {
    client: IChatHubClient;
    server: IChatHubServer;
}


interface IChatHubClient {
    broadcastMessage(name:string, message:string);
    doSomeShit();
}

interface IChatHubServer {
    sendTo(name:string, message:string): void;
}