namespace SocialNetwork.Models.Repositories
{
    public interface ISocialNetworkRepository
    {
        GroupChat CreateChat(string chatName, NetworkUser Creator);
        void JoinToChat(int chatId, string userId);
        Message SendMessageToChat(string senderId, string text, int chatId);
        public void LeaveFromChat(int chatId, string userId);
    }
}
