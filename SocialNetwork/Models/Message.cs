using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialNetwork.Models
{
    public class Message
    {
        public int Id { get; set; }
        public NetworkUser Sender { get; set; }

        public DateTime DateTime { get; set; }

        public string Text { get; set; } = string.Empty;  
        
    }
}
