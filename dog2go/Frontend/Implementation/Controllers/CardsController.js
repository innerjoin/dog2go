define(["require", "exports", "../Services/GameFieldsService", "../Services/RoundService", "../Services/TurnService"], function (require, exports, gfs, rs, ts) {
    "use strict";
    var GameFieldService = gfs.GameFieldService;
    var RoundService = rs.RoundService;
    var TurnService = ts.TurnService;
    var CardsController = (function () {
        function CardsController(tableId, meepleController) {
            this.selctedCard = null;
            this.meepleController = meepleController;
            this.gameFieldService = GameFieldService.getInstance(tableId);
            this.gameFieldService.assignHandCardsCb = this.showHandCards.bind(this);
            this.roundService = RoundService.getInstance(tableId);
            this.roundService.assignHandCardsCb = this.showHandCards.bind(this);
            this.turnService = TurnService.getInstance(tableId);
            this.turnService.notifyActualPlayerCardsCb = this.notifyActualPlayer.bind(this);
            this.turnService.dropCardsCb = this.dropAllCards.bind(this);
        }
        CardsController.prototype.showHandCards = function (cards) {
            this.myCards = cards;
            if (typeof cards !== "undefined" && cards !== null) {
                for (var i = 0; i < cards.length; i++) {
                    this.addCard(cards[i]);
                    this.setDragableOnCard(cards[i]);
                    this.disableDrag(cards[i]);
                }
            }
            this.makeGamefieldDroppable();
        };
        CardsController.prototype.notifyActualPlayer = function (possibleCards, meepleColor) {
            this.dropAllCards();
            this.showHandCards(possibleCards);
            for (var _i = 0, possibleCards_1 = possibleCards; _i < possibleCards_1.length; _i++) {
                var card = possibleCards_1[_i];
                if (card.IsValid) {
                    this.enableDrag(card);
                }
            }
        };
        CardsController.prototype.addCard = function (card) {
            var container = $("#cardContainer");
            container.append("<img class=\"handcards " + card.Name + "\" id=\"" + card.Name + "\" src=\"/Frontend/Images/cards-min/" + card.ImageIdentifier + "\" ></img>");
        };
        CardsController.prototype.makeGamefieldDroppable = function () {
            var self = this;
            $("#gameContent > canvas").droppable({
                accept: function (d) {
                    if (d.hasClass("handcards")) {
                        return true;
                    }
                },
                drop: function (event, ui) {
                    self.centerCard(ui.draggable, $(this));
                    var id = ui.draggable.attr("id");
                    self.handleDroppedCard(id);
                }
            });
        };
        CardsController.prototype.centerCard = function (drag, drop) {
            var dropP = drop.offset();
            var dragP = drag.offset();
            var leftEnd = dropP.left + drop.width() / 2 - dragP.left - drag.width() / 2 + 1;
            var topEnd = dropP.top + drop.height() / 2 - dragP.top - drag.height() / 2 + 1;
            drag.animate({
                top: "+=" + topEnd,
                left: "+=" + leftEnd
            });
        };
        CardsController.prototype.handleDroppedCard = function (id) {
            var card = this.getFirstCardsByName(id);
            var cardMove = { Card: card, SelectedAttribute: null };
            this.selctedCard = card;
            this.disableAllDrag();
            this.meepleController.proceedMeepleTurn(cardMove);
        };
        CardsController.prototype.dropAllCards = function () {
            $(".handcards").remove();
        };
        CardsController.prototype.disableAllDrag = function () {
            $(".handcards").draggable("disable");
        };
        CardsController.prototype.disableDrag = function (card) {
            $(".handcards." + card.Name).draggable("disable");
        };
        CardsController.prototype.enableDrag = function (card) {
            $(".handcards." + card.Name).draggable("enable");
        };
        CardsController.prototype.setDragableOnCard = function (card) {
            // HowTo draggable: http://stackoverflow.com/questions/5735270/revert-a-jquery-draggable-object-back-to-its-original-container-on-out-event-of
            $(".handcards." + card.Name).draggable({
                revert: function (event) {
                    $(this).data("ui-draggable").originalPosition = {
                        top: 0,
                        left: 0
                    };
                    return !event;
                }
            });
        };
        CardsController.prototype.getFirstCardsByName = function (name) {
            for (var _i = 0, _a = this.myCards; _i < _a.length; _i++) {
                var card = _a[_i];
                if (name === card.Name) {
                    return card;
                }
            }
            return null;
        };
        return CardsController;
    }());
    exports.CardsController = CardsController;
});
//# sourceMappingURL=CardsController.js.map