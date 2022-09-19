using SocialNetwork.Models.ChatModels;
using System;
using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Models.UserInfoModels
{
    public class FriendshipFact
    {
        //public int Id { get; set; }
        [Required]
        public string InitiatorId { get; set; }
        public NetworkUser Initiator { get; set; }
        [Required]
        public string InvitedId { get; set; }
        public NetworkUser Invited { get; set; }
        public int? DialogId { get; set; }
        public Dialog Dialog { get; set; }
        [Required]
        public bool RequestAccepted { get; set; } = false;
        [Required]
        public DateTime DateOfConclusion { get; set; }
    }
}
