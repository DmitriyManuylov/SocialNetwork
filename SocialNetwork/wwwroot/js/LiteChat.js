
import { CreateMessageItem, AddRoomToList } from "./HtmlGeneration.js";

var roomsList = document.getElementById("roomsList");
var userNameDialog = document.getElementById("userNameDialog");
var senderNameElement = userNameDialog.children[1];
var butSenderName = userNameDialog.children[2];
var createRoomDialog = document.getElementById("createRoomDialog");
var roomNameElement = createRoomDialog.children[1];
var buttonCreate = createRoomDialog.children[2];
var butShowCreateRoomDialog = document.getElementById("butCreateRoom");
var textArea = document.getElementById("messageInput").children[0];
var messagesArea = document.getElementById("messagesArea");

var currentRoom;
var currentRoomId;
var rooms;
var currentRoomMessages;
var userName;

var hubConnection = new signalR.HubConnectionBuilder().withUrl("/chat").withHubProtocol(new signalR.protocols.msgpack.MessagePackHubProtocol()).build();

hubConnection.on("Recieve", function (simpleMessage) {
    //if (simpleMessage.RoomId != currentRoomId) return;
    CreateMessageItem(simpleMessage.Id, simpleMessage.Sender, simpleMessage.Text, simpleMessage.DateTime);
});
hubConnection.on("GroupNotify", function (message, chatId, dateTime) {
   // if (chatId != currentRoomId) return;
    CreateMessageItem("", "System", message, dateTime)
})
hubConnection.on("LiteChatRoomCreated", onLiteChatRoomCreated);
function onLiteChatRoomCreated(room) {
    var rroom = {
        id: room.Id,
        name: room.Name
    }
    rooms.push(room);
    AddRoomToList(rroom, onRoomSelect);
}

window.onclose = () => { hubConnection.stop(); };
hubConnection.onreconnected(console.log("реконнект"));
hubConnection.onclose(console.log("Отрубилось подключение"));
hubConnection.start();

function FillRoomsList(rooms) {
    rooms.forEach(room => AddRoomToList(room, onRoomSelect));
}

window.addEventListener("load", init);

function init(e) {
    var request = new XMLHttpRequest();
    request.open("GET", "/Chat/Index");
    request.onload = () => {
        if (request.status == 200) {
            rooms = JSON.parse(request.response);
            
            if (rooms.length != 0) {
                FillRoomsList(rooms);
                InitialSelectRoom();
            }
        }
    };
    request.send();

}

function onRoomSelect(e) {
    if (userName == "" || userName == undefined) {
        userNameDialog.showModal();
        return;
    }
    var connectionId = hubConnection.connectionId;
    var roomId;
    if (currentRoom) {
        roomId = parseInt(currentRoom.nextSibling.value);
        currentRoom.classList.remove("network-list-item-selected");
        var exitRequest = new XMLHttpRequest();
        exitRequest.open("POST", "Chat/ExitFromGroup");
        let exitFormData = new FormData();
        exitFormData.append("roomId", roomId);
        exitFormData.append("userName", userName);
        exitFormData.append("connectionId", connectionId);
        exitRequest.send(exitFormData);
    }

    currentRoom = e.target;
    currentRoomId = parseInt(currentRoom.nextSibling.value);
    currentRoom.classList.add("network-list-item-selected")
    roomId = parseInt(currentRoom.nextSibling.value);

    messagesArea.innerHTML = '';
    
    var request = new XMLHttpRequest();

    var joinFormData = new FormData();
    joinFormData.append("roomId", roomId);
    joinFormData.append("userName", userName);
    joinFormData.append("connectionId", connectionId);
    request.open("POST", "/Chat/JoinToGroup");


    request.onload = (e) => {
        if (request.status == 200) {
            currentRoomMessages = JSON.parse(request.response);

            currentRoomMessages.forEach(message =>
                CreateMessageItem(message.id, message.sender, message.text, message.dateTime));
        }
    };
    request.send(joinFormData);

}


function ShowCreateRoomDialog(e) {
    createRoomDialog.showModal();
}

butShowCreateRoomDialog.addEventListener("click", ShowCreateRoomDialog);
buttonCreate.addEventListener("click", (e) => {
    createRoomDialog.close();
    var roomName = roomNameElement.value;
    if (roomName == "") {
        return;
    }
    var xhr = new XMLHttpRequest();
    var formData = new FormData();
    formData.append("roomName", roomName);

    xhr.open("POST", "/Chat/CreateRoom");
    xhr.onload = (e) => {
        if (xhr.status == 200) { alert("Комната создана"); }
    };

    xhr.send(formData);
});

function getData() { }

function onButSendMessage(e) {
    if (e.shiftKey) return;
    if (e.keyCode !== 13) return;
    e.preventDefault();

    var textAreaContent = textArea.value;

    if (textAreaContent.length == "") return;
    if (userName == "" || userName == undefined) {
        userNameDialog.showModal();
        return;
    }
    SendMessage(textAreaContent);
    textArea.value = '';
}
function SendMessage(message) {
    let xhr = new XMLHttpRequest();
    xhr.open("POST", "/Chat/Send");


    var formData = new FormData();
    formData.append("roomId", currentRoomId);
    formData.append("Sender", userName);
    formData.append("Text", message);
    xhr.onload = () => {
        if (xhr.status == 200)
            console.log("Сообщение отправлено");
        else
            console.log("Сообщение не отправлено");
    };
    xhr.send(formData);
}
butSenderName.addEventListener("click", () => {
    userName = senderNameElement.value;
    userNameDialog.close();
});
textArea.addEventListener("keypress", onButSendMessage);


