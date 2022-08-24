var hubConnection = new signalR.HubConnectionBuilder().withUrl("/SocialNetwork").withHubProtocol(new signalR.protocols.msgpack.MessagePackHubProtocol()).build();

var sendMessageBut = document.getElementById("SendMessageBut");
var userId = document.getElementById("User_Id").value;
if (sendMessageBut) {
    sendMessageBut.onclick = async (e) => {
        let chatId;

        let urlGetDialogId = new URL("/SocialNetwork/GetDialogId", location.origin);
        urlGetDialogId.searchParams.set("interlocutorId", userId);
        let getDialogIdResponse = await fetch(urlGetDialogId, {
            method: "GET"
        });

        chatId = await getDialogIdResponse.json();

        sessionStorage.setItem("currentChatId", chatId);
        
        let messengerUrl = new URL("/SocialNetwork/Index", location.origin);

        window.open(messengerUrl, "_blank");

    }
}