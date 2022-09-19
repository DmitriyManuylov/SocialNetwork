using System.Collections.Generic;

namespace SocialNetwork.Models.ViewModels.SocialNetworkViewModels
{
    public class ChatsPageViewModel
    {
        public List<ChatViewModel> UserChats { get; set; }

        public List<ChatViewModel> OtherChats { get; set; }

    }
}
