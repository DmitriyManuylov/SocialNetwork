import { Message, User } from "./TypedViewModels";

import { HubConnectionBuilder } from "@microsoft/signalr";
import { MessagePackHubProtocol } from "@microsoft/signalr-protocol-msgpack";
var hubConnection = new HubConnectionBuilder().withUrl("/SocialNetwork").withAutomaticReconnect().withHubProtocol(new MessagePackHubProtocol()).build();

hubConnection.on("MessageRecieved", onMessageRecievedHandler);
hubConnection.on("FriendshipRequested", onFriendshipRequestedHandler);
hubConnection.on("FriendshipAccepted", onFriendshipAcceptedHandler);
hubConnection.on("FriendshipRejected", onFriendshipRejectedHandler);
hubConnection.on("FriendshipInvitationCanceled", onFriendshipCanceledHandler)
hubConnection.on("DeletedByUserFromFriends", onDeletedByUserFromFriends)
hubConnection.start();

var notificationsArea = document.querySelector("#notifications ul");
var incomingInvitationsArea = document.querySelector("#incomingInvitations");
var outgoingInvitationsArea = document.querySelector("#outgoingInvitations");
var userId = (document.querySelector("#userId") as HTMLInputElement).value;

function onMessageRecievedHandler(message: Message) {
    if (message.SenderId == userId) {
        return;
    }
    let messageNotifyDiv: HTMLDivElement = document.createElement("div");
    let senderLink: HTMLAnchorElement = document.createElement("a");
    let senderUrl = new URL(message.SenderLink, location.origin);
    senderLink.href = senderUrl.href;
    senderLink.text = message.SenderName;
    senderLink.classList.add("user-link");
    messageNotifyDiv.innerHTML = "Новое сообщение от ";
    messageNotifyDiv.appendChild(senderLink);
    notificationsArea.appendChild(messageNotifyDiv);
}

function onFriendshipRequestedHandler(user: User) {
    let messageNotifyDiv: HTMLDivElement = document.createElement("div");
    let senderLink: HTMLAnchorElement = document.createElement("a");
    let senderUrl = new URL(user.UserPageLink, location.origin);
    senderLink.href = senderUrl.href;
    senderLink.text = user.UserName;
    senderLink.classList.add("user-link");

    messageNotifyDiv.appendChild(senderLink);
    messageNotifyDiv.append("предложил вам дружбу");

    notificationsArea.appendChild(messageNotifyDiv);
}

function onFriendshipAcceptedHandler(user: User) {
    let messageNotifyDiv: HTMLDivElement = document.createElement("div");
    let senderLink: HTMLAnchorElement = document.createElement("a");
    let senderUrl = new URL(user.UserPageLink, location.origin);
    senderLink.href = senderUrl.href;
    senderLink.text = user.UserName;
    senderLink.classList.add("user-link");

    messageNotifyDiv.appendChild(senderLink);
    messageNotifyDiv.append("принял ваше приглашение дружбы");

    notificationsArea.appendChild(messageNotifyDiv);
}

function onFriendshipRejectedHandler(user: User) {
    let messageNotifyDiv: HTMLDivElement = document.createElement("div");
    let senderLink: HTMLAnchorElement = document.createElement("a");
    let senderUrl = new URL(user.UserPageLink, location.origin);
    senderLink.href = senderUrl.href;
    senderLink.text = user.UserName;
    senderLink.classList.add("user-link");

    messageNotifyDiv.appendChild(senderLink);
    messageNotifyDiv.append("отклонил ваше приглашение дружбы");

    notificationsArea.appendChild(messageNotifyDiv);
}

function onFriendshipCanceledHandler(user: User) {
    let messageNotifyDiv: HTMLDivElement = document.createElement("div");
    let senderLink: HTMLAnchorElement = document.createElement("a");
    let senderUrl = new URL(user.UserPageLink, location.origin);
    senderLink.href = senderUrl.href;
    senderLink.text = user.UserName;
    senderLink.classList.add("user-link");

    messageNotifyDiv.appendChild(senderLink);
    messageNotifyDiv.append("отменил приглашение дружбы");

    notificationsArea.appendChild(messageNotifyDiv);
}

function onDeletedByUserFromFriends(user: User) {
    let messageNotifyDiv: HTMLDivElement = document.createElement("div");
    let senderLink: HTMLAnchorElement = document.createElement("a");
    let senderUrl = new URL(user.UserPageLink, location.origin);
    senderLink.href = senderUrl.href;
    senderLink.text = user.UserName;
    senderLink.classList.add("user-link");

    messageNotifyDiv.appendChild(senderLink);
    messageNotifyDiv.append("удалил вас из друзей");

    notificationsArea.appendChild(messageNotifyDiv);
}



//const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]');
//const tooltipList = [...tooltipTriggerList].map(tooltipTriggerEl => new bootstrap.Tooltip(tooltipTriggerEl));

