using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class SalaryDiscount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdvancementSalaryDiscounts_Advancements_AdvancementId",
                schema: "app",
                table: "AdvancementSalaryDiscounts");

            migrationBuilder.RenameColumn(
                name: "AdvancementId",
                schema: "app",
                table: "AdvancementSalaryDiscounts",
                newName: "EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_AdvancementSalaryDiscounts_AdvancementId",
                schema: "app",
                table: "AdvancementSalaryDiscounts",
                newName: "IX_AdvancementSalaryDiscounts_EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_AdvancementSalaryDiscounts_Employees_EmployeeId",
                schema: "app",
                table: "AdvancementSalaryDiscounts",
                column: "EmployeeId",
                principalSchema: "app",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdvancementSalaryDiscounts_Employees_EmployeeId",
                schema: "app",
                table: "AdvancementSalaryDiscounts");

            migrationBuilder.RenameColumn(
                name: "EmployeeId",
                schema: "app",
                table: "AdvancementSalaryDiscounts",
                newName: "AdvancementId");

            migrationBuilder.RenameIndex(
                name: "IX_AdvancementSalaryDiscounts_EmployeeId",
                schema: "app",
                table: "AdvancementSalaryDiscounts",
                newName: "IX_AdvancementSalaryDiscounts_AdvancementId");

            migrationBuilder.AddForeignKey(
                name: "FK_AdvancementSalaryDiscounts_Advancements_AdvancementId",
                schema: "app",
                table: "AdvancementSalaryDiscounts",
                column: "AdvancementId",
                principalSchema: "app",
                principalTable: "Advancements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
