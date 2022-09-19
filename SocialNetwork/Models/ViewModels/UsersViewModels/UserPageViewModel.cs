using SocialNetwork.Models.UserInfoModels;
using SocialNetwork.Models.ViewModels.SocialNetworkViewModels;
using System.Collections.Generic;

namespace SocialNetwork.Models.ViewModels.UsersViewModels
{
    public class UserPageViewModel
    {
        public NetworkUser User { get; set; }
        public FriendshipFactStates FriendshipFactState { get; set; }
        public List<UserViewModel> Friends { get; set; }
        public List<ChatViewModel> Chats { get; set; }
    }
}
