using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Models.ViewModels.LiteChatViewModels
{
    public class InSimpleMessageViewModel
    {
        [Required]
        public string Sender { get; set; }
        [Required]
        public string Text { get; set; }
    }
}
