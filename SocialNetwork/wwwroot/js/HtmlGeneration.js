export function CreateMessageItem(messageId, senderName, messageText, dateTime) {
    var messageItemDiv = document.createElement("div");
    messageItemDiv.setAttribute("id", "message" + messageId)
    messageItemDiv.classList.add("message-box");


    var metaDiv = document.createElement("div");
    metaDiv.classList.add("message-box-header");


    var nameElement = document.createElement("div");
    nameElement.textContent = senderName;


    var date = document.createElement("div");
    date.textContent = dateTime;

    var messageTextNode = document.createElement("div");
    messageTextNode.classList.add("message-box-item");
    messageTextNode.innerText = messageText;
    metaDiv.appendChild(nameElement);
    metaDiv.appendChild(date);

    var messagesArea = document.getElementById("messagesArea");
    messageItemDiv.appendChild(metaDiv);
    messageItemDiv.appendChild(messageTextNode);

    messagesArea.appendChild(messageItemDiv);
    messagesArea.parentElement.scrollTop = messagesArea.parentElement.scrollHeight;
}

export function AddRoomToList(room, onRoomSelect) {
    var divWrap = document.createElement("div");
    divWrap.classList.add("w-100");

    var button = document.createElement("button");
    button.innerText = room.name;
    button.classList.add("w-100", "network-list-item");
    button.setAttribute("name", "roomId");
    button.setAttribute("value", room.id);
    divWrap.appendChild(button);

    var hidden = document.createElement("input");
    hidden.setAttribute("type", "hidden");
    hidden.setAttribute("value", room.id);

    divWrap.appendChild(hidden);
    roomsList.appendChild(divWrap);
    button.addEventListener("click", onRoomSelect);
}