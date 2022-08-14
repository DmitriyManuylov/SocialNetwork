import { MessagePackHubProtocol } from "../lib/microsoft/signalr-protocol-msgpack/dist/esm/MessagePackHubProtocol.js";
import signalR = require("../lib/microsoft/signalr/dist/esm");
import * as ViewModels from "./ViewModels";
import * as HTMLGeneration from "./SocNetHtmlGen";
var access_token: string;
var hubConnection = new signalR.HubConnectionBuilder().withUrl("/SocialNetwork").withHubProtocol(new MessagePackHubProtocol()).build();
hubConnection.on("MessageRecieved", onMessageRecieved)

function onMessageRecieved(message: ViewModels.Message) {
    messagesArea.appendChild(HTMLGeneration.CreateMessageItem(message));
}
var comradesArea: HTMLDivElement = document.getElementById("comradesArea") as HTMLDivElement;
var collectivesArea: HTMLDivElement = document.getElementById("collectivesArea") as HTMLDivElement;
var messagesArea: HTMLDivElement = document.querySelector("#messagesArea");

window.addEventListener("load", init);

var currentChatbut: HTMLButtonElement;
var currentChatId;
var socNetData: ViewModels.SocialNetwork;
var currentChatMessages: Array<ViewModels.Message>;
var messageInput: HTMLTextAreaElement = document.querySelector("#messageInput");
messageInput.addEventListener("keypress", onMessageSend);

async function onMessageSend(e: KeyboardEvent) {
    if (e.keyCode != 13) return;
    if (e.shiftKey) return;
    e.preventDefault();
    var message: string = messageInput.value;
    if (message == null || message == "") return;
    if (currentChatId == undefined) return

    var formData = new FormData();
    formData.append("chatId", currentChatId)
    formData.append("text", message);
    var response = await fetch("/SocialNetwork/SendMessage", {
        method: "POST",
        body: formData
    });
    if (response.ok) {
        console.log("Сообщение отправлено");
    }
}

async function init() {
    var request: XMLHttpRequest = new XMLHttpRequest();
    request.open("GET", "/SocialNetwork/Data");
    var response = await fetch("SocialNetwork/Data", {
        method: "GET"
    });
    if (response.ok) {
        socNetData = await response.json();
        socNetData.Friends.forEach(friend => comradesArea.appendChild(HTMLGeneration.CreateUserListItem(friend, onChatSelected)));
        socNetData.Chats.forEach(chat => collectivesArea.appendChild(HTMLGeneration.CreateGroupChatListItem(chat, onChatSelected)));
    }
}

async function onChatSelected(e: MouseEvent) {
    if (currentChatbut)
        currentChatbut.classList.remove("network-list-item-selected");
    currentChatbut = e.target as HTMLButtonElement;
    currentChatbut.classList.add("network-list-item-selected");
    var formData = new FormData();
    var hiddenChatId = currentChatbut.nextSibling as HTMLInputElement;
    currentChatId = hiddenChatId.value;
    formData.append("chatId", currentChatId);
    var response = await fetch("/SocialNetwork/ConnectToChat", {
        method: "GET",
        body: formData
    })
    if (response.ok) {
        currentChatMessages = await response.json();
        currentChatMessages.forEach(message => messagesArea.appendChild(HTMLGeneration.CreateMessageItem(message)));
    }
}

