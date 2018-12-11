using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class RefactorAdvancement3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "MonthsReturnId",
                schema: "app",
                table: "Advancements",
                nullable: true,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "MonthsReturnId",
                schema: "app",
                table: "Advancements",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
