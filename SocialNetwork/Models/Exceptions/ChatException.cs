using System;

namespace SocialNetwork.Models.Exceptions
{
    public class ChatException: Exception
    {
        
        public ChatException(string message) :base(message)
        {

        }
    }
}
