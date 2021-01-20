using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GameWebAPI.Migrations
{
    public partial class BadMergeFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "birthDate",
                table: "Players");

            migrationBuilder.AddColumn<DateTime>(
                name: "date",
                table: "PlayersMatchdata",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "gameType",
                table: "GameInvites",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "date",
                table: "PlayersMatchdata");

            migrationBuilder.DropColumn(
                name: "gameType",
                table: "GameInvites");

            migrationBuilder.AddColumn<DateTime>(
                name: "birthDate",
                table: "Players",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
