using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class LanguageStudyRequired : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "LanguageRequired",
                schema: "app",
                table: "JobSearchs",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "StudyRequired",
                schema: "app",
                table: "JobSearchs",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LanguageRequired",
                schema: "app",
                table: "JobSearchs");

            migrationBuilder.DropColumn(
                name: "StudyRequired",
                schema: "app",
                table: "JobSearchs");
        }
    }
}
