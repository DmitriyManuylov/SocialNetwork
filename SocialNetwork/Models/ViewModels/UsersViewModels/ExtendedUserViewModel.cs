using SocialNetwork.Models.UserInfoModels;
using System;

namespace SocialNetwork.Models.ViewModels.UsersViewModels
{
    public class ExtendedUserViewModel : UserViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CityName { get; set; }
        public string CountryName { get; set; }
        public DateTime? BirthDate { get; set; }
        public Gender? Gender { get; set; }
        public int? Age { get; set; }

        public string Email { get; set; }
    }
}
