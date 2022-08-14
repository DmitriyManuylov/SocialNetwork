using System;

namespace SocialNetwork.Models.ViewModels.LiteChatViewModels
{
    public class OutSimpleMessageViewModel
    {
        public int Id { get; set; }

        public string Sender { get; set; }

        public string Text { get; set; }

        public string DateTime { get; set; }
    }
}
