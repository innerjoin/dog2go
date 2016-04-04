interface SignalR {
    gameHub: HubProxy;
}

interface HubProxy {
    client: IGameHubClient;
    server: IGameHubServer;
}
