using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class RefactorResourceBilling3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResourceBillings_Seniorities_SeniorityId",
                schema: "app",
                table: "ResourceBillings");

            migrationBuilder.AlterColumn<int>(
                name: "SeniorityId",
                schema: "app",
                table: "ResourceBillings",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_ResourceBillings_Seniorities_SeniorityId",
                schema: "app",
                table: "ResourceBillings",
                column: "SeniorityId",
                principalSchema: "app",
                principalTable: "Seniorities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResourceBillings_Seniorities_SeniorityId",
                schema: "app",
                table: "ResourceBillings");

            migrationBuilder.AlterColumn<int>(
                name: "SeniorityId",
                schema: "app",
                table: "ResourceBillings",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ResourceBillings_Seniorities_SeniorityId",
                schema: "app",
                table: "ResourceBillings",
                column: "SeniorityId",
                principalSchema: "app",
                principalTable: "Seniorities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
