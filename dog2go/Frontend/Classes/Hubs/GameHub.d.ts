///<reference path="../Services/buildUpTypes.ts"/>
interface HubProxy{}

interface IGameHub extends HubProxy {
    client: IGameHubClient;
    server: IGameHubServer;
}

interface IGameHubClient {
    createGameTable?(areas: any); // TODO: implement Class

    newSession?(cookie: string);
    updateOpenGames?(gameTable: IGameTable[]);
    backToGame?(table: IGameTable, cards: Card[]);
}

interface IGameHubServer {
    sendGameTable?(): void;

    login?(name: string, cookie: string);
}