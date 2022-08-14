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
Object.defineProperty(exports, "__esModule", { value: true });
var MessagePackHubProtocol_js_1 = require("../lib/microsoft/signalr-protocol-msgpack/dist/esm/MessagePackHubProtocol.js");
var signalR = require("../lib/microsoft/signalr/dist/esm");
var HTMLGeneration = require("./SocNetHtmlGen");
var access_token;
var hubConnection = new signalR.HubConnectionBuilder().withUrl("/SocialNetwork").withHubProtocol(new MessagePackHubProtocol_js_1.MessagePackHubProtocol()).build();
hubConnection.on("MessageRecieved", onMessageRecieved);
function onMessageRecieved(message) {
    messagesArea.appendChild(HTMLGeneration.CreateMessageItem(message));
}
var comradesArea = document.getElementById("comradesArea");
var collectivesArea = document.getElementById("collectivesArea");
var messagesArea = document.querySelector("#messagesArea");
window.addEventListener("load", init);
var currentChatbut;
var currentChatId;
var socNetData;
var currentChatMessages;
var messageInput = document.querySelector("#messageInput");
messageInput.addEventListener("keypress", onMessageSend);
function onMessageSend(e) {
    return __awaiter(this, void 0, void 0, function () {
        var message, formData, response;
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0:
                    if (e.keyCode != 13)
                        return [2 /*return*/];
                    if (e.shiftKey)
                        return [2 /*return*/];
                    e.preventDefault();
                    message = messageInput.value;
                    if (message == null || message == "")
                        return [2 /*return*/];
                    if (currentChatId == undefined)
                        return [2 /*return*/];
                    formData = new FormData();
                    formData.append("chatId", currentChatId);
                    formData.append("text", message);
                    return [4 /*yield*/, fetch("/SocialNetwork/SendMessage", {
                            method: "POST",
                            body: formData
                        })];
                case 1:
                    response = _a.sent();
                    if (response.ok) {
                        console.log("Сообщение отправлено");
                    }
                    return [2 /*return*/];
            }
        });
    });
}
function init() {
    return __awaiter(this, void 0, void 0, function () {
        var request, response;
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0:
                    request = new XMLHttpRequest();
                    request.open("GET", "/SocialNetwork/Data");
                    return [4 /*yield*/, fetch("SocialNetwork/Data", {
                            method: "GET"
                        })];
                case 1:
                    response = _a.sent();
                    if (!response.ok) return [3 /*break*/, 3];
                    return [4 /*yield*/, response.json()];
                case 2:
                    socNetData = _a.sent();
                    socNetData.Friends.forEach(function (friend) { return comradesArea.appendChild(HTMLGeneration.CreateUserListItem(friend, onChatSelected)); });
                    socNetData.Chats.forEach(function (chat) { return collectivesArea.appendChild(HTMLGeneration.CreateGroupChatListItem(chat, onChatSelected)); });
                    _a.label = 3;
                case 3: return [2 /*return*/];
            }
        });
    });
}
function onChatSelected(e) {
    return __awaiter(this, void 0, void 0, function () {
        var formData, hiddenChatId, response;
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0:
                    if (currentChatbut)
                        currentChatbut.classList.remove("network-list-item-selected");
                    currentChatbut = e.target;
                    currentChatbut.classList.add("network-list-item-selected");
                    formData = new FormData();
                    hiddenChatId = currentChatbut.nextSibling;
                    currentChatId = hiddenChatId.value;
                    formData.append("chatId", currentChatId);
                    return [4 /*yield*/, fetch("/SocialNetwork/ConnectToChat", {
                            method: "GET",
                            body: formData
                        })];
                case 1:
                    response = _a.sent();
                    if (!response.ok) return [3 /*break*/, 3];
                    return [4 /*yield*/, response.json()];
                case 2:
                    currentChatMessages = _a.sent();
                    currentChatMessages.forEach(function (message) { return messagesArea.appendChild(HTMLGeneration.CreateMessageItem(message)); });
                    _a.label = 3;
                case 3: return [2 /*return*/];
            }
        });
    });
}
//# sourceMappingURL=SocialNetwork.js.map