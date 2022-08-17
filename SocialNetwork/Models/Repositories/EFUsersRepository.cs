using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.Models.Exceptions;
using SocialNetwork.Models.UserInfoModels;
using SocialNetwork.Models.ViewModels.SocialNetworkViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SocialNetwork.Models.Repositories
{


    public class EFUsersRepository : IUsersRepository
    {
        private SocialNetworkDbContext _dbContext;
        public EFUsersRepository(SocialNetworkDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<NetworkUser> Users => _dbContext.Users;

        public IQueryable<City> Cities => _dbContext.Cities;

        public IQueryable<Country> Countries => _dbContext.Countries;

        public List<UserViewModel> UsersViewModel => _dbContext.Users.Select(user => new UserViewModel
        {
            Id = user.Id,
            UserName = user.UserName,
        }).ToList();

        public NetworkUser GetUserById(string Id)
        {
            return _dbContext.Users.FirstOrDefault(x => x.Id == Id);
        }

        public void SetUserBirthDate(DateTime birthDate, string userId)
        {
            NetworkUser user = GetUserById(userId);
            if (user == null) throw new NetworkUserException("Пользователь не существует");
            if (birthDate > DateTime.Now) throw new NetworkUserException("Введена недопустимая дата рождения! Данный гражданин из будущего!");

            user.BirthDate = birthDate;
            user.SetAge();
        }

        public List<NetworkUser> FilterUsers(UsersFilter usersFilter)
        {
            IQueryable<NetworkUser> users = _dbContext.Users;
            if (usersFilter.CityId.HasValue) users.Where(user => user.CityId.Value == usersFilter.CityId.Value);

            if (!string.IsNullOrEmpty(usersFilter.Name))
            {
                string[] nameParts = usersFilter.Name.Split(' ');
                if (nameParts.Length == 1) users.Where(user => EF.Functions.Like(user.FirstName, $"%{nameParts[0]}%"));
                if (nameParts.Length == 2) users.Where(user => (EF.Functions.Like(user.FirstName, $"%{nameParts[0]}%")
                                                                    &&
                                                                EF.Functions.Like(user.Surname, $"%{nameParts[1]}%"))
                                                                    ||
                                                                (EF.Functions.Like(user.FirstName, $"%{nameParts[1]}%")
                                                                    &&
                                                                EF.Functions.Like(user.FirstName, $"%{nameParts[0]}%")));
            }
            if (usersFilter.StartAge.HasValue)
            {
                users.Where(user => user.Age > usersFilter.StartAge);
            }

            if (usersFilter.EndAge.HasValue)
            {
                users.Where(user => user.Age < usersFilter.EndAge);
            }

            return users.ToList();

        }

        public void SetCityInUsersInfo(City city, NetworkUser user)
        {
            City dbEntry;
            if (city.Id == 0)
            {
                _dbContext.Add(city);
                user.City = city;

            }
            else
            {
                dbEntry = _dbContext.Cities.FirstOrDefault(dbCity => dbCity.Id == city.Id);
                if (dbEntry != null)
                    user.City = city;
            }
            _dbContext.SaveChanges();
        }

        public void SetCountryInUsersInfo(Country country, NetworkUser user)
        {
            Country dbEntry;
            if (country.Id == 0)
            {
                _dbContext.Add(country);
                user.Country = country;

            }
            else
            {
                dbEntry = _dbContext.Countries.FirstOrDefault(dbCountry => dbCountry.Id == country.Id);
                if (dbEntry != null)
                    user.Country = country;
            }
            _dbContext.SaveChanges();

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="invitorId"></param>
        /// <param name="invitedId"></param>
        /// <exception cref="FriendshipException"></exception>
        public void InviteFriend(string invitorId, string invitedId)
        {
            NetworkUser invitor = _dbContext.Users.Include(user => user.FriendshipFactsOut)
                                                  .Include(user => user.FriendshipFactsIn)
                                                  .FirstOrDefault(dbUser => dbUser.Id == invitorId);
            NetworkUser invited = _dbContext.Users.Include(user => user.FriendshipFactsOut)
                                                  .Include(user => user.FriendshipFactsIn)
                                                  .FirstOrDefault(dbUser => dbUser.Id == invitedId);
            if (invitor == null) throw new FriendshipException("Приглашающий не существует");
            if (invited == null) throw new FriendshipException("Приглашаемый не существует");
            FriendshipFact friendshipFactOut = invitor.FriendshipFactsOut.FirstOrDefault(fact => fact.InitiatorId == invitorId && fact.InvitedId == invitedId);
            FriendshipFact friendshipFactIn = invitor.FriendshipFactsOut.FirstOrDefault(fact => fact.InitiatorId == invitedId && fact.InvitedId == invitorId);
            if (friendshipFactIn != null || friendshipFactOut != null) throw new FriendshipException("Пользователи уже состоят в дружбе");
            FriendshipFact friendshipFact = new FriendshipFact()
            {
                InitiatorId = invitorId,
                Initiator = invitor,
                Invited = invited,
                InvitedId = invitedId,
                RequestAccepted = false,
            };
            _dbContext.FriendshipFacts.Add(friendshipFact);
            _dbContext.SaveChanges();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="invitorId"></param>
        /// <param name="invitedId"></param>
        /// <exception cref="FriendshipException"></exception>
        public void AcceptFriendship(string invitorId, string invitedId)
        {
            NetworkUser invitor = _dbContext.Users.Include(user => user.FriendshipFactsOut)
                                                  .Include(user => user.FriendshipFactsIn)
                                                  .FirstOrDefault(dbUser => dbUser.Id == invitorId);
            NetworkUser invited = _dbContext.Users.Include(user => user.FriendshipFactsOut)
                                                  .Include(user => user.FriendshipFactsIn)
                                                  .FirstOrDefault(dbUser => dbUser.Id == invitedId);
            if (invitor == null) throw new FriendshipException("Приглашающий не существует");
            if (invited == null) throw new FriendshipException("Приглашаемый не существует");

            FriendshipFact friendshipFactIn = invited.FriendshipFactsIn.FirstOrDefault(fact => fact.InitiatorId == invitorId && fact.InvitedId == invitedId);
            if (friendshipFactIn == null) throw new FriendshipException("Приглашение дружить не поступало");
            if (friendshipFactIn.RequestAccepted) throw new FriendshipException("Приглашение о дружбе уже было принято");
            friendshipFactIn.DateOfConclusion = DateTime.UtcNow;
            friendshipFactIn.RequestAccepted = true;
            _dbContext.SaveChanges();
        }

        public List<InterlocutorViewModel> GetFriends(string userId)
        {
            IQueryable<InterlocutorViewModel> friendsIn = from user in _dbContext.Users
                                                  join ff in _dbContext.FriendshipFacts on user.Id equals ff.InitiatorId
                                                  where ff.InvitedId == userId && ff.RequestAccepted == true
                                                  select new InterlocutorViewModel { Id = user.Id, UserName = user.UserName };
            IQueryable<InterlocutorViewModel> friendsOut = from user in _dbContext.Users
                                                  join ff in _dbContext.FriendshipFacts on user.Id equals ff.InvitedId
                                                  where ff.InitiatorId == userId && ff.RequestAccepted == true
                                                  select new InterlocutorViewModel { Id = user.Id, UserName = user.UserName };
            List<InterlocutorViewModel> friendsList = friendsIn.ToList();
            List<InterlocutorViewModel> friendsOutList = friendsOut.ToList();

            friendsList.AddRange(friendsOutList);
            return friendsList;
        }

        public List<InterlocutorViewModel> GetIncomingFriendshipInvitations(string userId)
        {
            IQueryable<InterlocutorViewModel> friendsIn = from user in _dbContext.Users
                                                  join ff in _dbContext.FriendshipFacts on user.Id equals ff.InitiatorId
                                                  where ff.InvitedId == userId && ff.RequestAccepted == false
                                                  select new InterlocutorViewModel { Id = user.Id, UserName = user.UserName };
            return friendsIn.ToList();
        }

        public List<InterlocutorViewModel> GetOutgoingFriendshipInvitations(string userId)
        {
            IQueryable<InterlocutorViewModel> friendsOut = from user in _dbContext.Users
                                                   join ff in _dbContext.FriendshipFacts on user.Id equals ff.InvitedId
                                                   where ff.InitiatorId == userId && ff.RequestAccepted == false
                                                   select new InterlocutorViewModel { Id = user.Id, UserName = user.UserName };
            return friendsOut.ToList();
        }
    }
}
