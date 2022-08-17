using Microsoft.EntityFrameworkCore;
using SocialNetwork.Models.ChatModels;
using SocialNetwork.Models.Exceptions;
using SocialNetwork.Models.UserInfoModels;
using SocialNetwork.Models.ViewModels.SocialNetworkViewModels;
using System.Collections.Generic;
using System.Linq;

namespace SocialNetwork.Models.Repositories
{
    public class EFSocialNetworkRepository: ISocialNetworkRepository
    {
        private SocialNetworkDbContext _dbContext;
        public EFSocialNetworkRepository(SocialNetworkDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<ChatViewModel> AllChatsViewModel => _dbContext.Chats.Select(chat => new ChatViewModel()
        {
            Id = chat.Id,
            Name = chat.Name,
        }).ToList();

        public List<ChatViewModel> GetUserChats(string userId)
        {
            IQueryable<ChatViewModel> chats = from chat in _dbContext.Chats
                                              join mic in _dbContext.MembershipInChats on chat.Id equals mic.ChatId
                                              where mic.UserId == userId && chat.Name != ""
                                              select new ChatViewModel
                                              {
                                                  Id = chat.Id,
                                                  Name = chat.Name
                                              };
            return chats.ToList();
        }

        public GroupChat GetChatById(int chatId)
        {
            GroupChat chat = _dbContext.Chats.FirstOrDefault(chat => chat.Id == chatId);
            if (chat == null) throw new ChatException("Чат с таким Id не существует");
            return chat;
        }

        public GroupChat GetUsersDialog(string userId, string interlocutorId)
        {
            NetworkUser user = _dbContext.Users.FirstOrDefault(user => user.Id == userId);
            if (user == null) throw new NetworkUserException("Запрашивающий пользователь не существует");
            NetworkUser interlocutor = _dbContext.Users.FirstOrDefault(user => user.Id == interlocutorId);
            if (user == null) throw new NetworkUserException("Собеседник пользователя не существует");

            var chat = _dbContext.MembershipInChats.Where(mic => mic.UserId == user.Id)
                                                        .Join(_dbContext.Chats,
                                                              mic => mic.ChatId,
                                                              chat => chat.Id,
                                                              (mic, chat) => chat)
                                                        .Join(_dbContext.MembershipInChats,
                                                              chat => chat.Id,
                                                              mic => mic.ChatId,
                                                              (chat, mic) => new { chat, mic.UserId })
                                                        .Where(a => a.chat.Name == "" && a.UserId == interlocutor.Id).Select(a => a.chat).FirstOrDefault();
            return chat;
        }
        public GroupChat EnsureGetUsersDialog(string userId, string interlocutorId)
        {
            NetworkUser user = _dbContext.Users.FirstOrDefault(user => user.Id == userId);
            if (user == null) throw new NetworkUserException("Запрашивающий пользователь не существует");
            NetworkUser interlocutor = _dbContext.Users.FirstOrDefault(user => user.Id == interlocutorId);
            if (user == null) throw new NetworkUserException("Собеседник пользователя не существует");

            var chat = _dbContext.MembershipInChats.Where(mic => mic.UserId == user.Id)
                                                        .Join(_dbContext.Chats,
                                                              mic => mic.ChatId,
                                                              chat => chat.Id,
                                                              (mic, chat) => chat)
                                                        .Join(_dbContext.MembershipInChats,
                                                              chat => chat.Id,
                                                              mic => mic.ChatId,
                                                              (chat, mic) => new { chat, mic.UserId })
                                                        .Where(a => a.chat.Name == "" && a.UserId == interlocutor.Id).Select(a => a.chat).FirstOrDefault();
            
            if (chat == null) chat = CreateUsersDialog(user, interlocutor);
            return chat;
        }
        private GroupChat CreateUsersDialog(NetworkUser user, NetworkUser interlocutor)
        {
            GroupChat chat = new GroupChat() { Name = "" };
            _dbContext.Chats.Add(chat);
            MembershipInChat mic1 = new MembershipInChat()
            {
                Chat = chat,
                User = user,
            };
            MembershipInChat mic2 = new MembershipInChat()
            {
                Chat = chat,
                User = interlocutor
            };
            _dbContext.MembershipInChats.AddRange(mic1, mic2);
            _dbContext.SaveChanges();

            return chat;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chatName"></param>
        /// <param name="Creator"></param>
        /// <exception cref="ChatException"></exception>
        public GroupChat CreateChat(string chatName, NetworkUser Creator)
        {
            GroupChat chat = _dbContext.Chats.FirstOrDefault(chat => chat.Name == chatName);
            if (chat != null) throw new ChatException("Чат с таким названием уже существует");
            chat = new GroupChat() { Name = chatName };
            _dbContext.Chats.Add(chat);
            chat.Users = new List<NetworkUser>(1);
            chat.Users.Add(Creator);
            _dbContext.SaveChanges();
            return chat;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="user"></param>
        /// <exception cref="ChatException"></exception>
        public GroupChat JoinToChat(int chatId, string userId)
        {
            GroupChat chat = _dbContext.Chats.FirstOrDefault(chat => chat.Id == chatId);
            if (chat == null) throw new ChatException("Чат не существует");
            NetworkUser user = _dbContext.Users.FirstOrDefault(dbUser => dbUser.Id == userId);
            if (user == null) throw new NetworkUserException("Пользователь не существует");
            var membershipInChat = _dbContext.Chats.Join(_dbContext.MembershipInChats,
                                                         chat => chat.Id,
                                                         mic => mic.ChatId,
                                                         (chat, mic) => 1);
            if(membershipInChat.Any())
            {
                throw new ChatException("Пользователь уже состоит в данном чате");
            }
            MembershipInChat mic = new MembershipInChat()
            {
                ChatId = chatId,
                UserId = userId,
                DateTime = System.DateTime.Now
            };
            _dbContext.MembershipInChats.Add(mic);
            _dbContext.SaveChanges();
            return chat;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="chatId"></param>
        /// <exception cref="ChatException"></exception>
        public Message SendMessageToChat(string senderId, string text, int chatId)
        {
            GroupChat chat = _dbContext.Chats.FirstOrDefault(dbChat => dbChat.Id == chatId);
            if (chat == null) throw new ChatException("Чат не существует");
            NetworkUser user = _dbContext.Users.FirstOrDefault(dbUser => dbUser.Id == senderId);
            if (user == null) throw new NetworkUserException("Пользователь не существует");
            
            var userChatPairs = from _user in _dbContext.Users
                        join fact in _dbContext.MembershipInChats on senderId equals fact.UserId
                        join _chat in _dbContext.Chats on fact.ChatId equals _chat.Id
                        select 1;
            if(!userChatPairs.Any()) throw new ChatException("Пользователь не состоит в данном чате");

            Message message = new Message() { SenderId = senderId, GroupChatId = chatId, Text = text, DateTime = System.DateTime.Now };
            _dbContext.Messages.Add(message);
            _dbContext.SaveChanges();
            return message;
        }

        public Message SendMessageToChat(string senderId, string text, string interlocutorId)
        {
            NetworkUser user = _dbContext.Users.FirstOrDefault(user => user.Id == senderId);
            if (user == null) throw new NetworkUserException("Запрашивающий пользователь не существует");
            NetworkUser interlocutor = _dbContext.Users.FirstOrDefault(user => user.Id == interlocutorId);
            if (user == null) throw new NetworkUserException("Собеседник пользователя не существует");
            GroupChat chat = EnsureGetUsersDialog(user.Id, interlocutor.Id);
            Message message = new Message() { SenderId = senderId, 
                                              Chat = chat,
                                              Text = text, 
                                              DateTime = System.DateTime.Now };
            _dbContext.Messages.Add(message);
            _dbContext.SaveChanges();
            return message;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="user"></param>
        /// <exception cref="ChatException"></exception>
        public GroupChat LeaveFromChat(int chatId, string userId)
        {
            GroupChat chat = _dbContext.Chats.FirstOrDefault(dbChat => dbChat.Id == chatId);
            if (chat == null) throw new ChatException("Чат не существует");
            NetworkUser user = _dbContext.Users.FirstOrDefault(dbUser => dbUser.Id == userId);
            if (user == null) throw new NetworkUserException("Пользователь не существует");

            var userChatPairs = from _user in _dbContext.Users
                                join fact in _dbContext.MembershipInChats on userId equals fact.UserId
                                join _chat in _dbContext.Chats on fact.ChatId equals _chat.Id
                                select fact;
            var userChatPair = userChatPairs.FirstOrDefault();
            if (userChatPair == null) throw new ChatException("Пользователь не состоит в данном чате");

            _dbContext.MembershipInChats.Remove(userChatPair);

            return chat;
        }

        public List<ChatMessageViewModel> GetChatMessages(int chatId, string userId)
        {
            GroupChat chat = _dbContext.Chats.FirstOrDefault(dbChat => dbChat.Id == chatId);
            if (chat == null) throw new ChatException("Чат не существует");
            NetworkUser user = _dbContext.Users.FirstOrDefault(dbUser => dbUser.Id == userId);
            if (user == null) throw new NetworkUserException("Пользователь не существует");

            var membershipInChat = from _user in _dbContext.Users
                                join fact in _dbContext.MembershipInChats on userId equals fact.UserId
                                join _chat in _dbContext.Chats on fact.ChatId equals _chat.Id
                                select 1;
            if (!membershipInChat.Any()) throw new ChatException("Пользователь не состоит в данном чате");


            IQueryable<ChatMessageViewModel> messages = from _message in _dbContext.Messages
                                                        where _message.GroupChatId == chatId
                                                        join _user in _dbContext.Users on _message.SenderId equals _user.Id
                                                        select new ChatMessageViewModel()
                                                        {
                                                            SenderId = _message.SenderId,
                                                            SenderName = _user.UserName,
                                                            Text = _message.Text,
                                                            DateTime = _message.DateTime.ToString("f"),
                                                        };
            return messages.ToList();
        }

        public List<ChatMessageViewModel> GetDialogMessages(string userId, string interlocutorId)
        {
            NetworkUser user = _dbContext.Users.FirstOrDefault(user => user.Id == userId);
            if (user == null) throw new NetworkUserException("Запрашивающий пользователь не существует");
            NetworkUser interlocutor = _dbContext.Users.FirstOrDefault(user => user.Id == interlocutorId);
            if (user == null) throw new NetworkUserException("Собеседник пользователя не существует");
            GroupChat chat = EnsureGetUsersDialog(userId, interlocutorId);
            IQueryable<ChatMessageViewModel> messages  = from _message in _dbContext.Messages
                                                         where _message.GroupChatId == chat.Id
                                                         join _user in _dbContext.Users on _message.SenderId equals _user.Id
                                                         select new ChatMessageViewModel()
                                                         {
                                                             SenderId = _message.SenderId,
                                                             SenderName = _user.UserName,
                                                             Text = _message.Text,
                                                             DateTime = _message.DateTime.ToString("f"),
                                                         };
            return messages.ToList();
        }
    }
}
