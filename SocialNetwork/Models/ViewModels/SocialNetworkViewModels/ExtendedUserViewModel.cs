using System;

namespace SocialNetwork.Models.ViewModels.SocialNetworkViewModels
{
    public class ExtendedUserViewModel: UserViewModel
    {
        public string CityName { get; set; }
        public string CountryName { get; set; }
        public DateTime? BirthDate { get; set; }
        public int? Age { get; set; }

        public string Email { get; set; }
    }
}
