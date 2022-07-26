
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace SocialNetwork.Models
{
    public class Dialog
    {
        public NetworkUser Sender { get; set; }

        public NetworkUser Receiver { get; set; }

        public ICollection<Message> Messages { get; set; }


    }
}
