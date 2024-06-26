﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Models.UserInfoModels
{
    public class Country
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public List<City> Cities;
    }
}
