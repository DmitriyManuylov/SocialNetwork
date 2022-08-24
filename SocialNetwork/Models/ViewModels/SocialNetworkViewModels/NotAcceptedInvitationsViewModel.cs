using System.Collections.Generic;

namespace SocialNetwork.Models.ViewModels.SocialNetworkViewModels
{
    public class NotAcceptedInvitationsViewModel
    {
        public List<InterlocutorViewModel> IncomingInvitations { get; set; }
        public List<InterlocutorViewModel> OutgoingInvitations { get; set; }
    }
}
