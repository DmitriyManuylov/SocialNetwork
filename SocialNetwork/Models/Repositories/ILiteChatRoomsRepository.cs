using SocialNetwork.Models.LiteChatModels;
using SocialNetwork.Models.ViewModels.LiteChatViewModels;
using System.Collections.Generic;

namespace SocialNetwork.Models.Repositories
{
    public interface ILiteChatRoomsRepository
    {
        List<LiteChatRoom> Rooms { get; }
        LiteChatRoom CreateLiteChatRoom(string roomName);
        SimpleMessage SendMessageToLiteChatRoom(int roomId, InSimpleMessageViewModel message);
        LiteChatRoom GetLiteChatRoomById(int roomId);

        List<OutSimpleMessageViewModel> GetLiteChatMessagesToView(int roomId);
    }
}
