using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Models.Repositories;
using SocialNetwork.Models.ViewModels.SocialNetworkViewModels;

namespace SocialNetwork.Components
{
    public class SocialNetworkLinksViewComponent: ViewComponent
    {
        private IUsersRepository _usersRepository;
        public SocialNetworkLinksViewComponent(IUsersRepository usersRepsitory)
        {
            _usersRepository = usersRepsitory;
        }
        public IViewComponentResult Invoke()
        {
            NotAcceptedInvitationsViewModel notAcceptedInvitations = new NotAcceptedInvitationsViewModel();
            return View(notAcceptedInvitations);
        }
    }
}
