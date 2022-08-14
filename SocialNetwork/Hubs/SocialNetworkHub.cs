using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SocialNetwork.Models.ViewModels.SocialNetworkViewModels;
using System.Threading.Tasks;

namespace SocialNetwork.Hubs
{
    public interface ISocialNetworkHubClient
    {
        Task ChatNotifyAsync(string message, string dateTime);
        Task FriendshipAccepted(string userIsInvitedId);
        Task FriendshipRequested(string invitorId);
        Task MessageRecieved(ChatMessageViewModel message);
    }
    [Authorize]
    public class SocialNetworkHub : Hub<ISocialNetworkHubClient>
    {

    }
}
