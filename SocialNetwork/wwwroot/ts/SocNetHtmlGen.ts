import { User, Message, Chat } from "../ts/ViewModels"

export function CreateUserListItem(user: User, onChatSelected): HTMLDivElement {
    var listItemDiv: HTMLDivElement = document.createElement("div");
    var butUser: HTMLButtonElement = document.createElement("button");
    var hiddenId: HTMLInputElement = document.createElement("input");
    var userLink: HTMLAnchorElement = document.createElement("a");

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

export function CreateGroupChatListItem(chat: Chat, onChatSelected): HTMLDivElement {
    var listItemDiv: HTMLDivElement = document.createElement("div");
    var butUser: HTMLButtonElement = document.createElement("button");
    var hiddenId: HTMLInputElement = document.createElement("input");


    butUser.classList.add("w-100", "network-list-item");
    butUser.onclick(onChatSelected);

    hiddenId.type = "hidden";
    hiddenId.value = chat.Id.toString();

    listItemDiv.appendChild(butUser);
    listItemDiv.appendChild(hiddenId);
    listItemDiv.classList.add("w-100");
    return listItemDiv;
}

export function CreateMessageItem(message: Message): HTMLDivElement {
    var messageDiv: HTMLDivElement = document.createElement("div");
    var headerDiv: HTMLDivElement = document.createElement("div");
    var textDiv: HTMLDivElement = document.createElement("div");
    var userLink: HTMLAnchorElement = document.createElement("a");
    var dateTimeDiv: HTMLDivElement = document.createElement("div");

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
