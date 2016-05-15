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


    constructor(meepleController: MeepleController) {
        this.meepleController = meepleController;

        this.gameFieldService = GameFieldService.getInstance();
        this.gameFieldService.assignHandCardsCB = this.showHandCards.bind(this);
        this.roundService = RoundService.getInstance();
        this.roundService.assignHandCardsCB = this.showHandCards.bind(this);
        this.turnService = TurnService.getInstance();
        this.turnService.notifyActualPlayerCardsCB = this.notifyActualPlayer.bind(this);

    }

    public showHandCards(cards: ICard[]) {
        this.myCards = cards;
        console.log("Show HandCards: ", cards);
        
        if (cards !== null) {
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
        for (var card of possibleCards) {
            if (card.IsValid) {
                this.enableDrag(card);
            }
        }
    }

    public addCard(card: ICard) {
        var container = $("#cardContainer");
        container.append(`<img class="handcards ${card.Name}" id="${card.Name}" src="/Frontend/Images/cards-min/${card.ImageIdentifier}" ></img>`);
    }

    public makeGamefieldDroppable() {
        var self = this;
        $("#gameContent > canvas").droppable({
            accept: function(d) {
                if (d.hasClass("handcards")) {
                    return true;
                }
            },
            drop: function (event, ui) {
                self.centerCard(ui.draggable, $(this));

                var id: string = ui.draggable.attr("id");
                console.log("Context: ", $(this), ui, ui.draggable.context);

                self.handleDroppedCard(id);
            }
        });
    }

    public centerCard(drag, drop) {
        var drop_p = drop.offset();
        var drag_p = drag.offset();
        var left_end = drop_p.left + drop.width() / 2 - drag_p.left - drag.width() / 2 + 1;
        var top_end = drop_p.top + drop.height() / 2 - drag_p.top - drag.height() / 2 + 1;
        drag.animate({
            top: '+=' + top_end,
            left: '+=' + left_end
        });
    }

    public handleDroppedCard(id: string) {
        var card: ICard = this.getFirstCardsByName(id);
        var cardMove: ICardMove = { Card: card, SelectedAttribute: null };
        this.selctedCard = card;
        this.disableAllDrag();
        this.meepleController.proceedMeepleTurn(cardMove);
    }

    public dropAllCards() {
        $(".handcards").remove();
    }

    public disableAllDrag() {
        $(`.handcards`).draggable('disable');
    }

    public disableDrag(card: ICard) {
        $(`.handcards.${card.Name}`).draggable('disable');
    }

    public enableDrag(card: ICard) {
        $(`.handcards.${card.Name}`).draggable('enable');
    }

    public setDragableOnCard(card: ICard) {
        // HowTo draggable: http://stackoverflow.com/questions/5735270/revert-a-jquery-draggable-object-back-to-its-original-container-on-out-event-of
        $(`.handcards.${card.Name}`).draggable({
            revert: function (event, ui) {
                $(this).data("ui-draggable").originalPosition = {
                    top: 0,
                    left: 0
                };
                return !event;
            }
        });
    }

    private getFirstCardsByName(name: string) :ICard {
        for (var card of this.myCards) {
            if (name === card.Name) {
                return card;
            }
        }
        return null;
    }
}