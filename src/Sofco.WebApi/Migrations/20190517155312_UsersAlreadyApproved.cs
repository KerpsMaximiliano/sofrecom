using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class UsersAlreadyApproved : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UsersAlreadyApproved",
                schema: "app",
                table: "Refunds",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsersAlreadyApproved",
                schema: "app",
                table: "Advancements",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UsersAlreadyApproved",
                schema: "app",
                table: "Refunds");

            migrationBuilder.DropColumn(
                name: "UsersAlreadyApproved",
                schema: "app",
                table: "Advancements");
        }
    }
}
