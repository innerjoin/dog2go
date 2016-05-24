define(["require", "exports", "../Services/ChatService"], function (require, exports, Service) {
    "use strict";
    var ChatService = Service.ChatService;
    var ChatController = (function () {
        function ChatController(tableId) {
            var _this = this;
            this.chatService = ChatService.getInstance(tableId, this.putMessage.bind(this), this.putSystemMessage.bind(this));
            $("#message").keypress(function (e) {
                if (e.which === 13)
                    _this.sendChat(tableId);
            });
            $("#sendmessage").click(function () {
                _this.sendChat(tableId);
            });
            $("#gameContent").focus();
        }
        ChatController.prototype.sendChat = function (tableId) {
            this.chatService.sendMessage($("#message").val(), tableId);
            $("#message").val("");
            $("#gameContent").focus();
        };
        ChatController.prototype.putMessage = function (name, message) {
            var encodedName = $("<div />").text(name).html();
            var encodedMsg = $("<div />").text(message).html();
            // Add the message to the page. 
            $("#chatBox").append("<li><strong>" + encodedName + "</strong>:&nbsp;&nbsp;" + encodedMsg + "</li>");
            $('#chatBox').animate({ scrollTop: $(document).height() }, "slow");
        };
        ChatController.prototype.putSystemMessage = function (message) {
            var encodedMsg = $("<div />").text(message).html();
            // Add the message to the page. 
            $("#chatBox").append("<li><strong>" + encodedMsg + "</strong></li>");
            $('#chatBox').animate({ scrollTop: $(document).height() }, "slow");
        };
        return ChatController;
    }());
    exports.ChatController = ChatController;
});
//# sourceMappingURL=ChatController.js.map