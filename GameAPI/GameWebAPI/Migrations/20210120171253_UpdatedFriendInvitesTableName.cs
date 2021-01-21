using Microsoft.EntityFrameworkCore.Migrations;

namespace GameWebAPI.Migrations
{
    public partial class UpdatedFriendInvitesTableName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FriendsRequests");

            migrationBuilder.CreateTable(
                name: "FriendInvites",
                columns: table => new
                {
                    playerId = table.Column<string>(type: "TEXT", nullable: false),
                    targetPlayerId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FriendInvites", x => new { x.playerId, x.targetPlayerId });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FriendInvites");

            migrationBuilder.CreateTable(
                name: "FriendsRequests",
                columns: table => new
                {
                    playerId = table.Column<string>(type: "TEXT", nullable: false),
                    targetPlayerId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FriendsRequests", x => new { x.playerId, x.targetPlayerId });
                });
        }
    }
}
