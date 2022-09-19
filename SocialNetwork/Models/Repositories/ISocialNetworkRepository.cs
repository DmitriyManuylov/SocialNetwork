using SocialNetwork.Models.ChatModels;
using SocialNetwork.Models.UserInfoModels;
using SocialNetwork.Models.ViewModels.SocialNetworkViewModels;
using SocialNetwork.Models.ViewModels.UsersViewModels;
using System.Collections.Generic;

namespace SocialNetwork.Models.Repositories
{
    public interface ISocialNetworkRepository
    {
        List<ChatViewModel> AllChatsViewModel { get; }
        List<ChatViewModel> FilterChats(ChatFilter chatFilter, string userId);
        List<ChatViewModel> GetUserChats(string userId);
        GroupChat GetChatById(int chatId);
        ChatViewModelWithUsers GetChatPageViewModel(int chatId);
        Dialog GetUsersDialog(string userId, string interlocutorId);
        Dialog EnsureGetUsersDialog(string userId, string interlocutorId);
        GroupChat CreateChat(string chatName, string creatorId);
        GroupChat JoinToChat(int chatId, string userId);
        Message SendMessageToChat(string senderId, string text, int chatId);
        Message SendMessageToDialog(string senderId, string text, string interlocutorId);
        public GroupChat LeaveFromChat(int chatId, string userId);
        List<ChatMessageViewModel> GetChatMessages(int chatId, string userId);
        List<ChatMessageViewModel> GetDialogMessages(int chatId, string userId, string interlocutorId);

    }
}
