"use strict";
exports.__esModule = true;
var hubConnection = new signalR.HubConnectionBuilder().withUrl("/SocialNetwork").withAutomaticReconnect().withHubProtocol(new signalR.protocols.msgpack.MessagePackHubProtocol()).build();
hubConnection.on("MessageRecieved", onMessageRecievedHandler);
hubConnection.on("FriendshipRequested", onFriendshipRequestedHandler);
hubConnection.on("FriendshipAccepted", onFriendshipAcceptedHandler);
hubConnection.on("FriendshipRejected", onFriendshipRejectedHandler);
hubConnection.on("FriendshipInvitationCanceled", onFriendshipCanceledHandler);
hubConnection.on("DeletedByUserFromFriends", onDeletedByUserFromFriends);
hubConnection.start();
var notificationsArea = document.querySelector("#notifications ul");
var incomingInvitationsArea = document.querySelector("#incomingInvitations");
var outgoingInvitationsArea = document.querySelector("#outgoingInvitations");
function onMessageRecievedHandler(message) {
    var messageNotifyDiv = document.createElement("div");
    var senderLink = document.createElement("a");
    var senderUrl = new URL(message.SenderLink, location.origin);
    senderLink.href = senderUrl.href;
    senderLink.text = message.SenderName;
    senderLink.classList.add("user-link");
    messageNotifyDiv.innerHTML = "Новое сообщение от ";
    messageNotifyDiv.appendChild(senderLink);
    notificationsArea.appendChild(messageNotifyDiv);
}
function onFriendshipRequestedHandler(user) {
    var messageNotifyDiv = document.createElement("div");
    var senderLink = document.createElement("a");
    var senderUrl = new URL(user.UserPageLink, location.origin);
    senderLink.href = senderUrl.href;
    senderLink.text = user.UserName;
    senderLink.classList.add("user-link");
    messageNotifyDiv.appendChild(senderLink);
    messageNotifyDiv.append("предложил вам дружбу");
    notificationsArea.appendChild(messageNotifyDiv);
}
function onFriendshipAcceptedHandler(user) {
    var messageNotifyDiv = document.createElement("div");
    var senderLink = document.createElement("a");
    var senderUrl = new URL(user.UserPageLink, location.origin);
    senderLink.href = senderUrl.href;
    senderLink.text = user.UserName;
    senderLink.classList.add("user-link");
    messageNotifyDiv.appendChild(senderLink);
    messageNotifyDiv.append("принял ваше приглашение дружбы");
    notificationsArea.appendChild(messageNotifyDiv);
}
function onFriendshipRejectedHandler(user) {
    var messageNotifyDiv = document.createElement("div");
    var senderLink = document.createElement("a");
    var senderUrl = new URL(user.UserPageLink, location.origin);
    senderLink.href = senderUrl.href;
    senderLink.text = user.UserName;
    senderLink.classList.add("user-link");
    messageNotifyDiv.appendChild(senderLink);
    messageNotifyDiv.append("отклонил ваше приглашение дружбы");
    notificationsArea.appendChild(messageNotifyDiv);
}
function onFriendshipCanceledHandler(user) {
    var messageNotifyDiv = document.createElement("div");
    var senderLink = document.createElement("a");
    var senderUrl = new URL(user.UserPageLink, location.origin);
    senderLink.href = senderUrl.href;
    senderLink.text = user.UserName;
    senderLink.classList.add("user-link");
    messageNotifyDiv.appendChild(senderLink);
    messageNotifyDiv.append("отменил приглашение дружбы");
    notificationsArea.appendChild(messageNotifyDiv);
}
function onDeletedByUserFromFriends(user) {
    var messageNotifyDiv = document.createElement("div");
    var senderLink = document.createElement("a");
    var senderUrl = new URL(user.UserPageLink, location.origin);
    senderLink.href = senderUrl.href;
    senderLink.text = user.UserName;
    senderLink.classList.add("user-link");
    messageNotifyDiv.appendChild(senderLink);
    messageNotifyDiv.append("удалил вас из друзей");
    notificationsArea.appendChild(messageNotifyDiv);
}
//const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]');
//const tooltipList = [...tooltipTriggerList].map(tooltipTriggerEl => new bootstrap.Tooltip(tooltipTriggerEl));
