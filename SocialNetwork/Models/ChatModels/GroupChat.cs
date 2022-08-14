using Microsoft.AspNetCore.Identity;
using SocialNetwork.Models.UserInfoModels;
using System.Collections.Generic;

namespace SocialNetwork.Models.ChatModels
{
    public class GroupChat
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<NetworkUser> Users { get; set; }

        public List<Message> Messages { get; set; }

        public List<MembershipInChat> MembershipInChats { get; set; }
    }
}
