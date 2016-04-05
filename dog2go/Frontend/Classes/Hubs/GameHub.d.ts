interface IGameHub extends HubProxy {
    client: IGameHubClient;
    server: IGameHubServer;
}

interface IGameHubClient {
    createGameTable(areas: PlayerFieldArea[]);
    doSomeShit();
}

interface IGameHubServer {
    sendGameTable(): void;
}