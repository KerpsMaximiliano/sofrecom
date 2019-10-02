using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class JobSearchApplicant : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JobSearchApplicants",
                schema: "app",
                columns: table => new
                {
                    JobSearchId = table.Column<int>(nullable: false),
                    ApplicantId = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ReasonId = table.Column<int>(nullable: false),
                    Comments = table.Column<string>(maxLength: 3000, nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobSearchApplicants", x => new { x.JobSearchId, x.ApplicantId });
                    table.ForeignKey(
                        name: "FK_JobSearchApplicants_Applicants_ApplicantId",
                        column: x => x.ApplicantId,
                        principalSchema: "app",
                        principalTable: "Applicants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobSearchApplicants_JobSearchs_JobSearchId",
                        column: x => x.JobSearchId,
                        principalSchema: "app",
                        principalTable: "JobSearchs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobSearchApplicants_ReasonCauses_ReasonId",
                        column: x => x.ReasonId,
                        principalSchema: "app",
                        principalTable: "ReasonCauses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobSearchApplicants_ApplicantId",
                schema: "app",
                table: "JobSearchApplicants",
                column: "ApplicantId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSearchApplicants_ReasonId",
                schema: "app",
                table: "JobSearchApplicants",
                column: "ReasonId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobSearchApplicants",
                schema: "app");
        }
    }
}
