var chatId = (document.getElementById("chatId") as HTMLInputElement).value;
let butSendMessage = document.getElementById("sendMessageBut");

if (butSendMessage) {
    butSendMessage.onclick = async e => {
        sendMessage();
    }
}

async function sendMessage() {

    sessionStorage.setItem("currentChatId", chatId);

    let messengerUrl = new URL("/Messenger", location.origin);

    window.open(messengerUrl, "_self");
}