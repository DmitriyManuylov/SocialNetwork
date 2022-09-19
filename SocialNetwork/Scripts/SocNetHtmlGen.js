"use strict";
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (Object.prototype.hasOwnProperty.call(b, p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        if (typeof b !== "function" && b !== null)
            throw new TypeError("Class extends value " + String(b) + " is not a constructor or null");
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
exports.__esModule = true;
exports.CreateMessageItem = exports.CreateGroupChatListItem = exports.CreateUserListItem = exports.ChatBut = exports.UserBut = void 0;
var UserBut = /** @class */ (function (_super) {
    __extends(UserBut, _super);
    function UserBut() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    return UserBut;
}(HTMLButtonElement));
exports.UserBut = UserBut;
var ChatBut = /** @class */ (function (_super) {
    __extends(ChatBut, _super);
    function ChatBut() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    return ChatBut;
}(HTMLButtonElement));
exports.ChatBut = ChatBut;
var messageTextArea = document.querySelector("#messageInput textarea");
function CreateUserListItem(user, onChatSelected, onMessageSend) {
    var listItemDiv = document.createElement("div");
    var butUser = document.createElement("button");
    var hiddenId = document.createElement("input");
    var userLink = document.createElement("a");
    var hiddenChatId = document.createElement("input");
    butUser.classList.add("w-100", "network-list-item");
    butUser.type = "button";
    hiddenId.type = "hidden";
    hiddenId.value = user.Id;
    hiddenChatId.type = "hidden";
    hiddenChatId.value = user.ChatId;
    var url = new URL(user.UserPageLink, location.origin);
    userLink.href = url.href;
    userLink.innerText = user.UserName;
    butUser.appendChild(userLink);
    butUser.setAttribute("id", "chat" + user.ChatId);
    var userBut = butUser;
    userBut.User = user;
    listItemDiv.appendChild(userBut);
    listItemDiv.appendChild(hiddenChatId);
    listItemDiv.appendChild(hiddenId);
    listItemDiv.classList.add("w-100");
    userBut.onclick = function (e) {
        onChatSelected(e);
        messageTextArea.onkeypress = function (e) { return onMessageSend(e, "SendMessageToInterlocutor"); };
    };
    return listItemDiv;
}
exports.CreateUserListItem = CreateUserListItem;
function CreateGroupChatListItem(chat, onChatSelected, onMessageSend) {
    var listItemDiv = document.createElement("div");
    var butChat = document.createElement("button");
    var hiddenId = document.createElement("input");
    var chatLink = document.createElement("a");
    butChat.classList.add("w-100", "network-list-item");
    butChat.type = "button";
    hiddenId.type = "hidden";
    hiddenId.value = chat.Id.toString();
    butChat.setAttribute("id", "chat" + chat.Id);
    var url = new URL(chat.ChatLink, location.origin);
    chatLink.href = url.href;
    chatLink.innerText = chat.Name;
    butChat.appendChild(chatLink);
    listItemDiv.appendChild(butChat);
    listItemDiv.appendChild(hiddenId);
    listItemDiv.classList.add("w-100");
    var chatBut = butChat;
    chatBut.Chat = chat;
    chatBut.onclick = function (e) {
        onChatSelected(e);
        messageTextArea.onkeypress = function (ev) { return onMessageSend(ev, "SendMessage"); };
    };
    return listItemDiv;
}
exports.CreateGroupChatListItem = CreateGroupChatListItem;
function CreateMessageItem(message) {
    var messageDiv = document.createElement("div");
    var headerDiv = document.createElement("div");
    var textDiv = document.createElement("div");
    var userLink = document.createElement("a");
    var dateTimeDiv = document.createElement("div");
    messageDiv.classList.add("message-box");
    var url = new URL(message.SenderLink, location.origin);
    userLink.href = url.href;
    userLink.innerText = message.SenderName;
    dateTimeDiv.innerText = message.DateTime;
    headerDiv.classList.add("message-box-header");
    headerDiv.appendChild(userLink);
    headerDiv.appendChild(dateTimeDiv);
    textDiv.innerText = message.Text;
    textDiv.classList.add("message-box-item");
    messageDiv.appendChild(headerDiv);
    messageDiv.appendChild(textDiv);
    return messageDiv;
}
exports.CreateMessageItem = CreateMessageItem;
