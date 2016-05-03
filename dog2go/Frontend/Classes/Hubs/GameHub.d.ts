
interface HubProxy{}

interface IGameHub extends HubProxy {
    client: IGameHubClient;
    server: IGameHubServer;
}

interface IGameHubClient {
    createGameTable(gameTable: IGameTable);
    
    backToGame(table: IGameTable, cards: ICard[]);
    assignHandCards(cards: ICard[]);

    notifyActualPlayer(possibleCards: ICard[], meepleColor: number);
    sendMeeplePositions(meeples: IMeeple[]);
    dropCards();
}

interface IGameHubServer {
    connectToTable(): IGameTable;
    validateMove(meepleMove: IMeepleMove, cardMove: ICardMove): boolean;
}
