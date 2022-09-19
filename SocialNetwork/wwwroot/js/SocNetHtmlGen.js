export class UserBut extends HTMLButtonElement {
}
export class ChatBut extends HTMLButtonElement {
}
var messageTextArea = document.querySelector("#messageInput textarea");
export function CreateUserListItem(user, onChatSelected, onMessageSend) {
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
    let url = new URL(user.UserPageLink, location.origin);
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
    userBut.onclick = e => {
        onChatSelected(e);
        messageTextArea.onkeypress = e => onMessageSend(e, "SendMessageToInterlocutor");
    };
    return listItemDiv;
}
export function CreateGroupChatListItem(chat, onChatSelected, onMessageSend) {
    var listItemDiv = document.createElement("div");
    var butChat = document.createElement("button");
    var hiddenId = document.createElement("input");
    var chatLink = document.createElement("a");
    butChat.classList.add("w-100", "network-list-item");
    butChat.type = "button";
    hiddenId.type = "hidden";
    hiddenId.value = chat.Id.toString();
    butChat.setAttribute("id", "chat" + chat.Id);
    let url = new URL(chat.ChatLink, location.origin);
    chatLink.href = url.href;
    chatLink.innerText = chat.Name;
    butChat.appendChild(chatLink);
    listItemDiv.appendChild(butChat);
    listItemDiv.appendChild(hiddenId);
    listItemDiv.classList.add("w-100");
    var chatBut = butChat;
    chatBut.Chat = chat;
    chatBut.onclick = e => {
        onChatSelected(e);
        messageTextArea.onkeypress = ev => onMessageSend(ev, "SendMessage");
    };
    return listItemDiv;
}
export function CreateMessageItem(message) {
    var messageDiv = document.createElement("div");
    var headerDiv = document.createElement("div");
    var textDiv = document.createElement("div");
    var userLink = document.createElement("a");
    var dateTimeDiv = document.createElement("div");
    messageDiv.classList.add("message-box");
    let url = new URL(message.SenderLink, location.origin);
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
//# sourceMappingURL=SocNetHtmlGen.js.map