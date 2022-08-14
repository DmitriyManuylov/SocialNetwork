export class User {
    Id: string;
    UserName: string;
    UserPageLink: string;
}

export class Message {
    SenderId: number;
    SenderName: string;
    SenderLink: string;
    Text: string;
    DateTime: string;
}

export class Chat {
    Id: number;
    Name: string;
}

export class SocialNetwork {
    Chats: Array<Chat>;
    Friends: Array<User>;
    IncomingFriendshipInvitations: Array<User>;
    OutgoingFriendshipInvitations: Array<User>;
}