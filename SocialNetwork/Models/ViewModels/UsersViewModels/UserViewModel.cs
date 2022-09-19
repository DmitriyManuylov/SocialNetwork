using SocialNetwork.Models.UserInfoModels;

namespace SocialNetwork.Models.ViewModels.UsersViewModels
{
    public class UserViewModel
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public string UserPageLink { get; set; }

        public void SetFullName(NetworkUser user)
        {
            string fullName;

            if (user.FirstName != null)
            {
                fullName = user.FirstName;
                if (user.Surname != null)
                {
                    fullName += $" {user.Surname}";
                }
                fullName += $"({user.UserName})";
            }
            else
            {
                fullName = user.UserName;
            }

            UserName = fullName;
        }

    }
}
