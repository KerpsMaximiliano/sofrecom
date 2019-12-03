using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class Interview2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClientExternalInterviewer",
                schema: "app",
                table: "JobSearchApplicants",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientInterviewComments",
                schema: "app",
                table: "JobSearchApplicants",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsClientExternal",
                schema: "app",
                table: "JobSearchApplicants",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsTechnicalExternal",
                schema: "app",
                table: "JobSearchApplicants",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TechnicalExternalInterviewer",
                schema: "app",
                table: "JobSearchApplicants",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TechnicalInterviewComments",
                schema: "app",
                table: "JobSearchApplicants",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClientExternalInterviewer",
                schema: "app",
                table: "JobSearchApplicants");

            migrationBuilder.DropColumn(
                name: "ClientInterviewComments",
                schema: "app",
                table: "JobSearchApplicants");

            migrationBuilder.DropColumn(
                name: "IsClientExternal",
                schema: "app",
                table: "JobSearchApplicants");

            migrationBuilder.DropColumn(
                name: "IsTechnicalExternal",
                schema: "app",
                table: "JobSearchApplicants");

            migrationBuilder.DropColumn(
                name: "TechnicalExternalInterviewer",
                schema: "app",
                table: "JobSearchApplicants");

            migrationBuilder.DropColumn(
                name: "TechnicalInterviewComments",
                schema: "app",
                table: "JobSearchApplicants");
        }
    }
}
