using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class ChangeHitoCurrency : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CrmDevelopmentId",
                schema: "app",
                table: "Currencies");

            migrationBuilder.RenameColumn(
                name: "CrmProductionId",
                schema: "app",
                table: "Currencies",
                newName: "CrmId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CrmId",
                schema: "app",
                table: "Currencies",
                newName: "CrmProductionId");

            migrationBuilder.AddColumn<string>(
                name: "CrmDevelopmentId",
                schema: "app",
                table: "Currencies",
                maxLength: 100,
                nullable: true);
        }
    }
}
