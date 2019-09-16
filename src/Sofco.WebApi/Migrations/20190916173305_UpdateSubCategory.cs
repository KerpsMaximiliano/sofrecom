using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class UpdateSubCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrencyId",
                schema: "app",
                table: "CostDetailStaff",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "OriginalValue",
                schema: "app",
                table: "CostDetailStaff",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_CostDetailStaff_CurrencyId",
                schema: "app",
                table: "CostDetailStaff",
                column: "CurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_CostDetailStaff_Currencies_CurrencyId",
                schema: "app",
                table: "CostDetailStaff",
                column: "CurrencyId",
                principalSchema: "app",
                principalTable: "Currencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CostDetailStaff_Currencies_CurrencyId",
                schema: "app",
                table: "CostDetailStaff");

            migrationBuilder.DropIndex(
                name: "IX_CostDetailStaff_CurrencyId",
                schema: "app",
                table: "CostDetailStaff");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                schema: "app",
                table: "CostDetailStaff");

            migrationBuilder.DropColumn(
                name: "OriginalValue",
                schema: "app",
                table: "CostDetailStaff");
        }
    }
}
