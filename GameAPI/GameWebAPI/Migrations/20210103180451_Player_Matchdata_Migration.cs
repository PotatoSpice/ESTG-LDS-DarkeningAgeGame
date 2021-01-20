using Microsoft.EntityFrameworkCore.Migrations;

namespace GameWebAPI.Migrations
{
    public partial class Player_Matchdata_Migration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlayersMatchdata",
                columns: table => new
                {
                    playerId = table.Column<string>(type: "TEXT", nullable: false),
                    gameID = table.Column<string>(type: "TEXT", nullable: false),
                    placement = table.Column<int>(type: "INTEGER", nullable: false),
                    armiesCreated = table.Column<int>(type: "INTEGER", nullable: false),
                    regionsConquered = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayersMatchdata", x => new { x.playerId, x.gameID });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayersMatchdata");
        }
    }
}
