export class User {
    Id;
    ChatId;
    UserName;
    UserPageLink;
    ButUser;
    IsFriend;
    IsTracked;
    Messages;
}

export class Message {
    SenderId;
    ChatId;
    SenderName;
    SenderLink;
    Text;
    DateTime;
}

export class Chat {
    Id;
    Name;
    ChatLink;
    ButChat;
    IsTracked;
    Messages;
}

export class SocialNetwork {
    Chats;
    Friends;
    Interlocutors;
}