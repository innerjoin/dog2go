﻿///<reference path="../Services/buildUpTypes.ts"/>

interface IGameHub extends HubProxy {
    client: IGameHubClient;
    server: IGameHubServer;
}

interface IGameHubClient {
    createGameTable(areas: any);
    // TODO: revert this
    //createGameTable(areas: PlayerFieldArea[]);
}

interface IGameHubServer {
    sendGameTable(): void;
}