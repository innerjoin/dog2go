
interface ISessionHub extends HubProxy {
    client: ISessionHubClient;
    server: ISessionHubServer;
}

interface ISessionHubClient {
    newSession(cookie: string);
    // TODO: Change to Type
    updateOpenGames(games: any);
}

interface ISessionHubServer {
    login(name: string, cookie: string);
    createGame();
    joinGame(gameNumber: number);
}