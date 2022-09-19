
import { Message, User } from "./TypedViewModels";

import { HubConnectionBuilder } from "@microsoft/signalr";
import { MessagePackHubProtocol } from "@microsoft/signalr-protocol-msgpack";
var hubConnection = new HubConnectionBuilder().withUrl("/SocialNetwork").withAutomaticReconnect().withHubProtocol(new MessagePackHubProtocol()).build();
//var hubConnection = new signalR.HubConnectionBuilder().withUrl("/SocialNetwork").withAutomaticReconnect().withHubProtocol(new signalR.protocols.msgpack.MessagePackHubProtocol()).build();

hubConnection.on("MessageRecieved", onMessageRecievedHandler);
hubConnection.on("FriendshipRequested", onFriendshipRequestedHandler);
hubConnection.on("FriendshipAccepted", onFriendshipAcceptedHandler);
hubConnection.on("FriendshipRejected", onFriendshipRejectedHandler);
hubConnection.on("FriendshipInvitationCanceled", onFriendshipCanceledHandler)
hubConnection.on("DeletedByUserFromFriends", onDeletedByUserFromFriends)
hubConnection.start();

var butInvite: HTMLButtonElement;
var butAccept: HTMLButtonElement;
var butReject: HTMLButtonElement;
var butCancel: HTMLButtonElement;
var butDelete: HTMLButtonElement;
var sendMessageBut = document.getElementById("SendMessageBut");
var userId = (document.getElementById("User_Id") as HTMLInputElement).value;

var divFriendshipBlockBlock: HTMLDivElement = document.getElementById("friendshipBlock") as HTMLDivElement;


function onMessageRecievedHandler(message: Message) {
   
}

function setStateIncomingFriendshipInvitation() {
    divFriendshipBlockBlock.innerHTML =
        "<button class=\"btn btn-outline-secondary\" id=\"butAccept\">Принять приглашение</button>" +
        "<button class=\"btn btn-outline-secondary\" id = \"butReject\" > Отклонить приглашение </button>";
    butAccept = document.getElementById("butAccept") as HTMLButtonElement;
    butReject = document.getElementById("butReject") as HTMLButtonElement;
    butAccept.onclick = async e => {
        acceptFriendship();
    };
    butReject.onclick = async e => {
        rejectFriendship();
    }
}

function setStateOutgoingFriendshipInvitation() {
    divFriendshipBlockBlock.innerHTML =
        "<label>Отправлено приглашение</label>" +
        "<button class=\"btn btn-outline-secondary\" id=\"butCancel\">Отозвать приглашение</button>";
    butCancel = document.getElementById("butCancel") as HTMLButtonElement;
    butCancel.onclick = async e => {
        cancelFriendshipInvitation();
    }
}

function setStateNotFriends() {
    divFriendshipBlockBlock.innerHTML = "<button class=\"btn btn-outline-secondary\" id=\"butInvite\">Пригласить</button>";
    butInvite = document.getElementById("butInvite") as HTMLButtonElement;
    butInvite.onclick = async e => {
        inviteUser();
    }
}

function setStateUsersInFriendship() {
    divFriendshipBlockBlock.innerHTML =
        "<label>Вы состоите в дружбе</label>" +
        "<button class=\"btn btn-outline-secondary\" id=\"butDelete\">Удалить из друзей</button>";
    butDelete = document.getElementById("butDelete") as HTMLButtonElement;
    butDelete.onclick = async e => {
        deleteUserFromFriends();
    }
}

function onFriendshipRequestedHandler(user: User) {
    setStateIncomingFriendshipInvitation();
}

function onFriendshipAcceptedHandler(user: User) {
    setStateUsersInFriendship();
}

function onFriendshipRejectedHandler(user: User) {
    setStateNotFriends();
}

function onFriendshipCanceledHandler(user: User) {
    setStateNotFriends();
}

function onDeletedByUserFromFriends(user: User) {
    setStateNotFriends();
}

butInvite = document.getElementById("butInvite") as HTMLButtonElement;
butAccept = document.getElementById("butAccept") as HTMLButtonElement;
butReject = document.getElementById("butReject") as HTMLButtonElement;
butCancel = document.getElementById("butCancel") as HTMLButtonElement;
butDelete = document.getElementById("butDelete") as HTMLButtonElement;

if (butInvite) {
    butInvite.onclick = async (e) => {
        inviteUser();
    }
}


if (butAccept) {
    butAccept.onclick = async e => {
        acceptFriendship();
    }
}

if (butReject) {
    butReject.onclick = async e => {
        rejectFriendship();
    }
}

if (butCancel) {
    butCancel.onclick = async e => {
        cancelFriendshipInvitation();
    }
}

if (butDelete) {
    butDelete.onclick = async e => {
        deleteUserFromFriends();
    }
}

if (sendMessageBut) {
    sendMessageBut.onclick = async (e) => {
        sendMessage();
    }
}

async function sendMessage() {
    let chatId;

    let urlGetDialogId = new URL(`/SocialNetwork/GetDialogId/${userId}`, location.origin);
    let getDialogIdResponse = await fetch(urlGetDialogId, {
        method: "GET"
    });

    chatId = await getDialogIdResponse.json();

    sessionStorage.setItem("currentChatId", chatId);

    let messengerUrl = new URL("/Messenger", location.origin);

    window.open(messengerUrl, "_self");
}

async function inviteUser() {
    let urlInviteFriend = new URL(`/SocialNetwork/InviteFriend/${userId}`, location.origin);
    let invitationResponse = await fetch(urlInviteFriend, {
        method: "POST"
    });
    if (invitationResponse.ok) {
        setStateOutgoingFriendshipInvitation();
    }
}

async function acceptFriendship() {
    let urlAcceptFriend = new URL(`/SocialNetwork/AcceptFriendship/${userId}`, location.origin);
    let invitationAcceptResponse = await fetch(urlAcceptFriend, {
        method: "POST"
    });
    if (invitationAcceptResponse.ok) {
        setStateUsersInFriendship();
    }
}

async function rejectFriendship() {
    let urlRejectFriendship = new URL(`/SocialNetwork/RejectFriendship/${userId}`, location.origin);
    let rejectFriendshipResponse = await fetch(urlRejectFriendship, {
        method: "POST"
    })
    if (rejectFriendshipResponse.ok) {
        setStateNotFriends();
    }
}

async function cancelFriendshipInvitation() {
    let urlCancelFriendshipInvitation = new URL(`/SocialNetwork/CancelFriendshipInvitation/${userId}`, location.origin);
    let cancelFriendshipInvitationResponse = await fetch(urlCancelFriendshipInvitation, {
        method: "POST"
    })
    if (cancelFriendshipInvitationResponse.ok) {
        setStateNotFriends();
    }
}

async function deleteUserFromFriends() {
    let urlDeleteUserFromFriends = new URL(`/SocialNetwork/DeleteUserFromFriends/${userId}`, location.origin);
    let deleteUserFromFriendsResponse = await fetch(urlDeleteUserFromFriends, {
        method: "POST"
    })
    if (deleteUserFromFriendsResponse.ok) {
        setStateNotFriends();
    }
}