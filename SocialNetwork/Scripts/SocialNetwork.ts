
import { User, Chat, Message, SocialNetwork } from "./TypedViewModels.js";
import { CreateUserListItem, CreateGroupChatListItem, CreateMessageItem, UserBut, ChatBut } from "./SocNetHtmlGen.js";


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

var messageTextArea = messageInput.firstElementChild as HTMLTextAreaElement;

var comradesHeader = document.getElementById("comrades");
var collectivesHeader = document.getElementById("collectives");
var interlocutorsHeader = document.getElementById("interlocutors");
var comradesArea = document.getElementById("comradesArea");
var collectivesArea = document.getElementById("collectivesArea");
var interlocutorsArea = document.getElementById("interlocutorsArea");
var messagesArea = document.querySelector("#messagesArea");
setListHeight();

var butShowCreationChatDialog: HTMLButtonElement = document.querySelector("#butShowCreationChatDialog");
var butCreateChat: HTMLButtonElement = document.querySelector("#butCreateChat");

butCreateChat.onclick = CreateChat;

async function CreateChat() {
    let chatNameElement: HTMLInputElement = document.querySelector("#chatNameInput") as HTMLInputElement;
    let chatName = chatNameElement.value;
    if (chatName == null || chatName == "") {
        return;
    }

    let createChatUrl = new URL("/SocialNetwork/CreateChat", location.origin);

    let formData = new FormData();
    formData.append("chatName", chatName);
    formData.append("connectionId", hubConnection.connectionId);

    let createChatResponse = await fetch(createChatUrl, {
        method: "POST",
        body: formData
    });

    let result = await createChatResponse.json();

    let chat: Chat = new Chat();
    chat.Id = result.id;
    chat.Name = result.name;
    chat.ChatLink = result.chatLink;
    chat.IsTracked = true;
    chat.Messages = new Array<Message>();

    chat.ButChat = collectivesArea.appendChild(CreateGroupChatListItem(chat, onChatSelected, onMessageSend)).firstElementChild as HTMLButtonElement;
    SocialNetworkModel.Chats.set(chat.Id, chat);
}
hubConnection.on("MessageRecieved", function (message: Message) {
    if (SocialNetworkModel.Chats.has(message.ChatId)) {
        let chat: Chat = SocialNetworkModel.Chats.get(message.ChatId);
        if (chat.IsTracked) {
            chat.Messages.push(message);
            if (currentChatId == chat.Id) {
                onMessageRecieved(message);
            }
        } else {

        }
        return;
    }
    if (SocialNetworkModel.Friends.has(message.ChatId)) {
        let friend: User = SocialNetworkModel.Friends.get(message.ChatId);
        if (!friend.ChatId) {
            friend.ChatId = message.ChatId;
        }
        if (friend.IsTracked) {
            friend.Messages.push(message);
            if (currentChatId == friend.ChatId) {
                onMessageRecieved(message);
            }
        } else {

        }
        return;
    }

    if (SocialNetworkModel.Interlocutors.has(message.ChatId)) {
        let interlocutor: User = SocialNetworkModel.Interlocutors.get(message.ChatId);
        if (!interlocutor.ChatId) {
            interlocutor.ChatId = message.ChatId;
        }
        if (interlocutor.IsTracked) {
            interlocutor.Messages.push(message);
            if (currentChatId == interlocutor.ChatId) {
                onMessageRecieved(message);
            }
        } else {

        }
        return;
    }

    for (let item of SocialNetworkModel.Friends) {
        let friend = item[1];
        if (friend.Id == message.SenderId) {
            friend.ChatId = message.ChatId;
            if (friend.Messages.length == 0)
                friend.Messages = new Array<Message>();

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
                interlocutor.Messages = new Array<Message>();

            interlocutor.Messages.push(message);
            interlocutor.IsTracked = true;

            if (currentChatId == interlocutor.ChatId) {
                onMessageRecieved(message);
            }
            return;
        }
    }

    let interlocutor: User = new User();
    interlocutor.Id = message.SenderId;
    interlocutor.ChatId = message.ChatId;
    interlocutor.UserName = message.SenderName;
    interlocutor.UserPageLink = message.SenderLink;
    interlocutor.IsTracked = true;
    interlocutor.IsFriend = false;
    let UserListItem = CreateUserListItem(interlocutor, onChatSelected, onMessageSend);
    interlocutorsArea.appendChild(UserListItem);

    interlocutor.ButUser = UserListItem.firstElementChild as HTMLButtonElement;

    SocialNetworkModel.Interlocutors.set(interlocutor.ChatId, interlocutor);
    interlocutor.Messages.push(message);

});


