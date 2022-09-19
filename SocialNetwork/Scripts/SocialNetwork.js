"use strict";
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
var __generator = (this && this.__generator) || function (thisArg, body) {
    var _ = { label: 0, sent: function() { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g;
    return g = { next: verb(0), "throw": verb(1), "return": verb(2) }, typeof Symbol === "function" && (g[Symbol.iterator] = function() { return this; }), g;
    function verb(n) { return function (v) { return step([n, v]); }; }
    function step(op) {
        if (f) throw new TypeError("Generator is already executing.");
        while (_) try {
            if (f = 1, y && (t = op[0] & 2 ? y["return"] : op[0] ? y["throw"] || ((t = y["return"]) && t.call(y), 0) : y.next) && !(t = t.call(y, op[1])).done) return t;
            if (y = 0, t) op = [op[0] & 2, t.value];
            switch (op[0]) {
                case 0: case 1: t = op; break;
                case 4: _.label++; return { value: op[1], done: false };
                case 5: _.label++; y = op[1]; op = [0]; continue;
                case 7: op = _.ops.pop(); _.trys.pop(); continue;
                default:
                    if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) { _ = 0; continue; }
                    if (op[0] === 3 && (!t || (op[1] > t[0] && op[1] < t[3]))) { _.label = op[1]; break; }
                    if (op[0] === 6 && _.label < t[1]) { _.label = t[1]; t = op; break; }
                    if (t && _.label < t[2]) { _.label = t[2]; _.ops.push(op); break; }
                    if (t[2]) _.ops.pop();
                    _.trys.pop(); continue;
            }
            op = body.call(thisArg, _);
        } catch (e) { op = [6, e]; y = 0; } finally { f = t = 0; }
        if (op[0] & 5) throw op[1]; return { value: op[0] ? op[1] : void 0, done: true };
    }
};
exports.__esModule = true;
var TypedViewModels_js_1 = require("./TypedViewModels.js");
var SocNetHtmlGen_js_1 = require("./SocNetHtmlGen.js");
var hubConnection = new signalR.HubConnectionBuilder().withUrl("/SocialNetwork").withAutomaticReconnect().withHubProtocol(new signalR.protocols.msgpack.MessagePackHubProtocol()).build();
var controller = "/SocialNetwork/";
var SocialNetworkModel = new TypedViewModels_js_1.SocialNetwork();
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
    return __awaiter(this, void 0, void 0, function () {
        var chatNameElement, chatName, createChatUrl, formData, createChatResponse, result, chat;
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0:
                    chatNameElement = document.querySelector("#chatNameInput");
                    chatName = chatNameElement.value;
                    if (chatName == null || chatName == "") {
                        return [2 /*return*/];
                    }
                    createChatUrl = new URL("/SocialNetwork/CreateChat", location.origin);
                    formData = new FormData();
                    formData.append("chatName", chatName);
                    return [4 /*yield*/, fetch(createChatUrl, {
                            method: "POST",
                            body: formData
                        })];
                case 1:
                    createChatResponse = _a.sent();
                    return [4 /*yield*/, createChatResponse.json()];
                case 2:
                    result = _a.sent();
                    chat = new TypedViewModels_js_1.Chat();
                    chat.Id = result.id;
                    chat.Name = result.name;
                    chat.ChatLink = result.chatLink;
                    chat.IsTracked = true;
                    chat.Messages = new Array();
                    chat.ButChat = collectivesArea.appendChild((0, SocNetHtmlGen_js_1.CreateGroupChatListItem)(chat, onChatSelected, onMessageSend)).firstElementChild;
                    SocialNetworkModel.Chats.set(chat.Id, chat);
                    return [2 /*return*/];
            }
        });
    });
}
hubConnection.on("MessageRecieved", function (message) {
    if (SocialNetworkModel.Chats.has(message.ChatId)) {
        var chat = SocialNetworkModel.Chats.get(message.ChatId);
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
        var friend = SocialNetworkModel.Friends.get(message.ChatId);
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
        var interlocutor_1 = SocialNetworkModel.Interlocutors.get(message.ChatId);
        if (!interlocutor_1.ChatId) {
            interlocutor_1.ChatId = message.ChatId;
        }
        if (interlocutor_1.IsTracked) {
            interlocutor_1.Messages.push(message);
            if (currentChatId == interlocutor_1.ChatId) {
                onMessageRecieved(message);
            }
        }
        else {
        }
        return;
    }
    for (var _i = 0, _a = SocialNetworkModel.Friends; _i < _a.length; _i++) {
        var item = _a[_i];
        var friend = item[1];
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
    for (var _b = 0, _c = SocialNetworkModel.Interlocutors; _b < _c.length; _b++) {
        var item = _c[_b];
        var interlocutor_2 = item[1];
        if (interlocutor_2.Id == message.SenderId) {
            interlocutor_2.ChatId = message.ChatId;
            if (interlocutor_2.Messages.length == 0)
                interlocutor_2.Messages = new Array();
            interlocutor_2.Messages.push(message);
            interlocutor_2.IsTracked = true;
            if (currentChatId == interlocutor_2.ChatId) {
                onMessageRecieved(message);
            }
            return;
        }
    }
    var interlocutor = new TypedViewModels_js_1.User();
    interlocutor.Id = message.SenderId;
    interlocutor.ChatId = message.ChatId;
    interlocutor.UserName = message.SenderName;
    interlocutor.UserPageLink = message.SenderLink;
    interlocutor.IsTracked = true;
    interlocutor.IsFriend = false;
    var UserListItem = (0, SocNetHtmlGen_js_1.CreateUserListItem)(interlocutor, onChatSelected, onMessageSend);
    interlocutorsArea.appendChild(UserListItem);
    interlocutor.ButUser = UserListItem.firstElementChild;
    SocialNetworkModel.Interlocutors.set(interlocutor.ChatId, interlocutor);
    interlocutor.Messages.push(message);
});
hubConnection.start();
function onMessageRecieved(message) {
    messagesArea.appendChild((0, SocNetHtmlGen_js_1.CreateMessageItem)(message));
    messagesArea.parentElement.scrollTop = messagesArea.parentElement.scrollHeight;
}
function init() {
    return __awaiter(this, void 0, void 0, function () {
        var response, friends_1, chats_1, interlocutors_1, savedChatId;
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0: return [4 /*yield*/, fetch(location.origin + "/SocialNetwork/Data", {
                        method: "GET"
                    })];
                case 1:
                    response = _a.sent();
                    if (!response.ok) return [3 /*break*/, 3];
                    return [4 /*yield*/, response.json()];
                case 2:
                    socNetData = _a.sent();
                    friends_1 = new Map();
                    chats_1 = new Map();
                    interlocutors_1 = new Map();
                    socNetData.friends.forEach(function (friend) {
                        var user = new TypedViewModels_js_1.User();
                        user.Id = friend.id;
                        user.ChatId = friend.chatId;
                        user.UserName = friend.userName;
                        user.UserPageLink = friend.userPageLink;
                        user.IsTracked = false;
                        user.IsFriend = true;
                        var UserListItem = (0, SocNetHtmlGen_js_1.CreateUserListItem)(user, onChatSelected, onMessageSend);
                        comradesArea.appendChild(UserListItem);
                        user.ButUser = UserListItem.firstElementChild;
                        friends_1.set(user.ChatId, user);
                    });
                    socNetData.chats.forEach(function (chat) {
                        var chatObj = new TypedViewModels_js_1.Chat();
                        chatObj.Id = chat.id;
                        chatObj.Name = chat.name;
                        chatObj.IsTracked = false;
                        chatObj.ChatLink = chat.chatLink;
                        var chatListItem = (0, SocNetHtmlGen_js_1.CreateGroupChatListItem)(chatObj, onChatSelected, onMessageSend);
                        collectivesArea.appendChild(chatListItem);
                        chatObj.ButChat = chatListItem.firstElementChild;
                        chats_1.set(chatObj.Id, chatObj);
                    });
                    socNetData.interlocutors.forEach(function (interlocutor) {
                        var user = new TypedViewModels_js_1.User();
                        user.Id = interlocutor.id;
                        user.ChatId = interlocutor.chatId;
                        user.UserName = interlocutor.userName;
                        user.UserPageLink = interlocutor.userPageLink;
                        user.IsTracked = false;
                        user.IsFriend = false;
                        var UserListItem = (0, SocNetHtmlGen_js_1.CreateUserListItem)(user, onChatSelected, onMessageSend);
                        interlocutorsArea.appendChild(UserListItem);
                        user.ButUser = UserListItem.firstElementChild;
                        interlocutors_1.set(user.ChatId, user);
                    });
                    SocialNetworkModel.UserId = socNetData.userId;
                    SocialNetworkModel.Friends = friends_1;
                    SocialNetworkModel.Chats = chats_1;
                    SocialNetworkModel.Interlocutors = interlocutors_1;
                    savedChatId = sessionStorage.getItem("currentChatId");
                    if (savedChatId) {
                        InitialSelectChat(savedChatId);
                    }
                    _a.label = 3;
                case 3: return [2 /*return*/];
            }
        });
    });
}
function InitialSelectChat(chatId) {
    var chatBut = document.getElementById("chat" + chatId);
    if (chatBut) {
        chatBut.parentElement.parentElement.parentElement.parentElement.parentElement.previousElementSibling.firstElementChild.click();
        chatBut.click();
    }
}
window.addEventListener("load", init);
function GetDialogId(userId) {
    return __awaiter(this, void 0, void 0, function () {
        var url, response, chatId;
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0:
                    if (!userId)
                        return [2 /*return*/];
                    url = location.origin + "/SocialNetwork/GetDialogId/" + userId;
                    return [4 /*yield*/, fetch(url, {
                            method: "GET"
                        })];
                case 1:
                    response = _a.sent();
                    if (!response.ok) return [3 /*break*/, 3];
                    return [4 /*yield*/, response.json()];
                case 2:
                    chatId = _a.sent();
                    _a.label = 3;
                case 3: return [2 /*return*/, chatId];
            }
        });
    });
}
function onMessageSend(e, action) {
    return __awaiter(this, void 0, void 0, function () {
        var message, formData, url, response;
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0:
                    if (e.keyCode != 13)
                        return [2 /*return*/];
                    if (e.shiftKey)
                        return [2 /*return*/];
                    e.preventDefault();
                    message = messageTextArea.value;
                    if (message == null || message == "")
                        return [2 /*return*/];
                    if (!currentChatId)
                        return [2 /*return*/];
                    messageTextArea.value = "";
                    formData = new FormData();
                    formData.append("text", message);
                    url = new URL(controller + action + "/" + currentChatId, location.origin);
                    if (action == "SendMessageToInterlocutor") {
                        formData.append("calledUserId", currentChatbut.parentElement.children[2].value);
                    }
                    return [4 /*yield*/, fetch(url, {
                            method: "POST",
                            body: formData
                        })];
                case 1:
                    response = _a.sent();
                    if (response.ok) {
                        console.log("Сообщение отправлено");
                    }
                    messagesArea.parentElement.scrollTop = messagesArea.parentElement.scrollHeight;
                    return [2 /*return*/];
            }
        });
    });
}
function GetChatMessagesFromServer(urlConnect) {
    return __awaiter(this, void 0, void 0, function () {
        var messages, response, chatMessages;
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0: return [4 /*yield*/, fetch(urlConnect, {
                        method: "GET"
                    })];
                case 1:
                    response = _a.sent();
                    if (!response.ok) return [3 /*break*/, 3];
                    return [4 /*yield*/, response.json()];
                case 2:
                    chatMessages = _a.sent();
                    messages = new Array(chatMessages.length);
                    chatMessages.forEach(function (message) {
                        var messageObj = new TypedViewModels_js_1.Message();
                        messageObj.ChatId = message.chatId;
                        messageObj.SenderId = message.senderId;
                        messageObj.Text = message.text;
                        messageObj.SenderName = message.senderName;
                        messageObj.SenderLink = message.senderLink;
                        messageObj.DateTime = message.dateTime;
                        messages.push(messageObj);
                    });
                    return [2 /*return*/, messages];
                case 3: throw "Ошибка запроса";
            }
        });
    });
}
function onChatSelected(e) {
    return __awaiter(this, void 0, void 0, function () {
        var chatId, targetChatBut, actionConnect, messages, urlConnect, User_1, _a, Chat_1;
        return __generator(this, function (_b) {
            switch (_b.label) {
                case 0:
                    messagesArea.innerHTML = "";
                    targetChatBut = e.target;
                    if (currentChatbut) {
                        currentChatbut.classList.remove("network-list-item-selected");
                    }
                    currentChatbut = targetChatBut;
                    currentChatbut.classList.add("network-list-item-selected");
                    if (!targetChatBut.User) return [3 /*break*/, 6];
                    User_1 = targetChatBut.User;
                    if (!!User_1.ChatId) return [3 /*break*/, 2];
                    _a = User_1;
                    return [4 /*yield*/, GetDialogId(User_1.Id)];
                case 1:
                    _a.ChatId = _b.sent();
                    targetChatBut.setAttribute("id", "chat" + chatId);
                    _b.label = 2;
                case 2:
                    chatId = User_1.ChatId;
                    if (chatId == currentChatId && chatId != null)
                        return [2 /*return*/];
                    if (!User_1.IsTracked) return [3 /*break*/, 3];
                    messages = User_1.Messages;
                    return [3 /*break*/, 5];
                case 3:
                    actionConnect = "ConnectToDialog";
                    urlConnect = new URL(controller + actionConnect + "/" + chatId, location.origin);
                    urlConnect.searchParams.set("calledUserId", User_1.Id);
                    return [4 /*yield*/, GetChatMessagesFromServer(urlConnect)];
                case 4:
                    messages = _b.sent();
                    User_1.Messages = messages;
                    User_1.IsTracked = true;
                    _b.label = 5;
                case 5:
                    isCurrentChatADialog = true;
                    _b.label = 6;
                case 6:
                    if (!targetChatBut.Chat) return [3 /*break*/, 10];
                    Chat_1 = targetChatBut.Chat;
                    chatId = Chat_1.Id;
                    if (chatId == currentChatId && chatId != null)
                        return [2 /*return*/];
                    if (!Chat_1.IsTracked) return [3 /*break*/, 7];
                    messages = Chat_1.Messages;
                    return [3 /*break*/, 9];
                case 7:
                    actionConnect = "ConnectToChat";
                    urlConnect = new URL(controller + actionConnect + "/" + chatId, location.origin);
                    urlConnect.searchParams.set("connectionId", hubConnection.connectionId);
                    return [4 /*yield*/, GetChatMessagesFromServer(urlConnect)];
                case 8:
                    messages = _b.sent();
                    Chat_1.Messages = messages;
                    Chat_1.IsTracked = true;
                    _b.label = 9;
                case 9:
                    isCurrentChatADialog = false;
                    _b.label = 10;
                case 10:
                    currentChatId = chatId;
                    currentChatMessages = messages;
                    sessionStorage.setItem("currentChatId", currentChatId);
                    if (messages) {
                        messages.forEach(function (message) {
                            messagesArea.appendChild((0, SocNetHtmlGen_js_1.CreateMessageItem)(message));
                        });
                    }
                    messagesArea.parentElement.scrollTop = messagesArea.parentElement.scrollHeight;
                    return [2 /*return*/];
            }
        });
    });
}
document.getElementById("applyFilter").addEventListener("click", function (e) { return __awaiter(void 0, void 0, void 0, function () {
    var controller, formData, urlConnect, response, filteredUsersWrap, result, downloadedPartialView, partialView;
    return __generator(this, function (_a) {
        switch (_a.label) {
            case 0:
                e.preventDefault();
                controller = "/SocialNetwork/";
                formData = new FormData(document.forms.namedItem("filterForm"));
                urlConnect = location.origin + controller + "FilterUsers";
                return [4 /*yield*/, fetch(urlConnect, {
                        method: "POST",
                        body: formData
                    })];
            case 1:
                response = _a.sent();
                filteredUsersWrap = document.getElementById("filteredUsersWrap");
                return [4 /*yield*/, response.text()];
            case 2:
                result = _a.sent();
                filteredUsersWrap.innerHTML = "";
                downloadedPartialView = new DOMParser().parseFromString(result, "text/html").getElementById("filteredUsers");
                if (downloadedPartialView) {
                    partialView = downloadedPartialView;
                    filteredUsersWrap.appendChild(partialView);
                }
                document.querySelector('#resetSearch').onclick = function (e) {
                    filteredUsersWrap.innerHTML = "";
                    document.forms.namedItem("filterForm").reset();
                };
                document.querySelector("#butSearchUsers").click();
                return [2 /*return*/];
        }
    });
}); });
function setListHeight() {
    var maxListSize = comradesHeader.parentElement.parentElement.clientHeight
        - (comradesHeader.firstElementChild.clientHeight
            + collectivesHeader.firstElementChild.clientHeight
            + interlocutorsHeader.firstElementChild.clientHeight) - 28;
    comradesArea.setAttribute("style", "height:" + maxListSize.toString() + "px");
    collectivesArea.setAttribute("style", "height:" + maxListSize.toString() + "px");
    interlocutorsArea.setAttribute("style", "height:" + maxListSize.toString() + "px");
}
window.onresize = function (e) {
    setListHeight();
};
