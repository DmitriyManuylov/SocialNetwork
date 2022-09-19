"use strict";
//import * as signalR from "@microsoft/signalr";
//import { MessagePackHubProtocol } from "@microsoft/signalr-protocol-msgpack";
//var hubConnection = new signalR.HubConnectionBuilder().withUrl("/SocialNetwork").withHubProtocol(new MessagePackHubProtocol()).build();
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
var hubConnection = new signalR.HubConnectionBuilder().withUrl("/SocialNetwork").withAutomaticReconnect().withHubProtocol(new signalR.protocols.msgpack.MessagePackHubProtocol()).build();
hubConnection.on("MessageRecieved", onMessageRecievedHandler);
hubConnection.on("FriendshipRequested", onFriendshipRequestedHandler);
hubConnection.on("FriendshipAccepted", onFriendshipAcceptedHandler);
hubConnection.on("FriendshipRejected", onFriendshipRejectedHandler);
hubConnection.on("FriendshipInvitationCanceled", onFriendshipCanceledHandler);
hubConnection.on("DeletedByUserFromFriends", onDeletedByUserFromFriends);
hubConnection.start();
var butInvite;
var butAccept;
var butReject;
var butCancel;
var butDelete;
var sendMessageBut = document.getElementById("SendMessageBut");
var userId = document.getElementById("User_Id").value;
var divFriendshipBlockBlock = document.getElementById("friendshipBlock");
function onMessageRecievedHandler(message) {
}
function setStateIncomingFriendshipInvitation() {
    var _this = this;
    divFriendshipBlockBlock.innerHTML =
        "<button class=\"btn btn-outline-secondary\" id=\"butAccept\">Принять приглашение</button>" +
            "<button class=\"btn btn-outline-secondary\" id = \"butReject\" > Отклонить приглашение </button>";
    butAccept = document.getElementById("butAccept");
    butReject = document.getElementById("butReject");
    butAccept.onclick = function (e) { return __awaiter(_this, void 0, void 0, function () {
        return __generator(this, function (_a) {
            acceptFriendship();
            return [2 /*return*/];
        });
    }); };
    butReject.onclick = function (e) { return __awaiter(_this, void 0, void 0, function () {
        return __generator(this, function (_a) {
            rejectFriendship();
            return [2 /*return*/];
        });
    }); };
}
function setStateOutgoingFriendshipInvitation() {
    var _this = this;
    divFriendshipBlockBlock.innerHTML =
        "<label>Отправлено приглашение</label>" +
            "<button class=\"btn btn-outline-secondary\" id=\"butCancel\">Отозвать приглашение</button>";
    butCancel = document.getElementById("butCancel");
    butCancel.onclick = function (e) { return __awaiter(_this, void 0, void 0, function () {
        return __generator(this, function (_a) {
            cancelFriendshipInvitation();
            return [2 /*return*/];
        });
    }); };
}
function setStateNotFriends() {
    var _this = this;
    divFriendshipBlockBlock.innerHTML = "<button class=\"btn btn-outline-secondary\" id=\"butInvite\">Пригласить</button>";
    butInvite = document.getElementById("butInvite");
    butInvite.onclick = function (e) { return __awaiter(_this, void 0, void 0, function () {
        return __generator(this, function (_a) {
            inviteUser();
            return [2 /*return*/];
        });
    }); };
}
function setStateUsersInFriendship() {
    var _this = this;
    divFriendshipBlockBlock.innerHTML =
        "<label>Вы состоите в дружбе</label>" +
            "<button class=\"btn btn-outline-secondary\" id=\"butDelete\">Удалить из друзей</button>";
    butDelete = document.getElementById("butDelete");
    butDelete.onclick = function (e) { return __awaiter(_this, void 0, void 0, function () {
        return __generator(this, function (_a) {
            deleteUserFromFriends();
            return [2 /*return*/];
        });
    }); };
}
function onFriendshipRequestedHandler(user) {
    setStateIncomingFriendshipInvitation();
}
function onFriendshipAcceptedHandler(user) {
    setStateUsersInFriendship();
}
function onFriendshipRejectedHandler(user) {
    setStateNotFriends();
}
function onFriendshipCanceledHandler(user) {
    setStateNotFriends();
}
function onDeletedByUserFromFriends(user) {
    setStateNotFriends();
}
butInvite = document.getElementById("butInvite");
butAccept = document.getElementById("butAccept");
butReject = document.getElementById("butReject");
butCancel = document.getElementById("butCancel");
butDelete = document.getElementById("butDelete");
if (butInvite) {
    butInvite.onclick = function (e) { return __awaiter(void 0, void 0, void 0, function () {
        return __generator(this, function (_a) {
            inviteUser();
            return [2 /*return*/];
        });
    }); };
}
if (butAccept) {
    butAccept.onclick = function (e) { return __awaiter(void 0, void 0, void 0, function () {
        return __generator(this, function (_a) {
            acceptFriendship();
            return [2 /*return*/];
        });
    }); };
}
if (butReject) {
    butReject.onclick = function (e) { return __awaiter(void 0, void 0, void 0, function () {
        return __generator(this, function (_a) {
            rejectFriendship();
            return [2 /*return*/];
        });
    }); };
}
if (butCancel) {
    butCancel.onclick = function (e) { return __awaiter(void 0, void 0, void 0, function () {
        return __generator(this, function (_a) {
            cancelFriendshipInvitation();
            return [2 /*return*/];
        });
    }); };
}
if (butDelete) {
    butDelete.onclick = function (e) { return __awaiter(void 0, void 0, void 0, function () {
        return __generator(this, function (_a) {
            deleteUserFromFriends();
            return [2 /*return*/];
        });
    }); };
}
if (sendMessageBut) {
    sendMessageBut.onclick = function (e) { return __awaiter(void 0, void 0, void 0, function () {
        return __generator(this, function (_a) {
            sendMessage();
            return [2 /*return*/];
        });
    }); };
}
function sendMessage() {
    return __awaiter(this, void 0, void 0, function () {
        var chatId, urlGetDialogId, getDialogIdResponse, messengerUrl;
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0:
                    urlGetDialogId = new URL("/SocialNetwork/GetDialogId/".concat(userId), location.origin);
                    return [4 /*yield*/, fetch(urlGetDialogId, {
                            method: "GET"
                        })];
                case 1:
                    getDialogIdResponse = _a.sent();
                    return [4 /*yield*/, getDialogIdResponse.json()];
                case 2:
                    chatId = _a.sent();
                    sessionStorage.setItem("currentChatId", chatId);
                    messengerUrl = new URL("/Messenger", location.origin);
                    window.open(messengerUrl, "_self");
                    return [2 /*return*/];
            }
        });
    });
}
function inviteUser() {
    return __awaiter(this, void 0, void 0, function () {
        var urlInviteFriend, invitationResponse;
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0:
                    urlInviteFriend = new URL("/SocialNetwork/InviteFriend/".concat(userId), location.origin);
                    return [4 /*yield*/, fetch(urlInviteFriend, {
                            method: "POST"
                        })];
                case 1:
                    invitationResponse = _a.sent();
                    if (invitationResponse.ok) {
                        setStateOutgoingFriendshipInvitation();
                    }
                    return [2 /*return*/];
            }
        });
    });
}
function acceptFriendship() {
    return __awaiter(this, void 0, void 0, function () {
        var urlAcceptFriend, invitationAcceptResponse;
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0:
                    urlAcceptFriend = new URL("/SocialNetwork/AcceptFriendship/".concat(userId), location.origin);
                    return [4 /*yield*/, fetch(urlAcceptFriend, {
                            method: "POST"
                        })];
                case 1:
                    invitationAcceptResponse = _a.sent();
                    if (invitationAcceptResponse.ok) {
                        setStateUsersInFriendship();
                    }
                    return [2 /*return*/];
            }
        });
    });
}
function rejectFriendship() {
    return __awaiter(this, void 0, void 0, function () {
        var urlRejectFriendship, rejectFriendshipResponse;
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0:
                    urlRejectFriendship = new URL("/SocialNetwork/RejectFriendship/".concat(userId), location.origin);
                    return [4 /*yield*/, fetch(urlRejectFriendship, {
                            method: "POST"
                        })];
                case 1:
                    rejectFriendshipResponse = _a.sent();
                    if (rejectFriendshipResponse.ok) {
                        setStateNotFriends();
                    }
                    return [2 /*return*/];
            }
        });
    });
}
function cancelFriendshipInvitation() {
    return __awaiter(this, void 0, void 0, function () {
        var urlCancelFriendshipInvitation, cancelFriendshipInvitationResponse;
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0:
                    urlCancelFriendshipInvitation = new URL("/SocialNetwork/CancelFriendshipInvitation/".concat(userId), location.origin);
                    return [4 /*yield*/, fetch(urlCancelFriendshipInvitation, {
                            method: "POST"
                        })];
                case 1:
                    cancelFriendshipInvitationResponse = _a.sent();
                    if (cancelFriendshipInvitationResponse.ok) {
                        setStateNotFriends();
                    }
                    return [2 /*return*/];
            }
        });
    });
}
function deleteUserFromFriends() {
    return __awaiter(this, void 0, void 0, function () {
        var urlDeleteUserFromFriends, deleteUserFromFriendsResponse;
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0:
                    urlDeleteUserFromFriends = new URL("/SocialNetwork/DeleteUserFromFriends/".concat(userId), location.origin);
                    return [4 /*yield*/, fetch(urlDeleteUserFromFriends, {
                            method: "POST"
                        })];
                case 1:
                    deleteUserFromFriendsResponse = _a.sent();
                    if (deleteUserFromFriendsResponse.ok) {
                        setStateNotFriends();
                    }
                    return [2 /*return*/];
            }
        });
    });
}
