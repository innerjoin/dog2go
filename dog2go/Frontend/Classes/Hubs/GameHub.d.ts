
interface HubProxy{}

interface IGameHub extends HubProxy {
    client: IGameHubClient;
    server: IGameHubServer;
}

interface IGameHubClient {
    createGameTable(gameTable: IGameTable);
    
    backToGame(table: IGameTable, cards: ICard[]);
    assignHandCards(cards: ICard[]);

    notifyActualPlayer(possibleCards: ICard[]);
    sendMeeplePositions(meeples: IMeeple[]);
    dropCards();
}

interface IGameHubServer {
    connectToTable(): IGameTable;
    ValidateMove(meepleMove: IMeepleMove, cardMove: ICardMove): boolean;
}
