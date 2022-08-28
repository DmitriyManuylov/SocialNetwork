﻿using Microsoft.AspNetCore.Identity;
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
            return _dbContext.Users
                             .Include(user => user.City)
                             .Include(user => user.Country)
                             .FirstOrDefault(x => x.Id == Id);
        }

        public NetworkUser UpdateUser(NetworkUser user)
        {
            _dbContext.Users.Update(user);
            _dbContext.SaveChanges();
            return user;
        }

        public void SetUserBirthDate(DateTime birthDate, string userId)
        {
            NetworkUser user = GetUserById(userId);
            if (user == null) throw new NetworkUserException("Пользователь не существует");
            if (birthDate > DateTime.Now) throw new NetworkUserException("Введена недопустимая дата рождения! Данный гражданин из будущего!");

            user.BirthDate = birthDate;
            user.SetAge();
        }

        public List<NetworkUser> FilterUsers(UsersFilter usersFilter, string userId)
        {
            IQueryable<NetworkUser> users = _dbContext.Users;
            users = users.Include(user => user.City).Include(user => user.Country).Where(user => user.Id != userId);
            if (!string.IsNullOrEmpty(usersFilter.CityName)) users = users.Where(user => user.City.Name == usersFilter.CityName);
            if (!string.IsNullOrEmpty(usersFilter.CountryName)) users = users.Where(user => user.Country.Name == usersFilter.CountryName);

            if (!string.IsNullOrEmpty(usersFilter.Name))
            {
                string[] nameParts = usersFilter.Name.Split(' ');
                if (nameParts.Length == 1) users = users.Where(user => EF.Functions.Like(user.FirstName, $"%{nameParts[0]}%")
                                                                           ||
                                                                       EF.Functions.Like(user.Surname, $"%{nameParts[0]}%"));

                else 
                if (nameParts.Length == 2) users = users.Where(user => (EF.Functions.Like(user.FirstName, $"%{nameParts[0]}%")
                                                                           &&
                                                                        EF.Functions.Like(user.Surname, $"%{nameParts[1]}%"))
                                                                           ||
                                                                       (EF.Functions.Like(user.FirstName, $"%{nameParts[1]}%")
                                                                           &&
                                                                        EF.Functions.Like(user.Surname, $"%{nameParts[0]}%")));
            }
            if (usersFilter.StartAge.HasValue)
            {
                users = users.Where(user => user.Age >= usersFilter.StartAge);
            }

            if (usersFilter.EndAge.HasValue)
            {
                users = users.Where(user => user.Age <= usersFilter.EndAge);
            }
            if (usersFilter.Gender != Gender.NotSpecified)
            {
                users = users.Where(user => user.Gender == usersFilter.Gender);
            }
            var result = users.ToList();
            return result;

        }

        public void SetCityInUsersInfo(string cityName, NetworkUser user)
        {
            City city = _dbContext.Cities.FirstOrDefault(dbCity => dbCity.Name == cityName);
            if (city == null)
            {
                city = new City()
                {
                    Name = cityName,
                };
                _dbContext.Cities.Add(city);
            }
                
            user.City = city;

            _dbContext.SaveChanges();
        }


        public void SetCountryInUsersInfo(string countryName, NetworkUser user)
        {
            Country country = _dbContext.Countries.FirstOrDefault(dbCountry => dbCountry.Name == countryName);
            if (country == null)
            {
                country = new Country()
                {
                    Name = countryName,
                };
                _dbContext.Countries.Add(country);
            }
            user.Country = country;
            _dbContext.SaveChanges();

        }

        public FriendshipFact GetFriendshipFact(string user1Id, string user2Id)
        {
            FriendshipFact friendshipFact = _dbContext.FriendshipFacts.FirstOrDefault(ff => ff.InitiatorId == user1Id
                                                                                                &&
                                                                                            ff.InvitedId == user2Id
                                                                                            ||
                                                                                            ff.InitiatorId == user2Id
                                                                                                &&
                                                                                            ff.InvitedId == user1Id);
            return friendshipFact;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="invitorId"></param>
        /// <param name="invitedId"></param>
        /// <exception cref="FriendshipException"></exception>
        public NetworkUser InviteFriend(string invitorId, string invitedId)
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

            if (friendshipFactIn != null && !friendshipFactIn.RequestAccepted) throw new FriendshipException("Инициатор уже приглашен");
            if (friendshipFactOut != null && !friendshipFactOut.RequestAccepted) throw new FriendshipException("Инициатор повторно приглашает пользователя");
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
            return invited;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="invitorId"></param>
        /// <param name="invitedId"></param>
        /// <exception cref="FriendshipException"></exception>
        public NetworkUser AcceptFriendship(string invitorId, string invitedId)
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
            FriendshipFact friendshipFactOut = invited.FriendshipFactsOut.FirstOrDefault(fact => fact.InitiatorId == invitedId && fact.InvitedId == invitorId);

            if (friendshipFactOut != null) throw new FriendshipException("Приглашение отплавлено данным пользователем(принимать должен второй пользователь)");
            if (friendshipFactIn == null) throw new FriendshipException("Приглашение дружить не поступало");
            if (friendshipFactIn.RequestAccepted) throw new FriendshipException("Приглашение о дружбе уже было принято");

            friendshipFactIn.DateOfConclusion = DateTime.Now;
            friendshipFactIn.RequestAccepted = true;
            _dbContext.SaveChanges();
            return invitor;
        }
        public NetworkUser DeleteFriend(string userId, string friendId)
        {
            NetworkUser user = _dbContext.Users.FirstOrDefault(user => user.Id == userId);
            if (user == null) throw new NetworkUserException("Пользователь не существует");
            NetworkUser friend = _dbContext.Users.FirstOrDefault(user => user.Id == friendId);
            if (friend == null) throw new NetworkUserException("Удаляемый из друзей пользователь не существует");

            var friendshipFactQuery = from ff in _dbContext.FriendshipFacts
                                      where (ff.InitiatorId == userId && ff.InvitedId == friendId)
                                          ||
                                      (ff.InvitedId == userId && ff.InitiatorId == friendId)
                                      select ff;
            var friendshipFact = friendshipFactQuery.FirstOrDefault();
            if (friendshipFact == null) throw new FriendshipException("Пользователи не состоят в дружбе");
            _dbContext.FriendshipFacts.Remove(friendshipFact);
            _dbContext.SaveChanges();
            return friend;
        }
        public List<InterlocutorViewModel> GetFriends(string userId)
        {
            IQueryable<string> friendsIn = from user in _dbContext.Users
                                                          join ff in _dbContext.FriendshipFacts on user.Id equals ff.InitiatorId
                                                          where ff.InvitedId == userId && ff.RequestAccepted == true
                                                          select ff.InitiatorId;

            IQueryable<string> friendsOut = from user in _dbContext.Users
                                                           join ff in _dbContext.FriendshipFacts on user.Id equals ff.InvitedId
                                                           where ff.InitiatorId == userId && ff.RequestAccepted == true
                                                           select ff.InvitedId;

            IQueryable<InterlocutorViewModel> friends = from _user in _dbContext.Users

                                                         where friendsIn.Contains(_user.Id)
                                                                  ||
                                                                friendsOut.Contains(_user.Id)

                                                         select new InterlocutorViewModel
                                                         {
                                                             Id = _user.Id,
                                                             UserName = _user.UserName,
                                                             UserPageLink = $"/User{_user.Id}"
                                                         };

            return friends.ToList();
        }

        public List<InterlocutorViewModel> GetInterlocutors(string userId)
        {
            IQueryable<string> friendsIn = from user in _dbContext.Users
                                                          join ff in _dbContext.FriendshipFacts on user.Id equals ff.InitiatorId
                                                          where ff.InvitedId == userId && ff.RequestAccepted == true
                                                          select user.Id;
            IQueryable<string> friendsOut = from user in _dbContext.Users
                                                           join ff in _dbContext.FriendshipFacts on user.Id equals ff.InvitedId
                                                           where ff.InitiatorId == userId && ff.RequestAccepted == true
                                                           select user.Id;
            IQueryable<InterlocutorViewModel> interlocutors = from user in _dbContext.Users
                                                              join mic in _dbContext.MembershipInChats on user.Id equals mic.UserId
                                                              where
                                                              (from chat in _dbContext.Chats
                                                               join userMic in _dbContext.MembershipInChats on chat.Id equals userMic.ChatId
                                                               where userMic.UserId == userId && chat.Name == ""
                                                               select chat.Id).Contains(mic.ChatId) && user.Id != userId && !friendsIn.Contains(user.Id) && !friendsOut.Contains(user.Id)
                                                              select new InterlocutorViewModel()
                                                              {
                                                                  Id = user.Id,
                                                                  ChatId = mic.ChatId,
                                                                  UserName = user.UserName,
                                                                  UserPageLink = $"/User{user.Id}"
                                                              };

            var result = interlocutors.ToList(); 
            return result;
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
