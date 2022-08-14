using System.Collections.Generic;
using SocialNetwork.Models.ChatModels;
using SocialNetwork.Models.UserInfoModels;

namespace SocialNetwork.Models.ViewModels.SocialNetworkViewModels
{
    public class SocialNetworkViewModel
    {
        public List<ChatViewModel> Chats { get; set; }

        public List<UserViewModel> Friends { get; set; }

        public List<UserViewModel> IncomingFriendshipInvitations { get; set; }
        public List<UserViewModel> OutgoingFriendshipInvitations { get; set; }
    }
}
