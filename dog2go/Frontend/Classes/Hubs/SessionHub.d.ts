///<reference path="../Services/buildUpTypes.ts"/>

interface ISessionHub extends HubProxy {
    client: ISessionHubClient;
    server: ISessionHubServer;
}

interface ISessionHubClient {
    newSession(cookie: string);
    // TODO: Change to Type
    updtadeOpenGames(games: any);
}

interface ISessionHubServer {
    login(name: string, cookie: string);
    createGame();
    joinGame(gameNumber: number);
}