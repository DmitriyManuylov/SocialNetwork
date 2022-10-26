using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Models.UserInfoModels
{
    public class City
    {

        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        public Country Country { get; set; }

        public int? CountryId { get; set; }
    }
}
