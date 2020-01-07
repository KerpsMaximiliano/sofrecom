using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class RefactorJobSearchApplicant : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "RemoteWork",
                schema: "app",
                table: "JobSearchApplicants",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "Salary",
                schema: "app",
                table: "JobSearchApplicants",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RemoteWork",
                schema: "app",
                table: "JobSearchApplicants");

            migrationBuilder.DropColumn(
                name: "Salary",
                schema: "app",
                table: "JobSearchApplicants");
        }
    }
}
