using System;

namespace SocialNetwork.Models.Exceptions
{
    public class FriendshipException: Exception
    {
        public FriendshipException(string message): base(message) { }
    }
}
