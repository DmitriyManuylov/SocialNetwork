using System.Collections.Generic;
using SocialNetwork.Models.ChatModels;
using SocialNetwork.Models.UserInfoModels;

namespace SocialNetwork.Models.ViewModels.SocialNetworkViewModels
{
    public class SocialLinksViewModel
    {
        public List<ChatViewModel> Chats { get; set; }

        public List<InterlocutorViewModel> Friends { get; set; }

        public List<InterlocutorViewModel> Interlocutors { get; set; }

    }
}
