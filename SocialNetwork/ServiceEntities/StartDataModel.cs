using System.Collections.Generic;
using SocialNetwork.Models.ChatModels;
using SocialNetwork.Models.UserInfoModels;
using SocialNetwork.Models.ViewModels.AccountViewModels;
using SocialNetwork.Models.ViewModels.SocialNetworkViewModels;

namespace SocialNetwork.ServiceEntities
{
    public class StartDataModel
    {
        public List<Country> Countries { get; set; }
        public List<City> Cities { get; set; }

        public List<UserStartInitializationModel> Users { get; set; }

        public List<Friendship> FriendshipFacts { get; set; }

        public List<GroupChat> Chats { get; set; }

    }
}
