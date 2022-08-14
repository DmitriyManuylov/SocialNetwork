namespace SocialNetwork.Models.ViewModels.SocialNetworkViewModels
{
    public class ChatMessageViewModel
    {
        public string SenderId { get; set; }
        public string SenderName { get; set; }
        public string SenderLink { get; set; }

        public string Text { get; set; }
        public string DateTime { get; set; }
    }
}
