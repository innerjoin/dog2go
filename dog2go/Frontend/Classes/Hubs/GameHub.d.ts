
interface HubProxy{}

interface IGameHub extends HubProxy {
    client: IGameHubClient;
    server: IGameHubServer;
    qs: string;
}

interface IGameHubClient {
    createGameTable(gameTable: IGameTable);
    
    backToGame(table: IGameTable, cards: ICard[]);
    assignHandCards(cards: ICard[]);

    notifyActualPlayer(possibleCards: IHandCard[], meepleColor: number);
    sendMeeplePositions(meeples: IMeeple[]);
    dropCards();
    returnMove();
}

interface IGameHubServer {
    connectToTable(gameTableId: number): IGameTable;
    validateMove(meepleMove: IMeepleMove, cardMove: ICardMove): boolean;
}
