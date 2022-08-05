namespace SocialNetwork.Models.Repositories
{
    public interface IChatsRepository
    {
        GroupChat CreateChat(string chatName, NetworkUser Creator);
        void JoinToChat(int chatId, string userId);
        Message SendMessageToChat(Message message, int chatId);
        public void LeaveFromChat(int chatId, string userId);
    }
}
