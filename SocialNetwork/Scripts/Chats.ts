
var chatsBlock: HTMLDivElement = document.getElementById("chatsBlock") as HTMLDivElement;
var userChats: HTMLDivElement = document.getElementById("userChats") as HTMLDivElement;
var searchInput: HTMLInputElement = document.getElementById("searchInput") as HTMLInputElement;
var filteredChatsWrap: HTMLDivElement = document.getElementById("filteredChatsWrap") as HTMLDivElement;

let chatName: string;
let oldChatName: string;
var locked: boolean;
var isFilterModified: boolean;

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

    let createChatResponse = await fetch(createChatUrl, {
        method: "POST",
        body: formData
    });

    let result = await createChatResponse.json();

    let chatLink = new URL(result.chatLink, location.origin);
    let chatLinkElement: HTMLAnchorElement = document.createElement("a");
    chatLinkElement.href = chatLink.href;
    chatLinkElement.text = result.name;
    chatLinkElement.classList.add("user-link");
    userChats.appendChild(chatLinkElement);

}

searchInput.onkeyup = e => {
    search();
}

async function search() {
    
    chatName = searchInput.value;
    if (oldChatName) {
        if (oldChatName == chatName) {
            return;
        }
    }
    oldChatName = chatName;
    if (locked) {
        isFilterModified = true;
        return;
    }
    locked = true;

    filteredChatsWrap.innerHTML = "";
    if (chatName == null || chatName == "") {
        userChats.hidden = false;
        isFilterModified = false;
        locked = false;
        return;
    }

    let searchUrl = new URL("/SocialNetwork/FilterChats", location.origin);
    searchUrl.searchParams.set("chatName", searchInput.value);
    let searchResponse = await fetch(searchUrl, {
        method: "Get"
    });
   
    let result = await searchResponse.text();

    let filteredChatsPart = new DOMParser().parseFromString(result, "text/html").getElementById("filteredChats") as HTMLDivElement;
    if (filteredChatsPart) {
        userChats.hidden = true;
        filteredChatsWrap.appendChild(filteredChatsPart);
    }

    locked = false;
    if (!isFilterModified) {
        return;
    }
    isFilterModified = false;
    search();
}