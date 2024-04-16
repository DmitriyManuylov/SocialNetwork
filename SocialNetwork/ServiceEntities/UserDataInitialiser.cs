using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using SocialNetwork.Models.ChatModels;
using SocialNetwork.Models.Repositories;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.Models;
using SocialNetwork.Models.UserInfoModels;
using SocialNetwork.ServiceEntities;
using System.IO;
using System.Text.Json;
using SocialNetwork.Models.ViewModels.AccountViewModels;

namespace SocialNetwork.ServiceEntitiies
{
    public static class UserDataInitialiser
    {
        public static void Initialize(UserManager<NetworkUser> userManager,
                                      SocialNetworkDbContext dbContext,
                                      IUsersRepository usersRepository)
        {
            List<NetworkUser> users = userManager.Users.Take(1).ToList();
            if (users.Count > 0) return;
            StartDataModel startDataModel;
            using (FileStream fs = new FileStream("StartData.json", FileMode.Open))
            {
                startDataModel = JsonSerializer.DeserializeAsync<StartDataModel>(fs).Result;
            }
            List<City> cities = dbContext.Cities.Take(13).Include(city => city.Country).ToList();
            List<GroupChat> chats = dbContext.Chats.Take(6).ToList();

            NetworkUser networkUser;
            foreach (UserStartInitializationModel user in startDataModel.Users)
            {
                networkUser = new NetworkUser(user.Name, user.FirstName, user.LastName);
                userManager.CreateAsync(networkUser, user.Password).Wait();
                networkUser.City = cities.First(city => city.Id == user.CityId);
                if (networkUser.City.Country != null)
                {
                    networkUser.Country = networkUser.City.Country;
                }
                networkUser.Gender = user.Gender;
                networkUser.Chats = chats.Where(chat => user.Chats.Contains(chat.Id)).ToList();
                dbContext.SaveChanges();
            }

            users = userManager.Users.ToList();

            foreach(Friendship friendshipFact in startDataModel.FriendshipFacts)
            {
                usersRepository.InviteFriend(users[friendshipFact.InitiatorId].Id, users[friendshipFact.InvitedId].Id);
                usersRepository.AcceptFriendship(users[friendshipFact.InitiatorId].Id, users[friendshipFact.InvitedId].Id);
                dbContext.SaveChanges();
            }

        }
    }
}
