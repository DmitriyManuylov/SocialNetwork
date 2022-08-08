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

        public DbSet<SimpleMessage> SimpleMessages { get; set; }
        public DbSet<LiteChatRoom> Rooms { get; set; }

        public DbSet<MembershipInChat> MembershipInChats { get; set; }

        public DbSet<FriendshipFact> FriendshipFacts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<NetworkUser>().Property(user => user.UserName).IsRequired();
            builder.Entity<NetworkUser>().Property(user => user.PasswordHash).IsRequired();
            builder.Entity<NetworkUser>()
                .HasMany(user => user.FriendsIn)
                .WithMany(user => user.FriendsOut)
                .UsingEntity<FriendshipFact>(
                 ff => ff
                    .HasOne(ff => ff.Initiator)
                    .WithMany(user => user.FriendshipFactsIn)
                    .HasForeignKey(ff => ff.InitiatorId)
                    .OnDelete(DeleteBehavior.ClientCascade),
                 ff => ff
                    .HasOne(ff => ff.Invited)
                    .WithMany(user => user.FriendshipFactsOut)
                    .HasForeignKey(ff => ff.InvitedId)
                    .OnDelete(DeleteBehavior.ClientCascade),
                 ff => ff.ToTable(nameof(FriendshipFact) + 's'));

            builder.Entity<NetworkUser>()
                .HasMany(user => user.Chats)
                .WithMany(chat => chat.Users)
                .UsingEntity<MembershipInChat>(
                mic => mic
                    .HasOne(mic => mic.Chat)
                    .WithMany(chat => chat.MembershipInChats)
                    .HasForeignKey(chat => chat.ChatId),
                mic => mic
                    .HasOne(mic => mic.User)
                    .WithMany(user => user.MembershipInChats)
                    .HasForeignKey(mic => mic.UserId),
                mic => mic.ToTable("MembershipInChats"));
        }

    }
}
