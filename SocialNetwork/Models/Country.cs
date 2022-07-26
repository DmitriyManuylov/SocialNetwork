using System.Collections.Generic;

namespace SocialNetwork.Models
{
    public class Country
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<City> Cities;
    }
}
