﻿using System.Collections.Generic;
using System.Linq;
using SocialNetwork.Models.UserInfoModels;
using SocialNetwork.Models.ViewModels.UsersViewModels;

namespace SocialNetwork.Models.Repositories
{
    public interface IUsersRepository
    {
        IQueryable<NetworkUser> Users { get; }

        IQueryable<City> Cities { get; }

        IQueryable<Country> Countries { get; }

        List<UserViewModel> UsersViewModel { get; }
        FriendshipFact GetFriendshipFact(string user1Id, string user2Id);
        NetworkUser InviteFriend(string invitorId, string invitedId);
        NetworkUser AcceptFriendship(string invitorId, string invitedId);
        NetworkUser DeleteFriend(string userId, string friendId);
        List<NetworkUser> FilterUsers(UsersFilter usersFilter, string userId);
        NetworkUser GetUserById(string Id);
        NetworkUser UpdateUser(NetworkUser user);
        void SetCityInUsersInfo(string cityName, NetworkUser user);
        void SetCountryInUsersInfo(string countryName, NetworkUser user);
        List<InterlocutorViewModel> GetFriends(string userId);
        List<InterlocutorViewModel> GetInterlocutors(string userId);
        List<InterlocutorViewModel> GetIncomingFriendshipInvitations(string userId);
        List<InterlocutorViewModel> GetOutgoingFriendshipInvitations(string userId);
    }
}
