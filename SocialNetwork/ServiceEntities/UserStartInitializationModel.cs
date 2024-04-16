using SocialNetwork.Models.ChatModels;
using SocialNetwork.Models.UserInfoModels;
using SocialNetwork.Models.ViewModels.AccountViewModels;
using System;
using System.Collections.Generic;

namespace SocialNetwork.ServiceEntities
{
    public class UserStartInitializationModel
    {
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CountryId { get; set; }

        public int CityId { get; set; }

        public string Password { get; set; }

        public DateTime BirthDate { get; set; }
        public Gender Gender { get; set; }
        public List<int> Chats { get; set; }
    }
}
