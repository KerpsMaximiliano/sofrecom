using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class JobSearchApplicantFile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Comment",
                schema: "app",
                table: "ReportsPowerBi",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstMonth",
                schema: "app",
                table: "ReportsPowerBi",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "JobSearchApplicantFiles",
                schema: "app",
                columns: table => new
                {
                    FileId = table.Column<int>(nullable: false),
                    JobSearchId = table.Column<int>(nullable: false),
                    ApplicantId = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobSearchApplicantFiles", x => new { x.JobSearchId, x.ApplicantId, x.Date, x.FileId });
                    table.ForeignKey(
                        name: "FK_JobSearchApplicantFiles_Files_FileId",
                        column: x => x.FileId,
                        principalSchema: "app",
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobSearchApplicantFiles_JobSearchApplicants_JobSearchId_ApplicantId_Date",
                        columns: x => new { x.JobSearchId, x.ApplicantId, x.Date },
                        principalSchema: "app",
                        principalTable: "JobSearchApplicants",
                        principalColumns: new[] { "JobSearchId", "ApplicantId", "CreatedDate" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobSearchApplicantFiles_FileId",
                schema: "app",
                table: "JobSearchApplicantFiles",
                column: "FileId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobSearchApplicantFiles",
                schema: "app");

            migrationBuilder.DropColumn(
                name: "Comment",
                schema: "app",
                table: "ReportsPowerBi");

            migrationBuilder.DropColumn(
                name: "FirstMonth",
                schema: "app",
                table: "ReportsPowerBi");
        }
    }
}
