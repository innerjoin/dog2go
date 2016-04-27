///<reference path="../../Library/JQuery/jqueryui.d.ts"/>

import gfs = require("../Services/GameFieldsService");
import GameFieldService = gfs.GameFieldService;

export class GameFieldController {
    private gameFieldService: GameFieldService;
    constructor() {
        this.gameFieldService = GameFieldService.getInstance();
        this.gameFieldService.assignHandCardsCB = this.showHandCards;
    }

    public showHandCards(cards: ICard[]) {
        const container = $("#cardContainer");
        $("#gameContent > canvas").droppable({
            accept: ".handcards",
            drop: (event, ui) => {
                console.log("drop it");
                var id = ui.draggable.attr("id");
                console.log("verify the following card: ", id);
            }
        });
        for (let i = 0; i < cards.length; i++) {
            container.append(`<img class="handcards" id="${cards[i].Name}" src="/Frontend/Images/cards-min/${cards[i].ImageIdentifier}" ></img>`);
            $(`#${cards[i].Name}`).draggable({
                revert: function (event, ui) {
                    // on older version of jQuery use "draggable"
                    // $(this).data("draggable")
                    // 
                    // on 1.11.x versions of jQuery use "uiDraggable"
                    // $(this).data("uiDraggable")
                    // on 2.x versions of jQuery use "ui-draggable"
                    // $(this).data("ui-draggable")
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
    }


}