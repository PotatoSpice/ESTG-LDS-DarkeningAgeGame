using Microsoft.EntityFrameworkCore.Migrations;

namespace GameWebAPI.Migrations
{
    public partial class AddedFriendsSystem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "online",
                table: "Players",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "FriendsList",
                columns: table => new
                {
                    friendId = table.Column<string>(type: "TEXT", nullable: false),
                    playerId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FriendsList", x => x.friendId);
                });

            migrationBuilder.CreateTable(
                name: "FriendsRequests",
                columns: table => new
                {
                    targetPlayerId = table.Column<string>(type: "TEXT", nullable: false),
                    playerId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FriendsRequests", x => x.targetPlayerId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FriendsList");

            migrationBuilder.DropTable(
                name: "FriendsRequests");

            migrationBuilder.DropColumn(
                name: "online",
                table: "Players");
        }
    }
}
