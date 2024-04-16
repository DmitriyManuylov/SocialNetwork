using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using System.Text.Json;
using SocialNetwork.Models.ChatModels;
using SocialNetwork.Models.LiteChatModels;
using SocialNetwork.Models.UserInfoModels;
using System;
using System.Data.Common;
using System.Threading.Tasks;
using SocialNetwork.ServiceEntities;
using System.IO;

namespace SocialNetwork.Models
{
    public class SocialNetworkDbContext: IdentityDbContext<NetworkUser>
    {
        public SocialNetworkDbContext(DbContextOptions<SocialNetworkDbContext> options) : base(options) {

        }

        public DbSet<Message> Messages { get; set; }
        public DbSet<GroupChat> Chats { get; set; }

        public DbSet<Country> Countries { get; set; }

        public DbSet<City> Cities { get; set; }

        public DbSet<SimpleMessage> SimpleMessages { get; set; }
        public DbSet<LiteChatRoom> Rooms { get; set; }

        public DbSet<MembershipInChat> MembershipInChats { get; set; }

        public DbSet<FriendshipFact> FriendshipFacts { get; set; }

        public DbSet<Dialog> Dialogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            ConfigurateTables(builder);
            FillStartData(builder);
           
        }

        private void FillStartData(ModelBuilder builder)
        {
            StartDataModel startDataModel;
            using (FileStream fs = new FileStream("StartData.json", FileMode.Open))
            {
                startDataModel = JsonSerializer.DeserializeAsync<StartDataModel>(fs).Result;
            }

            builder.Entity<Country>().HasData(startDataModel.Countries);

            builder.Entity<City>().HasData(startDataModel.Cities);

            builder.Entity<GroupChat>().HasData(startDataModel.Chats);

        }
        private void ConfigurateTables(ModelBuilder builder)
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
                    .WithMany(user => user.FriendshipFactsOut)
                    .HasForeignKey(ff => ff.InitiatorId)
                    .OnDelete(DeleteBehavior.ClientCascade),
                 ff => ff
                    .HasOne(ff => ff.Invited)
                    .WithMany(user => user.FriendshipFactsIn)
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
            builder.Entity<Dialog>().HasOne(dialog => dialog.User1).WithOne().OnDelete(DeleteBehavior.ClientCascade);
            builder.Entity<Dialog>().HasOne(dialog => dialog.User2).WithOne().OnDelete(DeleteBehavior.ClientCascade);
            builder.Entity<FriendshipFact>().HasOne(ff => ff.Dialog).WithOne().OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Dialog>().HasIndex(dialog => dialog.User1Id).IsUnique(false);
            builder.Entity<Dialog>().HasIndex(dialog => dialog.User2Id).IsUnique(false);
            builder.Entity<Dialog>().HasIndex(dialog => dialog.ChatId).IsUnique(true);

        }
    }
}
