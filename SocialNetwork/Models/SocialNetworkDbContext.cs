using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using SocialNetwork.Models.ChatModels;
using SocialNetwork.Models.LiteChatModels;
using SocialNetwork.Models.UserInfoModels;
using System;
using System.Data.Common;
using System.Threading.Tasks;

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
            Country Russia = new Country() {Id = 1, Name = "Россия" };
            Country Belalus = new Country() { Id = 2, Name = "Белоруссия" };
            Country Germany = new Country() { Id = 3, Name = "Германия" };
            Country China = new Country() { Id = 4, Name = "Китай" };
            Country France = new Country() { Id = 5, Name = "Франция" };

            builder.Entity<Country>().HasData(Russia, 
                                              Belalus, 
                                              Germany, 
                                              China, 
                                              France);

            City Sanct_Petersburg = new City() { Id = 1, Name = "Санкт-Петербург", CountryId = Russia.Id };
            City Moscow = new City() { Id = 2, Name = "Москва", CountryId = Russia.Id };
            City Rostov_on_Don = new City { Id = 3, Name = "Ростов-на-Дону", CountryId = Russia.Id };
            City Sochi = new City() { Id = 4, Name = "Сочи", CountryId = Russia.Id };
            City Saratov = new City() { Id = 5, Name = "Саратов", CountryId = Russia.Id };
            City Irkutsk = new City() { Id = 6, Name = "Иркутск", CountryId = Russia.Id };
            City Minsk = new City() { Id = 7, Name = "Минск", CountryId = Belalus.Id };
            City Gomel = new City() { Id = 8, Name = "Гомель", CountryId = Belalus.Id };
            City Berlin = new City() { Id = 9, Name = "Берлин", CountryId = Germany.Id };
            City Munhen = new City() { Id = 10, Name = "Мюнхен", CountryId = Germany.Id };
            City Pekin = new City() { Id = 11, Name = "Пекин", CountryId = China.Id };
            City Uhan = new City() { Id = 12, Name = "Ухань", CountryId = China.Id };
            City Paris = new City() { Id = 13, Name = "Париж", CountryId = France.Id };

            builder.Entity<City>().HasData(Sanct_Petersburg,
                                           Moscow,
                                           Rostov_on_Don,
                                           Sochi,
                                           Saratov,
                                           Irkutsk,
                                           Minsk,
                                           Gomel,
                                           Berlin,
                                           Munhen,
                                           Pekin,
                                           Uhan,
                                           Paris);

            GroupChat football_Rostov = new GroupChat() {Id = 1, Name = "Пинатели мяча в Ростове-на-Дону" };
            GroupChat basketball_Saratov = new GroupChat() { Id = 2, Name = "Швырятели мяча в Саратове" };
            GroupChat valleyball_Sochi = new GroupChat() { Id = 3, Name = "Пинатели мяча руками в Сочи" };
            GroupChat csTeam_Streltsi = new GroupChat() { Id = 4, Name = "Любители погонять в CS \"Стрельцы\"" };
            GroupChat wot_Team_Ne_Probil = new GroupChat() { Id = 5, Name = "Клуб любителей World of Tanks - \"Не пробил\"" };
            GroupChat dog_Breeders_Moscow = new GroupChat() { Id = 6, Name = "Собаководы Москвы" };

            builder.Entity<GroupChat>().HasData(football_Rostov,
                                                basketball_Saratov,
                                                valleyball_Sochi,
                                                csTeam_Streltsi,
                                                wot_Team_Ne_Probil,
                                                dog_Breeders_Moscow);

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
