"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.CreateMessageItem = exports.CreateGroupChatListItem = exports.CreateUserListItem = void 0;
function CreateUserListItem(user, onChatSelected) {
    var listItemDiv = document.createElement("div");
    var butUser = document.createElement("button");
    var hiddenId = document.createElement("input");
    var userLink = document.createElement("a");
    butUser.classList.add("w-100", "network-list-item");
    butUser.onclick(onChatSelected);
    hiddenId.type = "hidden";
    hiddenId.value = user.Id;
    userLink.href = user.UserPageLink;
    userLink.innerText = user.UserName;
    butUser.appendChild(userLink);
    listItemDiv.appendChild(butUser);
    listItemDiv.appendChild(hiddenId);
    listItemDiv.classList.add("w-100");
    return listItemDiv;
}
exports.CreateUserListItem = CreateUserListItem;
function CreateGroupChatListItem(chat, onChatSelected) {
    var listItemDiv = document.createElement("div");
    var butUser = document.createElement("button");
    var hiddenId = document.createElement("input");
    butUser.classList.add("w-100", "network-list-item");
    butUser.onclick(onChatSelected);
    hiddenId.type = "hidden";
    hiddenId.value = chat.Id.toString();
    listItemDiv.appendChild(butUser);
    listItemDiv.appendChild(hiddenId);
    listItemDiv.classList.add("w-100");
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
    userLink.href = message.SenderLink;
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
//# sourceMappingURL=SocNetHtmlGen.js.map