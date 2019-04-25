using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class CostDetail_Adjustment3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "Adjustment",
                schema: "app",
                table: "CostDetail",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Adjustment",
                schema: "app",
                table: "CostDetail",
                nullable: true,
                oldClrType: typeof(float),
                oldNullable: true);
        }
    }
}
