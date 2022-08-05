using System;

namespace SocialNetwork.Models.Exceptions
{
    public class NetworkUserException: Exception
    {
        public NetworkUserException(string message) : base(message) { }
    }
}
