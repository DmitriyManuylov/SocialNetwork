using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Models.Repositories;
using SocialNetwork.Models.UserInfoModels;
using SocialNetwork.Models.ViewModels.UsersViewModels;

namespace SocialNetwork.Components
{
    [Authorize]
    public class SocialNetworkLinksViewComponent: ViewComponent
    {
        private UserManager<NetworkUser> _userManager;
        private IUsersRepository _usersRepository;
        public SocialNetworkLinksViewComponent(UserManager<NetworkUser> userManager, IUsersRepository usersRepsitory)
        {
            _userManager = userManager;
            _usersRepository = usersRepsitory;
        }
        public IViewComponentResult Invoke()
        {
            string userId = _userManager.GetUserId(UserClaimsPrincipal);
            NotAcceptedInvitationsViewModel notAcceptedInvitations = new NotAcceptedInvitationsViewModel()
            {
                IncomingInvitations = _usersRepository.GetIncomingFriendshipInvitations(userId),
                OutgoingInvitations = _usersRepository.GetOutgoingFriendshipInvitations(userId),
            };
            return View(notAcceptedInvitations);
        }
    }
}
