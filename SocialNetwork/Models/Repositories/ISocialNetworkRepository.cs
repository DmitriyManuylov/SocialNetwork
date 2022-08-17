using SocialNetwork.Models.ChatModels;
using SocialNetwork.Models.UserInfoModels;
using SocialNetwork.Models.ViewModels.SocialNetworkViewModels;
using System.Collections.Generic;

namespace SocialNetwork.Models.Repositories
{
    public interface ISocialNetworkRepository
    {
        List<ChatViewModel> AllChatsViewModel { get; }
        List<ChatViewModel> GetUserChats(string userId);
        GroupChat GetChatById(int chatId);
        GroupChat GetUsersDialog(string userId, string interlocutorId);
        GroupChat EnsureGetUsersDialog(string userId, string interlocutorId);
        GroupChat CreateChat(string chatName, NetworkUser Creator);
        GroupChat JoinToChat(int chatId, string userId);
        Message SendMessageToChat(string senderId, string text, int chatId);
        Message SendMessageToChat(string senderId, string text, string interlocutorId);
        public GroupChat LeaveFromChat(int chatId, string userId);
        List<ChatMessageViewModel> GetChatMessages(int chatId, string userId);
        List<ChatMessageViewModel> GetDialogMessages(string userId, string interlocutorId);

    }
}
