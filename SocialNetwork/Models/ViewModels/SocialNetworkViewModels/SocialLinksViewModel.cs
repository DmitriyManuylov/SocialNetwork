using System.Collections.Generic;
using SocialNetwork.Models.ChatModels;
using SocialNetwork.Models.UserInfoModels;
using SocialNetwork.Models.ViewModels.UsersViewModels;

namespace SocialNetwork.Models.ViewModels.SocialNetworkViewModels
{
    public class SocialLinksViewModel
    {
        public string UserId { get; set; }
        public List<ChatViewModel> Chats { get; set; }

        public List<InterlocutorViewModel> Friends { get; set; }

        public List<InterlocutorViewModel> Interlocutors { get; set; }

    }
}
