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
var chatsBlock = document.getElementById("chatsBlock");
var userChats = document.getElementById("userChats");
var searchInput = document.getElementById("searchInput");
var filteredChatsWrap = document.getElementById("filteredChatsWrap");
var chatName;
var oldChatName;
var locked;
var isFilterModified;
var butShowCreationChatDialog = document.querySelector("#butShowCreationChatDialog");
var butCreateChat = document.querySelector("#butCreateChat");
butCreateChat.onclick = CreateChat;
function CreateChat() {
    return __awaiter(this, void 0, void 0, function () {
        var chatNameElement, chatName, createChatUrl, formData, createChatResponse, result, chatLink, chatLinkElement;
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
                    chatLink = new URL(result.chatLink, location.origin);
                    chatLinkElement = document.createElement("a");
                    chatLinkElement.href = chatLink.href;
                    chatLinkElement.text = result.name;
                    chatLinkElement.classList.add("user-link");
                    userChats.appendChild(chatLinkElement);
                    return [2 /*return*/];
            }
        });
    });
}
searchInput.onkeyup = function (e) {
    search();
};
function search() {
    return __awaiter(this, void 0, void 0, function () {
        var searchUrl, searchResponse, result, filteredChatsPart;
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0:
                    chatName = searchInput.value;
                    if (oldChatName) {
                        if (oldChatName == chatName) {
                            return [2 /*return*/];
                        }
                    }
                    oldChatName = chatName;
                    if (locked) {
                        isFilterModified = true;
                        return [2 /*return*/];
                    }
                    locked = true;
                    filteredChatsWrap.innerHTML = "";
                    if (chatName == null || chatName == "") {
                        userChats.hidden = false;
                        isFilterModified = false;
                        locked = false;
                        return [2 /*return*/];
                    }
                    searchUrl = new URL("/SocialNetwork/FilterChats", location.origin);
                    searchUrl.searchParams.set("chatName", searchInput.value);
                    return [4 /*yield*/, fetch(searchUrl, {
                            method: "Get"
                        })];
                case 1:
                    searchResponse = _a.sent();
                    return [4 /*yield*/, searchResponse.text()];
                case 2:
                    result = _a.sent();
                    filteredChatsPart = new DOMParser().parseFromString(result, "text/html").getElementById("filteredChats");
                    if (filteredChatsPart) {
                        userChats.hidden = true;
                        filteredChatsWrap.appendChild(filteredChatsPart);
                    }
                    locked = false;
                    if (!isFilterModified) {
                        return [2 /*return*/];
                    }
                    isFilterModified = false;
                    search();
                    return [2 /*return*/];
            }
        });
    });
}
