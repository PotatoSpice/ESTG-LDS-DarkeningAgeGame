using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GameWebAPI.Migrations
{
    public partial class AddedGameInviteSystem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GameInvites",
                columns: table => new
                {
                    invitedId = table.Column<string>(type: "TEXT", nullable: false),
                    roomId = table.Column<string>(type: "TEXT", nullable: true),
                    hostId = table.Column<string>(type: "TEXT", nullable: true),
                    createDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameInvites", x => x.invitedId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameInvites");
        }
    }
}
