using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class MarketStudy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobSearchs_Customers_ClientId",
                schema: "app",
                table: "JobSearchs");

            migrationBuilder.AlterColumn<int>(
                name: "ClientId",
                schema: "app",
                table: "JobSearchs",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<bool>(
                name: "IsMarketStudy",
                schema: "app",
                table: "JobSearchs",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "MarketStudy",
                schema: "app",
                table: "JobSearchs",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_JobSearchs_Customers_ClientId",
                schema: "app",
                table: "JobSearchs",
                column: "ClientId",
                principalSchema: "app",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobSearchs_Customers_ClientId",
                schema: "app",
                table: "JobSearchs");

            migrationBuilder.DropColumn(
                name: "IsMarketStudy",
                schema: "app",
                table: "JobSearchs");

            migrationBuilder.DropColumn(
                name: "MarketStudy",
                schema: "app",
                table: "JobSearchs");

            migrationBuilder.AlterColumn<int>(
                name: "ClientId",
                schema: "app",
                table: "JobSearchs",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_JobSearchs_Customers_ClientId",
                schema: "app",
                table: "JobSearchs",
                column: "ClientId",
                principalSchema: "app",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
