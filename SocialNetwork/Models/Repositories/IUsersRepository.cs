using System.Collections.Generic;
using System.Linq;
using SocialNetwork.Models.UserInfoModels;
using SocialNetwork.Models.ViewModels.SocialNetworkViewModels;

namespace SocialNetwork.Models.Repositories
{
    public interface IUsersRepository
    {
        IQueryable<NetworkUser> Users { get; }

        IQueryable<City> Cities { get; }

        IQueryable<Country> Countries { get; }

        List<UserViewModel> UsersViewModel { get; }
        void InviteFriend(string invitorId, string invitedId);
        void AcceptFriendship(string invitorId, string invitedId);
        List<NetworkUser> FilterUsers(UsersFilter usersFilter);
        NetworkUser GetUserById(string Id);
        void SetCityInUsersInfo(City city, NetworkUser user);
        void SetCountryInUsersInfo(Country country, NetworkUser user);
        List<UserViewModel> GetFriends(string userId);
        List<UserViewModel> GetIncomingFriendshipInvitations(string userId);
        List<UserViewModel> GetOutgoingFriendshipInvitations(string userId);
    }
}
