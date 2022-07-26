using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using System;
using System.Data.Common;

namespace SocialNetwork.Models
{
    public class SocialNetworkDbContext: IdentityDbContext<NetworkUser>
    {

        public SocialNetworkDbContext(DbContextOptions<SocialNetworkDbContext> options) : base(options) { }

        public DbSet<Message> Messages { get; set; }
        public DbSet<GroupChat> Chats { get; set; }

        public DbSet<Country> Countries { get; set; }

        public DbSet<City> Cities { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<NetworkUser>()
                    .HasMany(user => user.Friends)
                    .WithMany(user => user.Friends)
                    .UsingEntity<FriendshipFact>(
                    j => j
                    .HasOne(f => f.Friend2)
                    .WithMany(user => user.FriendshipFacts)
                    .HasForeignKey(f => f.Friend2Id),
                    j => j
                    .HasOne(f => f.Friend1)
                    .WithMany(user => user.FriendshipFacts)
                    .HasForeignKey(f => f.Friend1Id),
                    j =>
                    {
                        j.HasKey(fr => new { fr.Friend1Id, fr.Friend2Id });
                        j.ToTable(nameof(FriendshipFact) + 's');
                    });


                
        }

    }
}
