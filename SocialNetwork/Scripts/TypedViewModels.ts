export class User {
    Id: string;
    ChatId: number;
    UserName: string;
    UserPageLink: string;
    ButUser: HTMLButtonElement;
    IsFriend: boolean;
    IsTracked: boolean;
    Messages: Array<Message>;
}

export class Message {
    SenderId: string;
    ChatId: number;
    SenderName: string;
    SenderLink: string;
    Text: string;
    DateTime: string;
}

export class Chat {
    Id: number;
    Name: string;
    ChatLink: string;
    ButChat: HTMLButtonElement;
    IsTracked: boolean;
    Messages: Array<Message>;
}

export class SocialNetwork {
    UserId: string;
    Chats: Map<number, Chat>;
    Friends: Map<number, User>;
    Interlocutors: Map<number, User>;
}