function onMessageRecieved(message: Message) {
    messagesArea.appendChild(CreateMessageItem(message));
    messagesArea.parentElement.scrollTop = messagesArea.parentElement.scrollHeight;
}


async function init() {
    var response = await fetch(location.origin+"/SocialNetwork/Data", {
        method: "GET"
    });
    if (response.ok) {
        socNetData = await response.json();
        let friends = new Map<number, User>();
        let chats = new Map<number, Chat>();
        let interlocutors = new Map <number, User>();

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

            user.ButUser = UserListItem.firstElementChild as HTMLButtonElement;

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

            chatObj.ButChat = chatListItem.firstElementChild as HTMLButtonElement;
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
     
            user.ButUser = UserListItem.firstElementChild as HTMLButtonElement;

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

}
function InitialSelectChat(chatId) {
    var chatBut = document.getElementById("chat" + chatId);
    if (chatBut) {
        chatBut.click();
        (chatBut.parentElement.parentElement.parentElement.parentElement.parentElement.previousElementSibling.firstElementChild as HTMLButtonElement).click();
    }
    
}
window.addEventListener("load", () => {
    hubConnection.start().then(init);
});


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

async function onMessageSend(e, action) {
    if (e.keyCode != 13) return;
    if (e.shiftKey) return;
    e.preventDefault();
    var message = messageTextArea.value;
    if (message == null || message == "") return;
    if (!currentChatId) return;

    messageTextArea.value = "";
    var formData = new FormData();

    formData.append("text", message);

    var url = new URL(controller + action + "/" + currentChatId, location.origin);
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


async function GetChatMessagesFromServer(urlConnect: URL): Promise<Array<Message>> {

    var messages: Array<Message>;

    var response = await fetch(urlConnect, {
        method: "GET"
    })
    if (response.ok) {
        var chatMessages = await response.json();
        messages = new Array<Message>(chatMessages.length);
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
}


async function onChatSelected(e) {
    if (chatId == currentChatId && chatId != null) return;
    var chatId;

    messagesArea.innerHTML = "";


    var targetChatBut = e.target;

    if (currentChatbut) {
        currentChatbut.classList.remove("network-list-item-selected");
    }

    currentChatbut = targetChatBut;
    currentChatbut.classList.add("network-list-item-selected");

    var actionConnect;
    var messages: Array<Message>;
    var urlConnect: URL;
    if (targetChatBut.User) {
        let User: User = (targetChatBut as UserBut).User;

        if (!User.ChatId) {
            User.ChatId = await GetDialogId(User.Id);
            targetChatBut.setAttribute("id", "chat" + chatId);
        }

        chatId = User.ChatId;

        if (User.IsTracked) {
            messages = User.Messages;
        } else {
            actionConnect = "ConnectToDialog";
            urlConnect = new URL(controller + actionConnect + "/" + chatId, location.origin);
            urlConnect.searchParams.set("calledUserId", User.Id);

            messages = await GetChatMessagesFromServer(urlConnect);
            User.Messages = messages;
            User.IsTracked = true;
        }

        isCurrentChatADialog = true;
    }

    if (targetChatBut.Chat) {
        let Chat: Chat = (targetChatBut as ChatBut).Chat;

        chatId = Chat.Id;

        if (Chat.IsTracked) {
            messages = Chat.Messages;
        } else {
            actionConnect = "ConnectToChat";
            urlConnect = new URL(controller + actionConnect + "/" + chatId, location.origin);
            urlConnect.searchParams.set("connectionId", hubConnection.connectionId);
            messages = await GetChatMessagesFromServer(urlConnect);
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
} 


document.getElementById("applyFilter").addEventListener("click", async (e) => {
    e.preventDefault();

    var controller = "/SocialNetwork/";

    var formData = new FormData(document.forms.namedItem("filterForm"));

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
    if (downloadedPartialView) {
        partialView = downloadedPartialView;

        filteredUsersWrap.appendChild(partialView);
    }
    (document.querySelector('#resetSearch') as HTMLButtonElement).onclick = e => {
        filteredUsersWrap.innerHTML = "";
        document.forms.namedItem("filterForm").reset();
    }
    (document.querySelector("#butSearchUsers") as HTMLButtonElement).click();
});


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