using Microsoft.EntityFrameworkCore.Migrations;

namespace GameWebAPI.Migrations
{
    public partial class UpdatedPrimaryKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_FriendsRequests",
                table: "FriendsRequests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FriendsList",
                table: "FriendsList");

            migrationBuilder.AlterColumn<string>(
                name: "playerId",
                table: "FriendsRequests",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "playerId",
                table: "FriendsList",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_FriendsRequests",
                table: "FriendsRequests",
                columns: new[] { "playerId", "targetPlayerId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_FriendsList",
                table: "FriendsList",
                columns: new[] { "playerId", "friendId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_FriendsRequests",
                table: "FriendsRequests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FriendsList",
                table: "FriendsList");

            migrationBuilder.AlterColumn<string>(
                name: "playerId",
                table: "FriendsRequests",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "playerId",
                table: "FriendsList",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FriendsRequests",
                table: "FriendsRequests",
                column: "targetPlayerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FriendsList",
                table: "FriendsList",
                column: "friendId");
        }
    }
}
