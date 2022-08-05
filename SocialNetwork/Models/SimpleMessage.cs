using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Models
{
    public class SimpleMessage
    {
        public int Id { get; set; }
        [Required]
        public string SenderName { get; set; }
        [Required]
        public string Text { get; set; }
        [Required]
        public DateTime DateTime { get; set; }
        public LiteChatRoom Room { get; set; }
        [Required]
        public int RoomId { get; set; }
    }
}
