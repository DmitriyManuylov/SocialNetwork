using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace SocialNetwork.Models
{
    public class GroupChat
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<NetworkUser> Users { get; set; }

        public List<Message> Messages { get; set; }


    }
}
