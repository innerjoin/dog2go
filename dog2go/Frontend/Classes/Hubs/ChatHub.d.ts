interface IGameHubClient {
    createGameTable(areas: PlayerFieldArea[]);
    doSomeShit();
}

interface IGameHubServer {
    sendGameTable(): void;
}