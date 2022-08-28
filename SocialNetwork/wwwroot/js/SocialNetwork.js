
import { User, Chat, Message, SocialNetwork } from "./ViewModels.js";
import {CreateUserListItem, CreateGroupChatListItem, CreateMessageItem } from "./SocNetHtmlGen.js";
var hubConnection = new signalR.HubConnectionBuilder().withUrl("/SocialNetwork").withHubProtocol(new signalR.protocols.msgpack.MessagePackHubProtocol()).build();
hubConnection.on("MessageRecieved", function (message) {
    if (message.ChatId != currentChatId) return;
    var mess = {
        chatId: message.ChatId,
        senderId: message.SenderId,
        senderName: message.SenderName,
        senderLink: message.SenderLink,
        dateTime: message.DateTime,
        text: message.Text
    };
    onMessageRecieved(mess);
})
hubConnection.start();
function onMessageRecieved(message) {
    messagesArea.appendChild(CreateMessageItem(message));
    messagesArea.parentElement.scrollTop = messagesArea.parentElement.scrollHeight;
}
var comradesArea = document.getElementById("comradesArea");
var collectivesArea = document.getElementById("collectivesArea");
var interlocutorsArea = document.getElementById("interlocutorsArea");
var messagesArea = document.querySelector("#messagesArea");

async function init() {
    var response = await fetch(location.origin+"/SocialNetwork/Data", {
        method: "GET"
    });
    if (response.ok) {
        socNetData = await response.json();
        socNetData.friends.forEach(friend => comradesArea.appendChild(CreateUserListItem(friend, onChatSelected, onMessageSend)));
        socNetData.chats.forEach(chat => collectivesArea.appendChild(CreateGroupChatListItem(chat, onChatSelected, onMessageSend)));
        socNetData.interlocutors.forEach(int => interlocutorsArea.appendChild(CreateUserListItem(int, onChatSelected, onMessageSend)));

        let savedChatId = sessionStorage.getItem("currentChatId");
        if (savedChatId) {
            InitialSelectChat(savedChatId);
        }
    }

}
function InitialSelectChat(chatId) {
    var chatBut = document.getElementById("chat" + chatId);
    if (chatBut) {
        chatBut.parentElement.parentElement.parentElement.parentElement.parentElement.previousElementSibling.firstElementChild.click();
        chatBut.click();
    }
    
}
window.addEventListener("load", init);

var currentChatbut;
var currentChatId;
var isCurrentChatADialog;
var actionDisconnect = "DisconnectFromChat";

var socNetData;
var currentChatMessages;
var messageInput = document.querySelector("#messageInput");

var messageTextArea = messageInput.firstElementChild;

async function GetDialogId(userId) {
    if (!userId) return;
    var url = location.origin + "/SocialNetwork/GetDialogId/" + userId;
    var response = await fetch(url, {
        method: "GET"
    });
    var chatId;
    if (response.ok)
        chatId = await response.json();
    return chatId;
}

async function onMessageSend(e, action, chatId) {
    if (e.keyCode != 13) return;
    if (e.shiftKey) return;
    e.preventDefault();
    var message = messageTextArea.value;
    if (message == null || message == "") return;
    if (!currentChatId) return;

    var controller = "/SocialNetwork/";
    messageTextArea.value = "";
    var formData = new FormData();
    /*    formData.append("chatId", currentChatId)*/
    formData.append("text", message);
    var url = location.origin + controller + action + "/" + chatId;
    if (action == "SendMessageToInterlocutor") {
        formData.append("calledUserId", currentChatbut.parentElement.children[2].value);
    }
    var response = await fetch(url, {
        method: "POST",
        body: formData
    });
    if (response.ok) {
        console.log("Сообщение отправлено");
    }
    messagesArea.parentElement.scrollTop = messagesArea.parentElement.scrollHeight;
}


async function onChatSelected(e, actionConnect, isChatADialog, chatId) {
    if (chatId == currentChatId && chatId != null) return;
    messagesArea.innerHTML = "";
    var queryString = "?" + "connectionId=" + hubConnection.connectionId;
    var controller = "/SocialNetwork/";
    var targetChatBut = e.target;
    var hiddenChatId = targetChatBut.nextSibling;
    var hiddenUserId = hiddenChatId.nextSibling;
    if (hiddenUserId) {
        var userId = hiddenUserId.value;
        if (!chatId) {
            chatId = await GetDialogId(userId);
            targetChatBut.setAttribute("id", "chat" + chatId);
        }
    }

    var urlConnect = location.origin + controller + actionConnect + "/" + chatId;
    if (currentChatbut) {
        currentChatbut.classList.remove("network-list-item-selected");
        if (!isCurrentChatADialog) {
            var urlDisconnect = location.origin + controller + actionDisconnect + "/" + currentChatId;
            await fetch(urlDisconnect + queryString, {
                method: "GET"
            });

        };
    }
    isCurrentChatADialog = isChatADialog;
    currentChatbut = targetChatBut;
    currentChatbut.classList.add("network-list-item-selected");

    currentChatId = chatId;

    sessionStorage.setItem("currentChatId", currentChatId);

    var response = await fetch(urlConnect + queryString, {
        method: "GET"
    })
    if (response.ok) {
        currentChatMessages = await response.json();
        currentChatMessages.forEach(message => messagesArea.appendChild(CreateMessageItem(message)));
        messagesArea.parentElement.scrollTop = messagesArea.parentElement.scrollHeight;
    }

    
}
document.getElementById("applyFilter").addEventListener("click", async (e) => {
    e.preventDefault();

    var controller = "/SocialNetwork/";

    var formData = new FormData(filterForm);

    var urlConnect = location.origin + controller + "FilterUsers";
    var response = await fetch(urlConnect, {
        method: "POST",
        body: formData
    });
    var filteredUsersWrap = document.getElementById("filteredUsersWrap");

    var result = await response.text();
    filteredUsersWrap.innerHTML = "";
    var downloadedPartialView = new DOMParser().parseFromString(result, "text/html").getElementById("filteredUsers");
    var partialView;
    if (downloadedPartialView)
        partialView = downloadedPartialView;
    else {
        partialView = document.createElement("div");
        partialView.innerText = "Не найдено ни одного пользователя";
    }
    filteredUsersWrap.appendChild(partialView);
    document.getElementById("butSearchUsers").click();
});