///<reference path="../../Library/JQuery/jqueryui.d.ts"/>
import BuildUpTypes = require("buildUpTypes");
import PlayerFieldArea = BuildUpTypes.PlayerFieldArea;

export class GameFieldService {
    private static instance: GameFieldService = null;
    constructor(callback: (ev: IGameTable) => any) {
        if (GameFieldService.instance) {
            // ReSharper disable once TsNotResolved
            throw new Error("Error: GameFieldService instantiation failed. Singleton module! Use .getInstance() instead of new.");
        }
        var gameHub = $.connection.gameHub;
        gameHub.client.createGameTable = (areas) => {
            callback(areas);
        }
        gameHub.client.assignHandCards = (cards: Card[]) => {
            this.showHandCards(cards);
        }

        var tryingToReconnect = false;

        $.connection.hub.reconnecting(() => {
            console.log("reconnecting...");
            tryingToReconnect = true;
        });

        $.connection.hub.reconnected(() => {
            console.log("reconnected...");
            tryingToReconnect = false;
        });

        $.connection.hub.connectionSlow(() => {
            console.log("connection slow...");
        });

        $.connection.hub.disconnected(() => {
            console.log("disconnected...");
            if (tryingToReconnect) {
                if ($.connection.hub.lastError)
                    alert(`Disconnected. Reason: ${$.connection.hub.lastError.message}`);
            }
        });

        GameFieldService.instance = this;
    }

    public showHandCards(cards: Card[]) {
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

    public static getInstance(callback: (ev: IGameTable) => any) {
        // Create new instance if callback is given
        if (GameFieldService.instance === null && callback !== null) {
            GameFieldService.instance = new GameFieldService(callback);
        } else if (GameFieldService.instance === null){
            // ReSharper disable once TsNotResolved
            throw new Error("Error: First call needs a callback!");
        }
        return GameFieldService.instance;
    }

    public getGameFieldData():void {
        var gameHub = $.connection.gameHub;
        $.connection.hub.start().done(() => {
            gameHub.server.connectToTable();
        });
    }
}

