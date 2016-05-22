////<reference path="../../Library/JQuery/jqueryui.d.ts"/>
import gfs = require("../Services/GameFieldsService");
import GameFieldService = gfs.GameFieldService;
import rs = require("../Services/RoundService");
import RoundService = rs.RoundService;
import ts = require("../Services/TurnService");
import TurnService = ts.TurnService;
import mc = require("./MeepleController");
import MeepleController = mc.MeepleController;

export class CardsController {
    private gameFieldService: GameFieldService;
    private turnService: TurnService;
    private roundService: RoundService;
    private myCards: ICard[];
    private selctedCard: ICard = null;
    private meepleController: MeepleController;

    constructor(tableId: number, meepleController: MeepleController) {
        this.meepleController = meepleController;

        this.gameFieldService = GameFieldService.getInstance(tableId);
        this.gameFieldService.assignHandCardsCb = this.showHandCards.bind(this);
        this.roundService = RoundService.getInstance(tableId);
        this.roundService.assignHandCardsCb = this.showHandCards.bind(this);
        this.turnService = TurnService.getInstance(tableId);
        this.turnService.notifyActualPlayerCardsCb = this.notifyActualPlayer.bind(this);

        this.turnService.dropCardsCb = this.dropAllCards.bind(this);
    }

    public showHandCards(cards: ICard[]) {
        this.myCards = cards;

        if (typeof cards !== "undefined" && cards !== null) {
            for (let i = 0; i < cards.length; i++) {
                this.addCard(cards[i]);
                this.setDragableOnCard(cards[i]);
                this.disableDrag(cards[i]);
            }
        }
        this.makeGamefieldDroppable();
    }

    public notifyActualPlayer(possibleCards: IHandCard[], meepleColor: number) {
        this.dropAllCards();
        this.showHandCards(possibleCards);
        for (let card of possibleCards) {
            if (card.IsValid) {
                this.enableDrag(card);
            }
        }
    }

    public addCard(card: ICard) {
        const container = $("#cardContainer");
        container.append(`<img class="handcards ${card.Name}" id="${card.Name}" src="/Frontend/Images/cards-min/${card.ImageIdentifier}" ></img>`);
    }

    public makeGamefieldDroppable() {
        var self = this;
        $("#gameContent > canvas").droppable({
            accept: function (d) {
                if (d.hasClass("handcards")) {
                    return true;
                }
            },
            drop: function (event, ui) {
                self.centerCard(ui.draggable, $(this));
                const id = ui.draggable.attr("id");
                self.handleDroppedCard(id);
            }
        });
    }

    public centerCard(drag, drop) {
        const dropP = drop.offset();
        const dragP = drag.offset();
        const leftEnd = dropP.left + drop.width() / 2 - dragP.left - drag.width() / 2 + 1;
        const topEnd = dropP.top + drop.height() / 2 - dragP.top - drag.height() / 2 + 1;
        drag.animate({
            top: `+=${topEnd}`,
            left: `+=${leftEnd}`
        });
    }

    public handleDroppedCard(id: string) {
        const card: ICard = this.getFirstCardsByName(id);
        const cardMove: ICardMove = { Card: card, SelectedAttribute: null };
        this.selctedCard = card;
        this.disableAllDrag();
        this.meepleController.proceedMeepleTurn(cardMove);
    }

    public dropAllCards() {
        $(".handcards").remove();
    }

    public disableAllDrag() {
        $(`.handcards`).draggable("disable");
    }

    public disableDrag(card: ICard) {
        $(`.handcards.${card.Name}`).draggable("disable");
    }

    public enableDrag(card: ICard) {
        $(`.handcards.${card.Name}`).draggable("enable");
    }

    public setDragableOnCard(card: ICard) {
        // HowTo draggable: http://stackoverflow.com/questions/5735270/revert-a-jquery-draggable-object-back-to-its-original-container-on-out-event-of
        $(`.handcards.${card.Name}`).draggable({
            revert: function (event) {
                $(this).data("ui-draggable").originalPosition = {
                    top: 0,
                    left: 0
                };
                return !event;
            }
        });
    }

    private getFirstCardsByName(name: string): ICard {
        for (let card of this.myCards) {
            if (name === card.Name) {
                return card;
            }
        }
        return null;
    }
}