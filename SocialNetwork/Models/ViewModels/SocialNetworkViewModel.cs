using System.Collections.Generic;

namespace SocialNetwork.Models.ViewModels
{
    public class SocialNetworkViewModel
    {
        public List<GroupChat> Chats { get; set; }

        public List<NetworkUser> Friends { get; set; }
    }
}
