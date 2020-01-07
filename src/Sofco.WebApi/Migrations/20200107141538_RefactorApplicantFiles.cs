using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class RefactorApplicantFiles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobSearchApplicantFiles",
                schema: "app");

            migrationBuilder.CreateTable(
                name: "ApplicantFiles",
                schema: "app",
                columns: table => new
                {
                    FileId = table.Column<int>(nullable: false),
                    ApplicantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicantFiles", x => new { x.ApplicantId, x.FileId });
                    table.ForeignKey(
                        name: "FK_ApplicantFiles_Applicants_ApplicantId",
                        column: x => x.ApplicantId,
                        principalSchema: "app",
                        principalTable: "Applicants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicantFiles_Files_FileId",
                        column: x => x.FileId,
                        principalSchema: "app",
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicantFiles_FileId",
                schema: "app",
                table: "ApplicantFiles",
                column: "FileId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicantFiles",
                schema: "app");

            migrationBuilder.CreateTable(
                name: "JobSearchApplicantFiles",
                schema: "app",
                columns: table => new
                {
                    JobSearchId = table.Column<int>(nullable: false),
                    ApplicantId = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    FileId = table.Column<int>(nullable: false)
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
    }
}
