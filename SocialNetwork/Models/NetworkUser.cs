using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

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
        
        public ICollection<FriendshipFact> FriendshipFactsIn { get; set; }

        public ICollection<FriendshipFact> FriendshipFactsOut { get; set; }
        /// <summary>
        /// Пригласившие друзья
        /// </summary>
        public ICollection<NetworkUser> FriendsIn { get; set; }
        /// <summary>
        /// Приглашенные друзья
        /// </summary>
        public ICollection<NetworkUser> FriendsOut { get; set; }
        public ICollection<GroupChat> Chats { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }

        public DateTime? BirthDate { get; set; }

        public City City { get; set; }

        public int? CityId { get; set; }

    }
}
