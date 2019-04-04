using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class costdetailreorden2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "EmployeeId",
                schema: "app",
                table: "CostDetail",
                nullable: true,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "EmployeeId",
                schema: "app",
                table: "CostDetail",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
