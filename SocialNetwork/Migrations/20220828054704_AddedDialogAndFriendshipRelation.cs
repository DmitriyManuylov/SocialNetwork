using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialNetwork.Migrations
{
    public partial class AddedDialogAndFriendshipRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Dialogs_ChatId",
                table: "Dialogs");

            migrationBuilder.DropIndex(
                name: "IX_Dialogs_User1Id",
                table: "Dialogs");

            migrationBuilder.DropIndex(
                name: "IX_Dialogs_User2Id",
                table: "Dialogs");

            migrationBuilder.AddColumn<int>(
                name: "DialogId",
                table: "FriendshipFacts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FriendshipFacts_DialogId",
                table: "FriendshipFacts",
                column: "DialogId",
                unique: true,
                filter: "[DialogId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Dialogs_ChatId",
                table: "Dialogs",
                column: "ChatId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dialogs_User1Id",
                table: "Dialogs",
                column: "User1Id");

            migrationBuilder.CreateIndex(
                name: "IX_Dialogs_User2Id",
                table: "Dialogs",
                column: "User2Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FriendshipFacts_Dialogs_DialogId",
                table: "FriendshipFacts",
                column: "DialogId",
                principalTable: "Dialogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FriendshipFacts_Dialogs_DialogId",
                table: "FriendshipFacts");

            migrationBuilder.DropIndex(
                name: "IX_FriendshipFacts_DialogId",
                table: "FriendshipFacts");

            migrationBuilder.DropIndex(
                name: "IX_Dialogs_ChatId",
                table: "Dialogs");

            migrationBuilder.DropIndex(
                name: "IX_Dialogs_User1Id",
                table: "Dialogs");

            migrationBuilder.DropIndex(
                name: "IX_Dialogs_User2Id",
                table: "Dialogs");

            migrationBuilder.DropColumn(
                name: "DialogId",
                table: "FriendshipFacts");

            migrationBuilder.CreateIndex(
                name: "IX_Dialogs_ChatId",
                table: "Dialogs",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_Dialogs_User1Id",
                table: "Dialogs",
                column: "User1Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dialogs_User2Id",
                table: "Dialogs",
                column: "User2Id",
                unique: true);
        }
    }
}
