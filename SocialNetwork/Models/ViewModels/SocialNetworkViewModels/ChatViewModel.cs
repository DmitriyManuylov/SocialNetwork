using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Models.ViewModels.SocialNetworkViewModels
{
    public class ChatViewModel
    {
        [Required]
        public int Id;
        [Required]
        public string Name;
    }
}
