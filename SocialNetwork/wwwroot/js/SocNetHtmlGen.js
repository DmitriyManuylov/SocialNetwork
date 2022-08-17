var messageTextArea = document.querySelector("#messageInput textarea");
export function CreateUserListItem(user, onChatSelected, onMessageSend) {
    var listItemDiv = document.createElement("div");
    var butUser = document.createElement("button");
    var hiddenId = document.createElement("input");
    var userLink = document.createElement("a");
    var hiddenChatId = document.createElement("input");

    butUser.classList.add("w-100", "network-list-item");
    butUser.onclick = e => {
        onChatSelected(e, "ConnectToDialog", null, user.chatId);
        messageTextArea.onkeypress = e => onMessageSend(e, "SendMessageToInterlocutor", user.chatId);
    }
    
    hiddenId.type = "hidden";
    hiddenId.value = user.id;

    hiddenChatId.type = "hidden";
    hiddenChatId.value = user.chatId;

    userLink.href = user.userPageLink;
    userLink.innerText = user.userName;

    butUser.appendChild(userLink);
    listItemDiv.appendChild(butUser);
    listItemDiv.appendChild(hiddenChatId);
    listItemDiv.appendChild(hiddenId);
    listItemDiv.classList.add("w-100");
    return listItemDiv;
}

export function CreateGroupChatListItem(chat, onChatSelected, onMessageSend) {
    var listItemDiv = document.createElement("div");
    var butUser = document.createElement("button");
    var hiddenId = document.createElement("input");


    butUser.classList.add("w-100", "network-list-item");
    butUser.innerText = chat.name;
    butUser.onclick = e => {
        onChatSelected(e, "ConnectToChat", "DisconnectFromChat", chat.id);
        messageTextArea.onkeypress = e => onMessageSend(e, "SendMessage",chat.id);
    };

    hiddenId.type = "hidden";
    hiddenId.value = chat.id.toString();

    listItemDiv.appendChild(butUser);
    listItemDiv.appendChild(hiddenId);
    listItemDiv.classList.add("w-100");
    return listItemDiv;
}

export function CreateMessageItem(message) {
    var messageDiv = document.createElement("div");
    var headerDiv = document.createElement("div");
    var textDiv = document.createElement("div");
    var userLink = document.createElement("a");
    var dateTimeDiv = document.createElement("div");

    messageDiv.classList.add("message-box");

    userLink.href = message.senderLink;
    userLink.innerText = message.senderName;
    dateTimeDiv.innerText = message.dateTime;

    headerDiv.classList.add("message-box-header");
    headerDiv.appendChild(userLink);
    headerDiv.appendChild(dateTimeDiv);

    textDiv.innerText = message.text;
    textDiv.classList.add("message-box-item");

    messageDiv.appendChild(headerDiv);
    messageDiv.appendChild(textDiv);
    return messageDiv;
}