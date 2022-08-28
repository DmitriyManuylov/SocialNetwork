const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]');
const tooltipList = [...tooltipTriggerList].map(tooltipTriggerEl => new bootstrap.Tooltip(tooltipTriggerEl));
var hubConnection = new signalR.HubConnectionBuilder().withUrl("/SocialNetwork").withHubProtocol(new signalR.protocols.msgpack.MessagePackHubProtocol()).build();

hubConnection.on("MessageRecieved", onMessageRecievedHandler);
hubConnection.on("FriendshipRequested", onFriendshipRequestedHandler);
hubConnection.on("FriendshopAccepted", onFriendshipAcceptedHandler);


function onMessageRecievedHandler(message) {

}

function onFriendshipRequestedHandler(user){

}

function onFriendshipAcceptedHandler(user) {

}


