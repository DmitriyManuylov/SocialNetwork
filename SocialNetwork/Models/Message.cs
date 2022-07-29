using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace SocialNetwork.Models
{
    public class Message
    {
        public int Id { get; set; }
        [Required]
        public string SenderId { get; set; }
        public NetworkUser Sender { get; set; }
        [Required]
        public int GroupChatId { get; set; }

        public GroupChat Chat { get; set; }

        public DateTime DateTime { get; set; }
        [Required]
        public string Text { get; set; } = string.Empty;  
        
    }
}
