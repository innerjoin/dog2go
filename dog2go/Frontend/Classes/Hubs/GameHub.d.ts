///<reference path="../Services/buildUpTypes.ts"/>
///<reference path="../Model/TableModel.ts"/>

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
    assignHandCards(cards: Card[]);
}

interface IGameHubServer {
    connectToTable(): IGameTable;
}