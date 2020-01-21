using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class ReasonKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserDelegate",
                schema: "app");

            migrationBuilder.DropPrimaryKey(
                name: "PK_JobSearchApplicants",
                schema: "app",
                table: "JobSearchApplicants");

            migrationBuilder.AddPrimaryKey(
                name: "PK_JobSearchApplicants",
                schema: "app",
                table: "JobSearchApplicants",
                columns: new[] { "JobSearchId", "ApplicantId", "CreatedDate", "ReasonId" });
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
                columns: new[] { "JobSearchId", "ApplicantId", "CreatedDate" });

            migrationBuilder.CreateTable(
                name: "UserDelegate",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Created = table.Column<DateTime>(nullable: true),
                    CreatedUser = table.Column<string>(maxLength: 50, nullable: true),
                    Modified = table.Column<DateTime>(nullable: true),
                    ServiceId = table.Column<Guid>(nullable: false),
                    SourceId = table.Column<int>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDelegate", x => x.Id);
                });
        }
    }
}
