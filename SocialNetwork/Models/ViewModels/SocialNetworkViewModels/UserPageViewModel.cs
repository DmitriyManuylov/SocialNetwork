using SocialNetwork.Models.UserInfoModels;

namespace SocialNetwork.Models.ViewModels.SocialNetworkViewModels
{
    public class UserPageViewModel
    {
        public NetworkUser User { get; set; }
        public FriendshipFactStates FriendshipFactState { get; set; }
    }
}
