using System.Linq;

namespace SocialNetwork.Models.Repositories
{
    public interface IUsersRepository
    {
        IQueryable<NetworkUser> Users { get; }

        IQueryable<City> Cities { get; }

        IQueryable<Country> Countries { get; }

        

        void AcceptFriendship(string invitorId, string invitedId);
        IQueryable<NetworkUser> FilterUsers(UsersFilter usersFilter);
        NetworkUser GetUserById(string Id);
        void SetCityInUsersInfo(City city, NetworkUser user);
        void SetCountryInUsersInfo(Country country, NetworkUser user);
    }
}
