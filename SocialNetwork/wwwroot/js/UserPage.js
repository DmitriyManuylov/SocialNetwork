var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
import { HubConnectionBuilder } from "@microsoft/signalr";
import { MessagePackHubProtocol } from "@microsoft/signalr-protocol-msgpack";
var hubConnection = new HubConnectionBuilder().withUrl("/SocialNetwork").withAutomaticReconnect().withHubProtocol(new MessagePackHubProtocol()).build();
//var hubConnection = new signalR.HubConnectionBuilder().withUrl("/SocialNetwork").withAutomaticReconnect().withHubProtocol(new signalR.protocols.msgpack.MessagePackHubProtocol()).build();
hubConnection.on("MessageRecieved", onMessageRecievedHandler);
hubConnection.on("FriendshipRequested", onFriendshipRequestedHandler);
hubConnection.on("FriendshipAccepted", onFriendshipAcceptedHandler);
hubConnection.on("FriendshipRejected", onFriendshipRejectedHandler);
hubConnection.on("FriendshipInvitationCanceled", onFriendshipCanceledHandler);
hubConnection.on("DeletedByUserFromFriends", onDeletedByUserFromFriends);
hubConnection.start();
var butInvite;
var butAccept;
var butReject;
var butCancel;
var butDelete;
var sendMessageBut = document.getElementById("SendMessageBut");
var userId = document.getElementById("User_Id").value;
var divFriendshipBlockBlock = document.getElementById("friendshipBlock");
function onMessageRecievedHandler(message) {
}
function setStateIncomingFriendshipInvitation() {
    divFriendshipBlockBlock.innerHTML =
        "<button class=\"btn btn-outline-secondary\" id=\"butAccept\">Принять приглашение</button>" +
            "<button class=\"btn btn-outline-secondary\" id = \"butReject\" > Отклонить приглашение </button>";
    butAccept = document.getElementById("butAccept");
    butReject = document.getElementById("butReject");
    butAccept.onclick = (e) => __awaiter(this, void 0, void 0, function* () {
        acceptFriendship();
    });
    butReject.onclick = (e) => __awaiter(this, void 0, void 0, function* () {
        rejectFriendship();
    });
}
function setStateOutgoingFriendshipInvitation() {
    divFriendshipBlockBlock.innerHTML =
        "<label>Отправлено приглашение</label>" +
            "<button class=\"btn btn-outline-secondary\" id=\"butCancel\">Отозвать приглашение</button>";
    butCancel = document.getElementById("butCancel");
    butCancel.onclick = (e) => __awaiter(this, void 0, void 0, function* () {
        cancelFriendshipInvitation();
    });
}
function setStateNotFriends() {
    divFriendshipBlockBlock.innerHTML = "<button class=\"btn btn-outline-secondary\" id=\"butInvite\">Пригласить</button>";
    butInvite = document.getElementById("butInvite");
    butInvite.onclick = (e) => __awaiter(this, void 0, void 0, function* () {
        inviteUser();
    });
}
function setStateUsersInFriendship() {
    divFriendshipBlockBlock.innerHTML =
        "<label>Вы состоите в дружбе</label>" +
            "<button class=\"btn btn-outline-secondary\" id=\"butDelete\">Удалить из друзей</button>";
    butDelete = document.getElementById("butDelete");
    butDelete.onclick = (e) => __awaiter(this, void 0, void 0, function* () {
        deleteUserFromFriends();
    });
}
function onFriendshipRequestedHandler(user) {
    setStateIncomingFriendshipInvitation();
}
function onFriendshipAcceptedHandler(user) {
    setStateUsersInFriendship();
}
function onFriendshipRejectedHandler(user) {
    setStateNotFriends();
}
function onFriendshipCanceledHandler(user) {
    setStateNotFriends();
}
function onDeletedByUserFromFriends(user) {
    setStateNotFriends();
}
butInvite = document.getElementById("butInvite");
butAccept = document.getElementById("butAccept");
butReject = document.getElementById("butReject");
butCancel = document.getElementById("butCancel");
butDelete = document.getElementById("butDelete");
if (butInvite) {
    butInvite.onclick = (e) => __awaiter(void 0, void 0, void 0, function* () {
        inviteUser();
    });
}
if (butAccept) {
    butAccept.onclick = (e) => __awaiter(void 0, void 0, void 0, function* () {
        acceptFriendship();
    });
}
if (butReject) {
    butReject.onclick = (e) => __awaiter(void 0, void 0, void 0, function* () {
        rejectFriendship();
    });
}
if (butCancel) {
    butCancel.onclick = (e) => __awaiter(void 0, void 0, void 0, function* () {
        cancelFriendshipInvitation();
    });
}
if (butDelete) {
    butDelete.onclick = (e) => __awaiter(void 0, void 0, void 0, function* () {
        deleteUserFromFriends();
    });
}
if (sendMessageBut) {
    sendMessageBut.onclick = (e) => __awaiter(void 0, void 0, void 0, function* () {
        sendMessage();
    });
}
function sendMessage() {
    return __awaiter(this, void 0, void 0, function* () {
        let chatId;
        let urlGetDialogId = new URL(`/SocialNetwork/GetDialogId/${userId}`, location.origin);
        let getDialogIdResponse = yield fetch(urlGetDialogId, {
            method: "GET"
        });
        chatId = yield getDialogIdResponse.json();
        sessionStorage.setItem("currentChatId", chatId);
        let messengerUrl = new URL("/Messenger", location.origin);
        window.open(messengerUrl, "_self");
    });
}
function inviteUser() {
    return __awaiter(this, void 0, void 0, function* () {
        let urlInviteFriend = new URL(`/SocialNetwork/InviteFriend/${userId}`, location.origin);
        let invitationResponse = yield fetch(urlInviteFriend, {
            method: "POST"
        });
        if (invitationResponse.ok) {
            setStateOutgoingFriendshipInvitation();
        }
    });
}
function acceptFriendship() {
    return __awaiter(this, void 0, void 0, function* () {
        let urlAcceptFriend = new URL(`/SocialNetwork/AcceptFriendship/${userId}`, location.origin);
        let invitationAcceptResponse = yield fetch(urlAcceptFriend, {
            method: "POST"
        });
        if (invitationAcceptResponse.ok) {
            setStateUsersInFriendship();
        }
    });
}
function rejectFriendship() {
    return __awaiter(this, void 0, void 0, function* () {
        let urlRejectFriendship = new URL(`/SocialNetwork/RejectFriendship/${userId}`, location.origin);
        let rejectFriendshipResponse = yield fetch(urlRejectFriendship, {
            method: "POST"
        });
        if (rejectFriendshipResponse.ok) {
            setStateNotFriends();
        }
    });
}
function cancelFriendshipInvitation() {
    return __awaiter(this, void 0, void 0, function* () {
        let urlCancelFriendshipInvitation = new URL(`/SocialNetwork/CancelFriendshipInvitation/${userId}`, location.origin);
        let cancelFriendshipInvitationResponse = yield fetch(urlCancelFriendshipInvitation, {
            method: "POST"
        });
        if (cancelFriendshipInvitationResponse.ok) {
            setStateNotFriends();
        }
    });
}
function deleteUserFromFriends() {
    return __awaiter(this, void 0, void 0, function* () {
        let urlDeleteUserFromFriends = new URL(`/SocialNetwork/DeleteUserFromFriends/${userId}`, location.origin);
        let deleteUserFromFriendsResponse = yield fetch(urlDeleteUserFromFriends, {
            method: "POST"
        });
        if (deleteUserFromFriendsResponse.ok) {
            setStateNotFriends();
        }
    });
}
//# sourceMappingURL=UserPage.js.map