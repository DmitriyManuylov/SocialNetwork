using Microsoft.EntityFrameworkCore;
using SocialNetwork.Models.Exceptions;
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
        public void JoinToChat(int chatId, string userId)
        {
            GroupChat chat = _dbContext.Chats.FirstOrDefault(chat => chat.Id == chatId);
            if (chat == null) throw new ChatException("Чат не существует");
            NetworkUser user = chat.Users.FirstOrDefault(dbUser => dbUser.Id == userId);
            if (user != null)
            {
                throw new ChatException("Пользователь уже состоит в данном чате");
            }
            chat.Users.Add(user);
            _dbContext.SaveChanges();
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
            chat.Messages.Add(message);
            _dbContext.SaveChanges();
            return message;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="user"></param>
        /// <exception cref="ChatException"></exception>
        public void LeaveFromChat(int chatId, string userId)
        {
            GroupChat chat = _dbContext.Chats.FirstOrDefault(dbChat => dbChat.Id == chatId);
            if (chat == null) throw new ChatException("Чат не существует");
            NetworkUser user = _dbContext.Users.FirstOrDefault(dbUser => dbUser.Id == userId);
            if (user == null) throw new NetworkUserException("Пользователь не существует");

            var userChatPairs = from _user in _dbContext.Users
                                join fact in _dbContext.MembershipInChats on userId equals fact.UserId
                                join _chat in _dbContext.Chats on fact.ChatId equals _chat.Id
                                select 1;
            if (!userChatPairs.Any()) throw new ChatException("Пользователь не состоит в данном чате");

            chat.Users.Remove(user);
        }
    }
}
