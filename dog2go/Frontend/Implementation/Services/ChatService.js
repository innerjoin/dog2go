define(["require", "exports"], function (require, exports) {
    "use strict";
    var ChatService = (function () {
        function ChatService(tableId, callback, systemCallback) {
            var _this = this;
            this.tableId = tableId;
            if (ChatService.instance) {
                throw new Error("Error: ChatService instantiation failed. Singleton module! Use .getInstance() instead of new.");
            }
            var chatHub = $.connection.chatHub;
            $.connection.hub.qs = "tableId=" + tableId;
            chatHub.client.broadcastMessage = function (name, message, tableId) {
                // will autoconvert string to int
                // ReSharper disable once CoercedEqualsUsing
                if (tableId == _this.tableId) {
                    callback(name, message);
                }
            };
            chatHub.client.broadcastSystemMessage = function (message, tableId) {
                // will autoconvert string to int
                // ReSharper disable once CoercedEqualsUsing
                if (tableId == _this.tableId) {
                    systemCallback(message);
                }
            };
            ChatService.instance = this;
        }
        ChatService.getInstance = function (tableId, callback, systemCallback) {
            // Create new instance if callback is given
            if (ChatService.instance === null && callback !== null || tableId !== ChatService.instance.tableId) {
                ChatService.instance = new ChatService(tableId, callback, systemCallback);
            }
            else if (ChatService.instance === null) {
                throw new Error("Error: First call needs a callback!");
            }
            return ChatService.instance;
        };
        ChatService.prototype.sendMessage = function (message, tableId) {
            var chatHub = $.connection.chatHub;
            $.connection.hub.start().done(function () {
                chatHub.server.sendMessage(message, tableId);
            });
        };
        ChatService.instance = null;
        return ChatService;
    }());
    exports.ChatService = ChatService;
});
//# sourceMappingURL=ChatService.js.map