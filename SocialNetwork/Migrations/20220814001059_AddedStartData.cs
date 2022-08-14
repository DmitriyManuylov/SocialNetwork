using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialNetwork.Migrations
{
    public partial class AddedStartData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MembershipInChats",
                table: "MembershipInChats");

            migrationBuilder.DropIndex(
                name: "IX_MembershipInChats_ChatId",
                table: "MembershipInChats");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MembershipInChats",
                table: "MembershipInChats",
                columns: new[] { "ChatId", "UserId" });

            migrationBuilder.InsertData(
                table: "Chats",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Пинатели мяча в Ростове-на-Дону" },
                    { 2, "Швырятели мяча в Саратове" },
                    { 3, "Пинатели мяча руками в Сочи" },
                    { 4, "Любители погонять в CS \"Стрельцы\"" },
                    { 5, "Клуб любителей World of Tanks - \"Не пробил\"" },
                    { 6, "Собаководы Москвы" }
                });

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Россия" },
                    { 2, "Белоруссия" },
                    { 3, "Германия" },
                    { 4, "Китай" },
                    { 5, "Франция" }
                });

            migrationBuilder.InsertData(
                table: "Cities",
                columns: new[] { "Id", "CountryId", "Name" },
                values: new object[,]
                {
                    { 1, 1, "Санкт-Петербург" },
                    { 2, 1, "Москва" },
                    { 3, 1, "Ростов-на-Дону" },
                    { 4, 1, "Сочи" },
                    { 5, 1, "Саратов" },
                    { 6, 1, "Иркутск" },
                    { 7, 2, "Минск" },
                    { 8, 2, "Гомель" },
                    { 9, 3, "Берлин" },
                    { 10, 3, "Мюнхен" },
                    { 11, 4, "Пекин" },
                    { 12, 4, "Ухань" },
                    { 13, 5, "Париж" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_MembershipInChats_UserId",
                table: "MembershipInChats",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MembershipInChats",
                table: "MembershipInChats");

            migrationBuilder.DropIndex(
                name: "IX_MembershipInChats_UserId",
                table: "MembershipInChats");

            migrationBuilder.DeleteData(
                table: "Chats",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Chats",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Chats",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Chats",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Chats",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Chats",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MembershipInChats",
                table: "MembershipInChats",
                columns: new[] { "UserId", "ChatId" });

            migrationBuilder.CreateIndex(
                name: "IX_MembershipInChats_ChatId",
                table: "MembershipInChats",
                column: "ChatId");
        }
    }
}
