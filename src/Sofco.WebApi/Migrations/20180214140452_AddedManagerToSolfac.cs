using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class AddedManagerToSolfac : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Manager",
                schema: "app",
                table: "Solfacs",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ManagerId",
                schema: "app",
                table: "Solfacs",
                maxLength: 100,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Manager",
                schema: "app",
                table: "Solfacs");

            migrationBuilder.DropColumn(
                name: "ManagerId",
                schema: "app",
                table: "Solfacs");
        }
    }
}
