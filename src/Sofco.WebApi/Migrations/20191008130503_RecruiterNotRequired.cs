using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class RecruiterNotRequired : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "RecruiterId",
                schema: "app",
                table: "JobSearchs",
                nullable: true,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "RecruiterId",
                schema: "app",
                table: "JobSearchs",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
