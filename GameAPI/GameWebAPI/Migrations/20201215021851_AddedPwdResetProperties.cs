using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GameWebAPI.Migrations
{
    public partial class AddedPwdResetProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "pwdResetToken",
                table: "Players",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "pwdResetTokenExpires",
                table: "Players",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "pwdResetToken",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "pwdResetTokenExpires",
                table: "Players");
        }
    }
}
