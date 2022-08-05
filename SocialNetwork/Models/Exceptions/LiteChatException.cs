using System;

namespace SocialNetwork.Models.Exceptions
{
    public class LiteChatException: Exception
    {
        public LiteChatException(string message): base(message) { }
    }
}
