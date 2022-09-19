using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SocialNetwork.Models.ViewModels.SocialNetworkViewModels;
using SocialNetwork.Models.ViewModels.UsersViewModels;
using System.Threading.Tasks;

namespace SocialNetwork.Hubs
{
    public interface ISocialNetworkHubClient
    {
        Task ChatNotifyAsync(string message, string dateTime);
        Task FriendshipAccepted(UserViewModel userIsInvited);
        Task FriendshipRequested(UserViewModel invitor);
        Task FriendshipRejected(UserViewModel invitor);
        Task FriendshipInvitationCanceled(UserViewModel invitor);
        Task DeletedByUserFromFriends(UserViewModel initiator);
        Task ChatCreated(ChatViewModel chatViewModel);
        Task MessageRecieved(ChatMessageViewModel message);
    }
    [Authorize]
    public class SocialNetworkHub : Hub<ISocialNetworkHubClient>
    {

    }
}
