using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class JobSearchApplicantKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_JobSearchApplicants",
                schema: "app",
                table: "JobSearchApplicants");

            migrationBuilder.AddPrimaryKey(
                name: "PK_JobSearchApplicants",
                schema: "app",
                table: "JobSearchApplicants",
                columns: new[] { "JobSearchId", "ApplicantId", "CreatedDate" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_JobSearchApplicants",
                schema: "app",
                table: "JobSearchApplicants");

            migrationBuilder.AddPrimaryKey(
                name: "PK_JobSearchApplicants",
                schema: "app",
                table: "JobSearchApplicants",
                columns: new[] { "JobSearchId", "ApplicantId" });
        }
    }
}
