using Microsoft.EntityFrameworkCore;
using SocialNetwork.Models.ChatModels;
using SocialNetwork.Models.Exceptions;
using SocialNetwork.Models.UserInfoModels;
using SocialNetwork.Models.ViewModels.SocialNetworkViewModels;
using SocialNetwork.Models.ViewModels.UsersViewModels;
using System;
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


        public List<ChatViewModel> FilterChats(ChatFilter chatFilter, string userId)
        {
            List<ChatViewModel> filteredChats;
            IQueryable<int> userChatsIds = from chat in _dbContext.Chats
                                           join mic in _dbContext.MembershipInChats on chat.Id equals mic.ChatId
                                           where mic.UserId == userId
                                           select chat.Id;
            if (!string.IsNullOrEmpty(chatFilter.ChatName))
            {
                filteredChats = _dbContext.Chats.Where(chat => EF.Functions.Like(chat.Name, $"%{chatFilter.ChatName}%")).Select(chat => new ChatViewModel()
                {
                    Id = chat.Id,
                    Name = chat.Name,
                    ChatLink = $"/Chat{chat.Id}",
                    IsUserAMember = userChatsIds.Contains(chat.Id)
                }).OrderBy(chat => chat.Name).ToList();
            }
            else
                filteredChats = new List<ChatViewModel>();

            return filteredChats;
        }


        public List<ChatViewModel> GetUserChats(string userId)
        {
            IQueryable<ChatViewModel> chats = from chat in _dbContext.Chats
                                              join mic in _dbContext.MembershipInChats on chat.Id equals mic.ChatId
                                              where mic.UserId == userId
                                              select new ChatViewModel
                                              {
                                                  Id = chat.Id,
                                                  Name = chat.Name,
                                                  ChatLink = $"/Chat{chat.Id}",
                                                  IsUserAMember = true
                                              };
            return chats.ToList();
        }


        public GroupChat GetChatById(int chatId)
        {
            GroupChat chat = _dbContext.Chats.FirstOrDefault(chat => chat.Id == chatId);
            if (chat == null) throw new ChatException("Чат с таким Id не существует");
            return chat;
        }


        public ChatViewModelWithUsers GetChatPageViewModel(int chatId)
        { 
            ChatViewModelWithUsers model = _dbContext.Chats.Select(chat => new ChatViewModelWithUsers()
            {
                Id = chat.Id,
                Name = chat.Name,
                Members = _dbContext.Users.Join(_dbContext.MembershipInChats, u => u.Id, mic => mic.UserId, (u, mic) => new
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    ChatId = mic.ChatId
                }).Where(a => a.ChatId == chat.Id).Select(a => new UserViewModel()
                {
                    Id = a.Id,
                    UserName = a.UserName,
                    UserPageLink = $"User{a.Id}"
                }).ToList()

            }).FirstOrDefault(chat => chat.Id == chatId);
            return model;
        }


        public Dialog GetUsersDialog(string userId, string interlocutorId)
        {
            NetworkUser user = _dbContext.Users.FirstOrDefault(user => user.Id == userId);
            if (user == null) throw new NetworkUserException("Запрашивающий пользователь не существует");
            NetworkUser interlocutor = _dbContext.Users.FirstOrDefault(user => user.Id == interlocutorId);
            if (interlocutor == null) throw new NetworkUserException("Собеседник пользователя не существует");

            Dialog dialog = _dbContext.Dialogs.Include(dialog => dialog.User1)
                                              .Include(dialog => dialog.User2)
                                              .Include(dialog => dialog.Chat)
                                              .FirstOrDefault(dialog => (dialog.User1Id == userId && dialog.User2Id == interlocutorId)
                                                                            ||
                                                                        (dialog.User1Id == interlocutorId && dialog.User2Id == userId));
            
            return dialog;
        }


        public Dialog EnsureGetUsersDialog(string userId, string interlocutorId)
        {
            NetworkUser user = _dbContext.Users.FirstOrDefault(user => user.Id == userId);
            if (user == null) throw new NetworkUserException("Запрашивающий пользователь не существует");
            NetworkUser interlocutor = _dbContext.Users.FirstOrDefault(user => user.Id == interlocutorId);
            if (interlocutor == null) throw new NetworkUserException("Собеседник пользователя не существует");

            Dialog dialog = _dbContext.Dialogs.Include(dialog => dialog.User1)
                                              .Include(dialog => dialog.User2)
                                              .Include(dialog => dialog.Chat)
                                              .FirstOrDefault(dialog => (dialog.User1Id == userId && dialog.User2Id == interlocutorId)
                                                                            ||
                                                                        (dialog.User1Id == interlocutorId && dialog.User2Id == userId));

            if (dialog == null) dialog = CreateUsersDialog(user, interlocutor);
            return dialog;
        }


        private Dialog CreateUsersDialog(NetworkUser user, NetworkUser interlocutor)
        {
            FriendshipFact friendshipFact = _dbContext.FriendshipFacts.FirstOrDefault(ff => (ff.InitiatorId == user.Id && ff.InvitedId == interlocutor.Id
                                                                                                || 
                                                                                            ff.InitiatorId == interlocutor.Id && ff.InvitedId == user.Id)
                                                                                                &&
                                                                                            ff.RequestAccepted);
             
            GroupChat chat = new GroupChat() { Name = "" };
            _dbContext.Chats.Add(chat);
            Dialog dialog = new Dialog()
            {
                User1Id = user.Id,
                User2Id = interlocutor.Id,
                User1 = user,
                User2 = interlocutor,
                Chat = chat,
            };
            _dbContext.Dialogs.Add(dialog);
            if (friendshipFact != null)
                friendshipFact.Dialog = dialog;
            _dbContext.SaveChanges();

            return dialog;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chatName"></param>
        /// <param name="Creator"></param>
        /// <exception cref="ChatException"></exception>
        public GroupChat CreateChat(string chatName, string creatorId)
        {

            GroupChat chat = _dbContext.Chats.FirstOrDefault(chat => chat.Name == chatName);
            if (chat != null) throw new ChatException("Чат с таким названием уже существует");
            chat = new GroupChat() { Name = chatName };
            _dbContext.Chats.Add(chat);
            MembershipInChat mic = new MembershipInChat()
            {
                Chat = chat,
                UserId = creatorId,
                DateTime = DateTime.Now,
            };
            _dbContext.MembershipInChats.Add(mic);
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
            var membershipInChat = _dbContext.MembershipInChats.Where(mic => mic.ChatId == chatId && mic.UserId == userId);

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

            var membershipInChats = from mic in _dbContext.MembershipInChats
                                    where mic.ChatId == chatId & mic.UserId == senderId
                                    select 1;

            if(!membershipInChats.Any()) throw new ChatException("Пользователь не состоит в данном чате");

            Message message = new Message() { SenderId = senderId, GroupChatId = chatId, Text = text, DateTime = DateTime.Now };
            _dbContext.Messages.Add(message);
            _dbContext.SaveChanges();
            return message;
        }

        public Message SendMessageToDialog(string senderId, string text, string interlocutorId)
        {
            NetworkUser user = _dbContext.Users.FirstOrDefault(user => user.Id == senderId);
            if (user == null) throw new NetworkUserException("Запрашивающий пользователь не существует");
            NetworkUser interlocutor = _dbContext.Users.FirstOrDefault(user => user.Id == interlocutorId);
            if (user == null) throw new NetworkUserException("Собеседник пользователя не существует");
            Dialog dialog = EnsureGetUsersDialog(user.Id, interlocutor.Id);
            GroupChat chat = dialog.Chat; 
            Message message = new Message() { SenderId = senderId, 
                                              Chat = chat,
                                              Text = text, 
                                              DateTime = DateTime.Now };
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

            var membershipInChats = from mic in _dbContext.MembershipInChats
                                    where mic.ChatId == chatId & mic.UserId == userId
                                    select mic;
            var membershipInChat = membershipInChats.FirstOrDefault();
            if (membershipInChat == null) throw new ChatException("Пользователь не состоит в данном чате");

            _dbContext.MembershipInChats.Remove(membershipInChat);
            _dbContext.SaveChanges();
            return chat;
        }

        public List<ChatMessageViewModel> GetChatMessages(int chatId, string userId)
        {
            GroupChat chat = _dbContext.Chats.FirstOrDefault(dbChat => dbChat.Id == chatId);
            if (chat == null) throw new ChatException("Чат не существует");
            NetworkUser user = _dbContext.Users.FirstOrDefault(dbUser => dbUser.Id == userId);
            if (user == null) throw new NetworkUserException("Пользователь не существует");

            var membershipInChats = from mic in _dbContext.MembershipInChats
                                    where mic.ChatId == chatId & mic.UserId == userId
                                    select mic;
            var membershipInChat = membershipInChats.FirstOrDefault();
            if (membershipInChat == null) throw new ChatException("Пользователь не состоит в данном чате");


            IQueryable<ChatMessageViewModel> messages = from _message in _dbContext.Messages
                                                        where _message.GroupChatId == chatId
                                                        join _user in _dbContext.Users on _message.SenderId equals _user.Id
                                                        select new ChatMessageViewModel()
                                                        {
                                                            SenderId = _message.SenderId,
                                                            SenderName = _user.UserName,
                                                            Text = _message.Text,
                                                            DateTime = _message.DateTime.ToString("f"),
                                                            ChatId = chatId,
                                                            SenderLink = $"User{_message.SenderId}"
                                                        };
            return messages.ToList();
        }

        public List<ChatMessageViewModel> GetDialogMessages(int chatId, string userId, string interlocutorId)
        {
            NetworkUser user = _dbContext.Users.FirstOrDefault(user => user.Id == userId);
            if (user == null) throw new NetworkUserException("Запрашивающий пользователь не существует");
            NetworkUser interlocutor = _dbContext.Users.FirstOrDefault(user => user.Id == interlocutorId);
            if (user == null) throw new NetworkUserException("Собеседник пользователя не существует");
            Dialog dialog = EnsureGetUsersDialog(userId, interlocutorId);
            GroupChat chat = dialog.Chat;
            IQueryable<ChatMessageViewModel> messages  = from _message in _dbContext.Messages
                                                         where _message.GroupChatId == chat.Id
                                                         join _user in _dbContext.Users on _message.SenderId equals _user.Id
                                                         select new ChatMessageViewModel()
                                                         {
                                                             SenderId = _message.SenderId,
                                                             SenderName = _user.UserName,
                                                             Text = _message.Text,
                                                             DateTime = _message.DateTime.ToString("f"),
                                                             ChatId = chat.Id,
                                                             SenderLink = $"User{_message.SenderId}"
                                                         };
            return messages.ToList();
        }
    }
}
