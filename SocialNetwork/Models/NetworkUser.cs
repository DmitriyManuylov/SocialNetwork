using Microsoft.AspNetCore.Identity;
using SocialNetwork.Models.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SocialNetwork.Models
{
    public class NetworkUser : IdentityUser
    {
        public NetworkUser(string userName) : base(userName) { }

        public NetworkUser(string userName
                            , string firstName = null
                            , string surname = null
                            , DateTime? birthDate = null
                            , int? cityId = null) : base(userName)
        {
            FirstName = firstName;
            Surname = surname;
            BirthDate = birthDate;
            CityId = cityId;
        }
        
        public List<MembershipInChat> MembershipInChats { get; set; }
        public List<FriendshipFact> FriendshipFactsIn { get; set; }

        public List<FriendshipFact> FriendshipFactsOut { get; set; }
        /// <summary>
        /// Пригласившие друзья
        /// </summary>
        public List<NetworkUser> FriendsIn { get; set; }
        /// <summary>
        /// Приглашенные друзья
        /// </summary>
        public List<NetworkUser> FriendsOut { get; set; }
        public List<GroupChat> Chats { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }

        public DateTime? BirthDate { get; set; }

        public int? Age { get; set; }

        public City City { get; set; }

        public int? CityId { get; set; }

        public Country Country { get; set; }

        public int? CountryId { get; set; }

        public int SetAge()
        {
            if(!BirthDate.HasValue) throw new NetworkUserException("Не задана дата рождения пользователя");
            DateTime now = DateTime.Now;
            int age = DateTime.Now.Year - BirthDate.Value.Year;
            DateTime birthDateInCurrentYear = new DateTime(now.Year, BirthDate.Value.Month, BirthDate.Value.Day);
            if (birthDateInCurrentYear < now) age--;
            Age = age;
            return age;
        }

    }
}
