using System;
using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Models
{
    public class MembershipInChat
    {
        [Required]
        public string UserId { get; set; }
        public NetworkUser User { get; set; }

        [Required]
        public int ChatId { get; set; }
        public GroupChat Chat { get; set; }
        [Required]
        public DateTime DateTime { get; set; }
    }
}
