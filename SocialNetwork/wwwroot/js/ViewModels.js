export class User {
    Id;
    UserName;
    UserPageLink;
}

export class Message {
    SenderId;
    SenderName;
    SenderLink;
    Text;
    DateTime;
}

export class Chat {
    Id;
    Name;
}

export class SocialNetwork {
    Chats;
    Friends;
    IncomingFriendshipInvitations;
    OutgoingFriendshipInvitations;
}