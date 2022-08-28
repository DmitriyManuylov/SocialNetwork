var hubConnection = new signalR.HubConnectionBuilder().withUrl("/SocialNetwork").withHubProtocol(new signalR.protocols.msgpack.MessagePackHubProtocol()).build();
//hubConnection.st
hubConnection.on("FriendshipRequested", e => {

});


var butInvite = document.getElementById("butInvite");
var butAccept = document.getElementById("butAccept");
var sendMessageBut = document.getElementById("SendMessageBut");
var userId = document.getElementById("User_Id").value;

if (butInvite) {
    butInvite.onclick = async (e) => {
        let urlInviteFriend = new URL("/SocialNetwork/InviteFriend/" + userId, location.origin);
        let invitationResponse = await fetch(urlInviteFriend, {
            method: "POST"
        });
        if (invitationResponse.ok) {
            let friendshipStateLabel = document.createElement("label");
            friendshipStateLabel.innerText = "Отправлено приглашение";
            butInvite.parentElement.insertBefore(friendshipStateLabel, butInvite);
            butInvite.remove();
        }
    }
}

if (butAccept) {
    butAccept.onclick = async e => {
        let urlAcceptFriend = new URL("/SocialNetwork/AcceptFriendship/" + userId, location.origin);
        let invitationAcceptResponse = await fetch(urlAcceptFriend, {
            method: "POST"
        });
        if (invitationAcceptResponse.ok) {
            let friendshipStateLabel = document.createElement("label");
            friendshipStateLabel.innerText = "Вы состоите в дружбе";
            butAccept.parentElement.insertBefore(friendshipStateLabel, butAccept);
            butAccept.remove();
        }
    }
}

if (sendMessageBut) {
    sendMessageBut.onclick = async (e) => {
        let chatId;

        let urlGetDialogId = new URL("/SocialNetwork/GetDialogId", location.origin);
        urlGetDialogId.searchParams.set("calledUserId", userId);
        let getDialogIdResponse = await fetch(urlGetDialogId, {
            method: "GET"
        });

        chatId = await getDialogIdResponse.json();

        sessionStorage.setItem("currentChatId", chatId);
        
        let messengerUrl = new URL("/SocialNetwork/Index", location.origin);

        window.open(messengerUrl, "_blank");

    }
}