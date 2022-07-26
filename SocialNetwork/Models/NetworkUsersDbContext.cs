using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace SocialNetwork.Models
{
    public class NetworkUsersDbContext: IdentityDbContext<NetworkUser>
    {
        public NetworkUsersDbContext(Microsoft.EntityFrameworkCore.DbContextOptions options) : base(options) { }
    }
}
