using System;

namespace SocialNetwork.Models.UserInfoModels
{
    public class UsersFilter
    {
        public string Name { get; set; }
        public string CityName { get; set; }
        public string CountryName { get; set; }
        public int? StartAge { get; set; }
        public int? EndAge { get; set; }
        public Gender Gender { get; set; }
    }
}
