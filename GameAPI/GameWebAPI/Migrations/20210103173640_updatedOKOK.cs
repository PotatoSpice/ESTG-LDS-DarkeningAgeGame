using Microsoft.EntityFrameworkCore.Migrations;

namespace GameWebAPI.Migrations
{
    public partial class updatedOKOK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_GameInvites",
                table: "GameInvites");

            migrationBuilder.AlterColumn<string>(
                name: "roomId",
                table: "GameInvites",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_GameInvites",
                table: "GameInvites",
                columns: new[] { "roomId", "invitedId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_GameInvites",
                table: "GameInvites");

            migrationBuilder.AlterColumn<string>(
                name: "roomId",
                table: "GameInvites",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GameInvites",
                table: "GameInvites",
                column: "invitedId");
        }
    }
}
