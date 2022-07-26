using System;
using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Models
{
    public class FriendshipFact
    {

        [Required]
        public string UserId { get; set; }
        public NetworkUser User { get; set; }
        [Required]
        public string FriendId { get; set; }
        public NetworkUser Friend { get; set; }

        public DateTime DateOfConclusion { get; set; }
    }
}
