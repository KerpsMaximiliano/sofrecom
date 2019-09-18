using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class currencycostdetailother3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrencyId",
                schema: "app",
                table: "CostDetailOthers",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_CostDetailOthers_CurrencyId",
                schema: "app",
                table: "CostDetailOthers",
                column: "CurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_CostDetailOthers_Currencies_CurrencyId",
                schema: "app",
                table: "CostDetailOthers",
                column: "CurrencyId",
                principalSchema: "app",
                principalTable: "Currencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CostDetailOthers_Currencies_CurrencyId",
                schema: "app",
                table: "CostDetailOthers");

            migrationBuilder.DropIndex(
                name: "IX_CostDetailOthers_CurrencyId",
                schema: "app",
                table: "CostDetailOthers");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                schema: "app",
                table: "CostDetailOthers");
        }
    }
}
