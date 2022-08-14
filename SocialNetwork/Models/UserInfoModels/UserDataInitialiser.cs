using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using SocialNetwork.Models.ChatModels;
using SocialNetwork.Models.Repositories;

namespace SocialNetwork.Models.UserInfoModels
{
    public static class UserDataInitialiser
    {
        public static void Initialize(UserManager<NetworkUser> userManager, 
                                      SocialNetworkDbContext dbContext, 
                                      IUsersRepository usersRepository)
        {
            List<City> cities = dbContext.Cities.Take(13).ToList();
            List<Country> countries = dbContext.Countries.Take(5).ToList();
            List<GroupChat> chats = dbContext.Chats.Take(6).ToList();


            string nameIvanIvanov = "IvanIvanov";
            string nameIam = "DmitriyManuylov";
            string namePetrSidorov = "PetrSidorov";
            string nameSidorPetrov = "SidorPetrov";
            string nameKuznetsovVasiliy = "KuznetsovVasiliy";


            var user1 = userManager.FindByNameAsync(nameIvanIvanov).Result;
            if (user1 == null)
            {
                NetworkUser IvanIvanov;
                IvanIvanov = new NetworkUser(nameIvanIvanov, "Иван", "Иванов");
                IvanIvanov.Id = Guid.NewGuid().ToString();
                userManager.CreateAsync(IvanIvanov, "qWeQrt_13y").Wait();

                IvanIvanov.City = cities[1];
                IvanIvanov.Chats = new List<GroupChat>();
                IvanIvanov.Chats.Add(chats[1]);
                IvanIvanov.Chats.Add(chats[3]);
                dbContext.SaveChanges();
            }

            var user2 = userManager.FindByNameAsync(nameIam).Result;
            if (user2 == null)
            {
                NetworkUser Iam;
                Iam = new NetworkUser(nameIam, "Дмитрий", "Мануйлов", new DateTime(1997, 5, 13));
                Iam.Email = "dmitriy.manuylov@mail.ru";
                Iam.SetAge();
                Iam.Id = Guid.NewGuid().ToString();
                userManager.CreateAsync(Iam, "qWeQrt_14y").Wait();
                Iam.City = cities[2];
                Iam.Chats = new List<GroupChat>();
                Iam.Chats.Add(chats[2]);
                Iam.Chats.Add(chats[3]);
                Iam.Chats.Add(chats[4]);

                NetworkUser IvanIvanov = userManager.FindByNameAsync(nameIvanIvanov).Result;

                usersRepository.InviteFriend(Iam.Id, IvanIvanov.Id);
                usersRepository.AcceptFriendship(Iam.Id, IvanIvanov.Id);

                dbContext.SaveChanges();
            }
            

            var user3 = userManager.FindByNameAsync(namePetrSidorov).Result;
            if (user3 == null)
            {
                NetworkUser Petr_Sidorov = new NetworkUser(namePetrSidorov, "Пётр", "Сидоров", new DateTime(1994, 9, 23));
                Petr_Sidorov.Id = Guid.NewGuid().ToString();
                Petr_Sidorov.SetAge();
                Petr_Sidorov.City = cities[5];
                Petr_Sidorov.Chats = new List<GroupChat>();
                Petr_Sidorov.Chats.Add(chats[5]);
                Petr_Sidorov.Chats.Add(chats[0]);
                Petr_Sidorov.Chats.Add(chats[1]);
                userManager.CreateAsync(Petr_Sidorov, "qWeQrt_15y").Wait();
                NetworkUser Iam = userManager.FindByNameAsync(nameIam).Result;
                usersRepository.InviteFriend(Petr_Sidorov.Id, Iam.Id);
                usersRepository.AcceptFriendship(Petr_Sidorov.Id, Iam.Id);
            }

            var user4 = userManager.FindByNameAsync(nameSidorPetrov).Result;
            if (user4 == null)
            {
                NetworkUser Sidor_Petrov = new NetworkUser(nameSidorPetrov, "Сидор", "Петров", new DateTime(1987, 2, 7));
                Sidor_Petrov.Id = Guid.NewGuid().ToString();
                Sidor_Petrov.SetAge();
                Sidor_Petrov.City = cities[3];
                Sidor_Petrov.Chats = new List<GroupChat>();
                Sidor_Petrov.Chats.Add(chats[4]);
                Sidor_Petrov.Chats.Add(chats[2]);
                Sidor_Petrov.Chats.Add(chats[3]);
                userManager.CreateAsync(Sidor_Petrov, "qWeQrt_16y").Wait();
                NetworkUser Iam = userManager.FindByNameAsync(nameIam).Result;
                usersRepository.InviteFriend(Sidor_Petrov.Id, Iam.Id);
                usersRepository.AcceptFriendship(Sidor_Petrov.Id, Iam.Id);

                NetworkUser IvanIvanov = userManager.FindByNameAsync(nameIvanIvanov).Result;
                usersRepository.InviteFriend(Sidor_Petrov.Id, IvanIvanov.Id);
                usersRepository.AcceptFriendship(Sidor_Petrov.Id, IvanIvanov.Id);
            }

            var user5 = userManager.FindByNameAsync(nameKuznetsovVasiliy).Result;
            if (user5 == null)
            {
                NetworkUser Vasiliy_Kuznetsov = new NetworkUser(nameKuznetsovVasiliy, "Василий", "Кузнецов", new DateTime(2001, 11, 17));
                Vasiliy_Kuznetsov.Id = Guid.NewGuid().ToString();
                Vasiliy_Kuznetsov.SetAge();
                Vasiliy_Kuznetsov.City = cities[6];
                Vasiliy_Kuznetsov.Chats = new List<GroupChat>();
                Vasiliy_Kuznetsov.Chats.Add(chats[4]);
                Vasiliy_Kuznetsov.Chats.Add(chats[0]);
                userManager.CreateAsync(Vasiliy_Kuznetsov, "qWeQrt_17y").Wait();
                NetworkUser Iam = userManager.FindByNameAsync(nameIam).Result;
                NetworkUser IvanIvanov = userManager.FindByNameAsync(nameIvanIvanov).Result;
                NetworkUser SidorPetrov = userManager.FindByNameAsync(nameSidorPetrov).Result;
                usersRepository.InviteFriend(Iam.Id, Vasiliy_Kuznetsov.Id);
                usersRepository.AcceptFriendship(Iam.Id, Vasiliy_Kuznetsov.Id);

                usersRepository.InviteFriend(IvanIvanov.Id, Vasiliy_Kuznetsov.Id);
                usersRepository.AcceptFriendship(IvanIvanov.Id, Vasiliy_Kuznetsov.Id);

                usersRepository.InviteFriend(Vasiliy_Kuznetsov.Id, SidorPetrov.Id);
                usersRepository.AcceptFriendship(Vasiliy_Kuznetsov.Id, SidorPetrov.Id);
            }

        }
    }
}
