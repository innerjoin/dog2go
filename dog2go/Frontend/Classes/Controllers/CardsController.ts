﻿////<reference path="../../Library/JQuery/jqueryui.d.ts"/>

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

        this.makeGamefieldDroppable();
    }

    public showHandCards(cards: ICard[]) {
        this.myCards = cards;
        console.log("Show HandCards: ", cards);
        
        if (cards !== null) {
            for (let i = 0; i < cards.length; i++) {
                var container = $("#cardContainer");
                container.append(`<img class="handcards" id="${cards[i].Name}" src="/Frontend/Images/cards-min/${cards[i].ImageIdentifier}" ></img>`);
                this.makeCardDragable(cards[i]);
                
            }
        }
    }

    public notifyActualPlayer(possibleCards: IHandCard[], meepleColor: number) {
        for (var card of possibleCards) {
            //if (card.is)
            this.makeCardDragable(card);
        }
    }

    public makeGamefieldDroppable() {
        $("#gameContent > canvas").droppable({
            accept: ".handcards",
            drop: (event, ui) => {
                var id: string = ui.draggable.attr("id");
                var card: ICard = this.getFirstCardsByName(id);
                // TODO: Handle Atribute-Selection?
                var cardMove: ICardMove = { Card: card, SelectedAttribute: card.Attributes[0] };
                this.selctedCard = card;
                this.meepleController.proceedMeepleTurn(cardMove);
                console.log("verify the following card: ", id, card);
            }
        });
    }

    public makeCardDragable(card: ICard) {
        $(`#${card.Name}`).draggable({
            // keep this as a function, else the scope gets broken.
            revert: function (event, ui) {
                // on older version of jQuery use "draggable"
                // $(this).data("draggable")
                // 
                // on 1.11.x versions of jQuery use "uiDraggable"
                // $(this).data("uiDraggable")
                // on 2.x versions of jQuery use "ui-draggable"
                // $(this).data("ui-draggable")
                console.log($(this).data("ui-draggable"));
                $(this).data("ui-draggable").originalPosition = {
                    top: 0,
                    left: 0
                };
                // return boolean
                return !event;
                // that evaluate like this:
                // return event !== false ? false : true;
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