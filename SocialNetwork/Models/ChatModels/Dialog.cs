using SocialNetwork.Models.UserInfoModels;
using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Models.ChatModels
{
    public class Dialog
    {
        public int Id { get; set; }
        [Required]
        public int ChatId { get; set; }
        public GroupChat Chat { get; set; }
        [Required]
        public string User1Id { get; set; }
        public NetworkUser User1 { get; set; }
        [Required]
        public string User2Id { get; set; }
        public NetworkUser User2 { get; set; }
    }
}
