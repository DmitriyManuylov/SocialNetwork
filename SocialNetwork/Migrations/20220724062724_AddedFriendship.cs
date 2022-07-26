using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialNetwork.Migrations
{
    public partial class AddedFriendship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupChatNetworkUser_Chats_chatsId",
                table: "GroupChatNetworkUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupChatNetworkUser",
                table: "GroupChatNetworkUser");

            migrationBuilder.DropIndex(
                name: "IX_GroupChatNetworkUser_chatsId",
                table: "GroupChatNetworkUser");

            migrationBuilder.RenameColumn(
                name: "chatsId",
                table: "GroupChatNetworkUser",
                newName: "ChatsId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupChatNetworkUser",
                table: "GroupChatNetworkUser",
                columns: new[] { "ChatsId", "UsersId" });

            migrationBuilder.CreateTable(
                name: "Friendship",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FriendId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DateOfConclusion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Friendship", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Friendship_AspNetUsers_FriendId",
                        column: x => x.FriendId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Friendship_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GroupChatNetworkUser_UsersId",
                table: "GroupChatNetworkUser",
                column: "UsersId");

            migrationBuilder.CreateIndex(
                name: "IX_Friendship_FriendId",
                table: "Friendship",
                column: "FriendId");

            migrationBuilder.CreateIndex(
                name: "IX_Friendship_UserId",
                table: "Friendship",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupChatNetworkUser_Chats_ChatsId",
                table: "GroupChatNetworkUser",
                column: "ChatsId",
                principalTable: "Chats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupChatNetworkUser_Chats_ChatsId",
                table: "GroupChatNetworkUser");

            migrationBuilder.DropTable(
                name: "Friendship");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupChatNetworkUser",
                table: "GroupChatNetworkUser");

            migrationBuilder.DropIndex(
                name: "IX_GroupChatNetworkUser_UsersId",
                table: "GroupChatNetworkUser");

            migrationBuilder.RenameColumn(
                name: "ChatsId",
                table: "GroupChatNetworkUser",
                newName: "chatsId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupChatNetworkUser",
                table: "GroupChatNetworkUser",
                columns: new[] { "UsersId", "chatsId" });

            migrationBuilder.CreateIndex(
                name: "IX_GroupChatNetworkUser_chatsId",
                table: "GroupChatNetworkUser",
                column: "chatsId");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupChatNetworkUser_Chats_chatsId",
                table: "GroupChatNetworkUser",
                column: "chatsId",
                principalTable: "Chats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
