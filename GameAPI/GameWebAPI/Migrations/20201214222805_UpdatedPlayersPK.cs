using Microsoft.EntityFrameworkCore.Migrations;

namespace GameWebAPI.Migrations
{
    public partial class UpdatedPlayersPK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Players",
                table: "Players");

            migrationBuilder.DropIndex(
                name: "IX_Players_username",
                table: "Players");

            migrationBuilder.AlterColumn<string>(
                name: "username",
                table: "Players",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "Players",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Players",
                table: "Players",
                column: "username");

            migrationBuilder.CreateIndex(
                name: "IX_Players_email",
                table: "Players",
                column: "email",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Players",
                table: "Players");

            migrationBuilder.DropIndex(
                name: "IX_Players_email",
                table: "Players");

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "Players",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "username",
                table: "Players",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Players",
                table: "Players",
                column: "email");

            migrationBuilder.CreateIndex(
                name: "IX_Players_username",
                table: "Players",
                column: "username",
                unique: true);
        }
    }
}
