using SocialNetwork.Models.ViewModels.UsersViewModels;
using System.Collections.Generic;

namespace SocialNetwork.Models.ViewModels.SocialNetworkViewModels
{
    public class ChatViewModelWithUsers: ChatViewModel
    {
        public List<UserViewModel> Members { get; set; }
    }
}
