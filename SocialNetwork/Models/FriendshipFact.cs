using System;
using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Models
{
    public class FriendshipFact
    {

        [Required]
        public string Friend1Id { get; set; }
        public NetworkUser Friend1 { get; set; }
        [Required]
        public string Friend2Id { get; set; }
        public NetworkUser Friend2 { get; set; }

        public DateTime DateOfConclusion { get; set; }
    }
}
