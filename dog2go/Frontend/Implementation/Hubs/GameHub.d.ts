
interface HubProxy{}

interface IGameHub extends HubProxy {
    client: IGameHubClient;
    server: IGameHubServer;
    qs: string;
}

interface IGameHubClient {
    createGameTable(gameTable: IGameTable, tableId: number);
    
    backToGame(table: IGameTable, cards: ICard[], tableId: number);
    assignHandCards(cards: ICard[], tableId: number);

    notifyActualPlayer(possibleCards: IHandCard[], meepleColor: number, tableId: number);
    sendMeeplePositions(meeples: IMeeple[], tableId: number);
    dropCards(tableId: number);
    returnMove(tableId: number);
}

interface IGameHubServer {
    connectToTable(gameTableId: number): IGameTable;
    validateMove(meepleMove: IMeepleMove, cardMove: ICardMove, tableId: number): boolean;
}
