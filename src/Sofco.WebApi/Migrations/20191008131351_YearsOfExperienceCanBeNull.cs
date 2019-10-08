using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class YearsOfExperienceCanBeNull : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "YearsExperience",
                schema: "app",
                table: "JobSearchs",
                nullable: true,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "YearsExperience",
                schema: "app",
                table: "JobSearchs",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
