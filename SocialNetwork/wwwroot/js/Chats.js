var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
var chatsBlock = document.getElementById("chatsBlock");
var userChats = document.getElementById("userChats");
var searchInput = document.getElementById("searchInput");
var filteredChatsWrap = document.getElementById("filteredChatsWrap");
let chatName;
let oldChatName;
var locked;
var isFilterModified;
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
        let chatLink = new URL(result.chatLink, location.origin);
        let chatLinkElement = document.createElement("a");
        chatLinkElement.href = chatLink.href;
        chatLinkElement.text = result.name;
        chatLinkElement.classList.add("user-link");
        userChats.appendChild(chatLinkElement);
    });
}
searchInput.onkeyup = e => {
    search();
};
function search() {
    return __awaiter(this, void 0, void 0, function* () {
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
        let searchResponse = yield fetch(searchUrl, {
            method: "Get"
        });
        let result = yield searchResponse.text();
        let filteredChatsPart = new DOMParser().parseFromString(result, "text/html").getElementById("filteredChats");
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
    });
}
//# sourceMappingURL=Chats.js.map