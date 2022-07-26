using System.Collections.Generic;

namespace SocialNetwork.Models
{
    public class Chat
    {
        public ICollection<Message> Messages { get; set; }


    }
}
