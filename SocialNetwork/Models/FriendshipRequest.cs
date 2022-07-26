namespace SocialNetwork.Models
{
    public class FriendshipRequest
    {
        public int FriendshipRequestId { get; set; }
        public string InitiatorId { get; set; }
        public NetworkUser Initiator { get; set; }

        public string InvitedId { get; set; }
        public NetworkUser Invited { get; set; }

    }
}
