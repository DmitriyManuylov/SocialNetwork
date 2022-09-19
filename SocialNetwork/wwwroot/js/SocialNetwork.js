var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
import { User, Chat, Message, SocialNetwork } from "./TypedViewModels.js";
import { CreateUserListItem, CreateGroupChatListItem, CreateMessageItem } from "./SocNetHtmlGen.js";
import { HubConnectionBuilder } from "@microsoft/signalr";
import { MessagePackHubProtocol } from "@microsoft/signalr-protocol-msgpack";
var hubConnection = new HubConnectionBuilder().withUrl("/SocialNetwork").withAutomaticReconnect().withHubProtocol(new MessagePackHubProtocol()).build();
//var hubConnection = new signalR.HubConnectionBuilder().withUrl("/SocialNetwork").withAutomaticReconnect().withHubProtocol(new signalR.protocols.msgpack.MessagePackHubProtocol()).build();
var controller = "/SocialNetwork/";
var SocialNetworkModel = new SocialNetwork();
var currentChatbut;
var currentChatId;
var isCurrentChatADialog;
var actionDisconnect = "DisconnectFromChat";
var socNetData;
var currentChatMessages;
var messageInput = document.querySelector("#messageInput");
var messageTextArea = messageInput.firstElementChild;
var comradesHeader = document.getElementById("comrades");
var collectivesHeader = document.getElementById("collectives");
var interlocutorsHeader = document.getElementById("interlocutors");
var comradesArea = document.getElementById("comradesArea");
var collectivesArea = document.getElementById("collectivesArea");
var interlocutorsArea = document.getElementById("interlocutorsArea");
var messagesArea = document.querySelector("#messagesArea");
setListHeight();
var butShowCreationChatDialog = document.querySelector("#butShowCreationChatDialog");
var butCreateChat = document.querySelector("#butCreateChat");
butCreateChat.onclick = CreateChat;
function CreateChat() {
    return __awaiter(this, void 0, void 0, function* () {
        let chatNameElement = document.querySelector("#chatNameInput");
        let chatName = chatNameElement.value;
        if (chatName == null || chatName == "") {
            return;
        }
        let createChatUrl = new URL("/SocialNetwork/CreateChat", location.origin);
        let formData = new FormData();
        formData.append("chatName", chatName);
        let createChatResponse = yield fetch(createChatUrl, {
            method: "POST",
            body: formData
        });
        let result = yield createChatResponse.json();
        let chat = new Chat();
        chat.Id = result.id;
        chat.Name = result.name;
        chat.ChatLink = result.chatLink;
        chat.IsTracked = true;
        chat.Messages = new Array();
        chat.ButChat = collectivesArea.appendChild(CreateGroupChatListItem(chat, onChatSelected, onMessageSend)).firstElementChild;
        SocialNetworkModel.Chats.set(chat.Id, chat);
    });
}
hubConnection.on("MessageRecieved", function (message) {
    if (SocialNetworkModel.Chats.has(message.ChatId)) {
        let chat = SocialNetworkModel.Chats.get(message.ChatId);
        if (chat.IsTracked) {
            chat.Messages.push(message);
            if (currentChatId == chat.Id) {
                onMessageRecieved(message);
            }
        }
        else {
        }
        return;
    }
    if (SocialNetworkModel.Friends.has(message.ChatId)) {
        let friend = SocialNetworkModel.Friends.get(message.ChatId);
        if (!friend.ChatId) {
            friend.ChatId = message.ChatId;
        }
        if (friend.IsTracked) {
            friend.Messages.push(message);
            if (currentChatId == friend.ChatId) {
                onMessageRecieved(message);
            }
        }
        else {
        }
        return;
    }
    if (SocialNetworkModel.Interlocutors.has(message.ChatId)) {
        let interlocutor = SocialNetworkModel.Interlocutors.get(message.ChatId);
        if (!interlocutor.ChatId) {
            interlocutor.ChatId = message.ChatId;
        }
        if (interlocutor.IsTracked) {
            interlocutor.Messages.push(message);
            if (currentChatId == interlocutor.ChatId) {
                onMessageRecieved(message);
            }
        }
        else {
        }
        return;
    }
    for (let item of SocialNetworkModel.Friends) {
        let friend = item[1];
        if (friend.Id == message.SenderId) {
            friend.ChatId = message.ChatId;
            if (friend.Messages.length == 0)
                friend.Messages = new Array();
            friend.Messages.push(message);
            friend.IsTracked = true;
            if (currentChatId == friend.ChatId) {
                onMessageRecieved(message);
            }
            return;
        }
    }
    for (let item of SocialNetworkModel.Interlocutors) {
        let interlocutor = item[1];
        if (interlocutor.Id == message.SenderId) {
            interlocutor.ChatId = message.ChatId;
            if (interlocutor.Messages.length == 0)
                interlocutor.Messages = new Array();
            interlocutor.Messages.push(message);
            interlocutor.IsTracked = true;
            if (currentChatId == interlocutor.ChatId) {
                onMessageRecieved(message);
            }
            return;
        }
    }
    let interlocutor = new User();
    interlocutor.Id = message.SenderId;
    interlocutor.ChatId = message.ChatId;
    interlocutor.UserName = message.SenderName;
    interlocutor.UserPageLink = message.SenderLink;
    interlocutor.IsTracked = true;
    interlocutor.IsFriend = false;
    let UserListItem = CreateUserListItem(interlocutor, onChatSelected, onMessageSend);
    interlocutorsArea.appendChild(UserListItem);
    interlocutor.ButUser = UserListItem.firstElementChild;
    SocialNetworkModel.Interlocutors.set(interlocutor.ChatId, interlocutor);
    interlocutor.Messages.push(message);
});
function onMessageRecieved(message) {
    messagesArea.appendChild(CreateMessageItem(message));
    messagesArea.parentElement.scrollTop = messagesArea.parentElement.scrollHeight;
}
function init() {
    return __awaiter(this, void 0, void 0, function* () {
        var response = yield fetch(location.origin + "/SocialNetwork/Data", {
            method: "GET"
        });
        if (response.ok) {
            socNetData = yield response.json();
            let friends = new Map();
            let chats = new Map();
            let interlocutors = new Map();
            socNetData.friends.forEach(friend => {
                let user = new User();
                user.Id = friend.id;
                user.ChatId = friend.chatId;
                user.UserName = friend.userName;
                user.UserPageLink = friend.userPageLink;
                user.IsTracked = false;
                user.IsFriend = true;
                let UserListItem = CreateUserListItem(user, onChatSelected, onMessageSend);
                comradesArea.appendChild(UserListItem);
                user.ButUser = UserListItem.firstElementChild;
                friends.set(user.ChatId, user);
            });
            socNetData.chats.forEach(chat => {
                let chatObj = new Chat();
                chatObj.Id = chat.id;
                chatObj.Name = chat.name;
                chatObj.IsTracked = false;
                chatObj.ChatLink = chat.chatLink;
                let chatListItem = CreateGroupChatListItem(chatObj, onChatSelected, onMessageSend);
                collectivesArea.appendChild(chatListItem);
                chatObj.ButChat = chatListItem.firstElementChild;
                chats.set(chatObj.Id, chatObj);
            });
            socNetData.interlocutors.forEach(interlocutor => {
                let user = new User();
                user.Id = interlocutor.id;
                user.ChatId = interlocutor.chatId;
                user.UserName = interlocutor.userName;
                user.UserPageLink = interlocutor.userPageLink;
                user.IsTracked = false;
                user.IsFriend = false;
                let UserListItem = CreateUserListItem(user, onChatSelected, onMessageSend);
                interlocutorsArea.appendChild(UserListItem);
                user.ButUser = UserListItem.firstElementChild;
                interlocutors.set(user.ChatId, user);
            });
            SocialNetworkModel.UserId = socNetData.userId;
            SocialNetworkModel.Friends = friends;
            SocialNetworkModel.Chats = chats;
            SocialNetworkModel.Interlocutors = interlocutors;
            let savedChatId = sessionStorage.getItem("currentChatId");
            if (savedChatId) {
                InitialSelectChat(savedChatId);
            }
        }
    });
}
function InitialSelectChat(chatId) {
    var chatBut = document.getElementById("chat" + chatId);
    if (chatBut) {
        chatBut.click();
        chatBut.parentElement.parentElement.parentElement.parentElement.parentElement.previousElementSibling.firstElementChild.click();
    }
}
window.addEventListener("load", () => hubConnection.start().then(init));
function GetDialogId(userId) {
    return __awaiter(this, void 0, void 0, function* () {
        if (!userId)
            return;
        var url = location.origin + "/SocialNetwork/GetDialogId/" + userId;
        var response = yield fetch(url, {
            method: "GET"
        });
        var chatId;
        if (response.ok)
            chatId = yield response.json();
        return chatId;
    });
}
function onMessageSend(e, action) {
    return __awaiter(this, void 0, void 0, function* () {
        if (e.keyCode != 13)
            return;
        if (e.shiftKey)
            return;
        e.preventDefault();
        var message = messageTextArea.value;
        if (message == null || message == "")
            return;
        if (!currentChatId)
            return;
        messageTextArea.value = "";
        var formData = new FormData();
        formData.append("text", message);
        var url = new URL(controller + action + "/" + currentChatId, location.origin);
        if (action == "SendMessageToInterlocutor") {
            formData.append("calledUserId", currentChatbut.parentElement.children[2].value);
        }
        var response = yield fetch(url, {
            method: "POST",
            body: formData
        });
        if (response.ok) {
            console.log("Сообщение отправлено");
        }
        messagesArea.parentElement.scrollTop = messagesArea.parentElement.scrollHeight;
    });
}
function GetChatMessagesFromServer(urlConnect) {
    return __awaiter(this, void 0, void 0, function* () {
        var messages;
        var response = yield fetch(urlConnect, {
            method: "GET"
        });
        if (response.ok) {
            var chatMessages = yield response.json();
            messages = new Array(chatMessages.length);
            chatMessages.forEach(message => {
                let messageObj = new Message();
                messageObj.ChatId = message.chatId;
                messageObj.SenderId = message.senderId;
                messageObj.Text = message.text;
                messageObj.SenderName = message.senderName;
                messageObj.SenderLink = message.senderLink;
                messageObj.DateTime = message.dateTime;
                messages.push(messageObj);
            });
            return messages;
        }
        else {
            throw "Ошибка запроса";
        }
    });
}
function onChatSelected(e) {
    return __awaiter(this, void 0, void 0, function* () {
        if (chatId == currentChatId && chatId != null)
            return;
        var chatId;
        messagesArea.innerHTML = "";
        var targetChatBut = e.target;
        if (currentChatbut) {
            currentChatbut.classList.remove("network-list-item-selected");
        }
        currentChatbut = targetChatBut;
        currentChatbut.classList.add("network-list-item-selected");
        var actionConnect;
        var messages;
        var urlConnect;
        if (targetChatBut.User) {
            let User = targetChatBut.User;
            if (!User.ChatId) {
                User.ChatId = yield GetDialogId(User.Id);
                targetChatBut.setAttribute("id", "chat" + chatId);
            }
            chatId = User.ChatId;
            if (User.IsTracked) {
                messages = User.Messages;
            }
            else {
                actionConnect = "ConnectToDialog";
                urlConnect = new URL(controller + actionConnect + "/" + chatId, location.origin);
                urlConnect.searchParams.set("calledUserId", User.Id);
                messages = yield GetChatMessagesFromServer(urlConnect);
                User.Messages = messages;
                User.IsTracked = true;
            }
            isCurrentChatADialog = true;
        }
        if (targetChatBut.Chat) {
            let Chat = targetChatBut.Chat;
            chatId = Chat.Id;
            if (Chat.IsTracked) {
                messages = Chat.Messages;
            }
            else {
                actionConnect = "ConnectToChat";
                urlConnect = new URL(controller + actionConnect + "/" + chatId, location.origin);
                urlConnect.searchParams.set("connectionId", hubConnection.connectionId);
                messages = yield GetChatMessagesFromServer(urlConnect);
                Chat.Messages = messages;
                Chat.IsTracked = true;
            }
            isCurrentChatADialog = false;
        }
        currentChatId = chatId;
        currentChatMessages = messages;
        sessionStorage.setItem("currentChatId", currentChatId);
        if (messages) {
            messages.forEach(message => {
                messagesArea.appendChild(CreateMessageItem(message));
            });
        }
        messagesArea.parentElement.scrollTop = messagesArea.parentElement.scrollHeight;
    });
}
document.getElementById("applyFilter").addEventListener("click", (e) => __awaiter(void 0, void 0, void 0, function* () {
    e.preventDefault();
    var controller = "/SocialNetwork/";
    var formData = new FormData(document.forms.namedItem("filterForm"));
    var urlConnect = location.origin + controller + "FilterUsers";
    var response = yield fetch(urlConnect, {
        method: "POST",
        body: formData
    });
    var filteredUsersWrap = document.getElementById("filteredUsersWrap");
    var result = yield response.text();
    filteredUsersWrap.innerHTML = "";
    var downloadedPartialView = new DOMParser().parseFromString(result, "text/html").getElementById("filteredUsers");
    var partialView;
    if (downloadedPartialView) {
        partialView = downloadedPartialView;
        filteredUsersWrap.appendChild(partialView);
    }
    document.querySelector('#resetSearch').onclick = e => {
        filteredUsersWrap.innerHTML = "";
        document.forms.namedItem("filterForm").reset();
    };
    document.querySelector("#butSearchUsers").click();
}));
function setListHeight() {
    var maxListSize = comradesHeader.parentElement.parentElement.clientHeight
        - (comradesHeader.firstElementChild.clientHeight
            + collectivesHeader.firstElementChild.clientHeight
            + interlocutorsHeader.firstElementChild.clientHeight) - 28;
    comradesArea.setAttribute("style", "height:" + maxListSize.toString() + "px");
    collectivesArea.setAttribute("style", "height:" + maxListSize.toString() + "px");
    interlocutorsArea.setAttribute("style", "height:" + maxListSize.toString() + "px");
}
window.onresize = e => {
    setListHeight();
};
//# sourceMappingURL=SocialNetwork.js.map