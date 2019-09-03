using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class JobSearch : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JobSearchs",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(nullable: false),
                    ReasonCauseId = table.Column<int>(nullable: false),
                    ClientId = table.Column<int>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    TimeHiring = table.Column<string>(maxLength: 100, nullable: true),
                    MaximunSalary = table.Column<decimal>(nullable: false),
                    Comments = table.Column<string>(maxLength: 3000, nullable: true),
                    Status = table.Column<int>(nullable: false),
                    ReopenDate = table.Column<DateTime>(nullable: true),
                    SuspendedDate = table.Column<DateTime>(nullable: true),
                    CloseDate = table.Column<DateTime>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobSearchs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobSearchs_Customers_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "app",
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobSearchs_ReasonCauses_ReasonCauseId",
                        column: x => x.ReasonCauseId,
                        principalSchema: "app",
                        principalTable: "ReasonCauses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobSearchs_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobSearchProfiles",
                schema: "app",
                columns: table => new
                {
                    JobSearchId = table.Column<int>(nullable: false),
                    ProfileId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobSearchProfiles", x => new { x.JobSearchId, x.ProfileId });
                    table.ForeignKey(
                        name: "FK_JobSearchProfiles_JobSearchs_JobSearchId",
                        column: x => x.JobSearchId,
                        principalSchema: "app",
                        principalTable: "JobSearchs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobSearchProfiles_Profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalSchema: "app",
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobSearchSeniorities",
                schema: "app",
                columns: table => new
                {
                    JobSearchId = table.Column<int>(nullable: false),
                    SeniorityId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobSearchSeniorities", x => new { x.JobSearchId, x.SeniorityId });
                    table.ForeignKey(
                        name: "FK_JobSearchSeniorities_JobSearchs_JobSearchId",
                        column: x => x.JobSearchId,
                        principalSchema: "app",
                        principalTable: "JobSearchs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobSearchSeniorities_Seniorities_SeniorityId",
                        column: x => x.SeniorityId,
                        principalSchema: "app",
                        principalTable: "Seniorities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobSearchSkills",
                schema: "app",
                columns: table => new
                {
                    JobSearchId = table.Column<int>(nullable: false),
                    SkillId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobSearchSkills", x => new { x.JobSearchId, x.SkillId });
                    table.ForeignKey(
                        name: "FK_JobSearchSkills_JobSearchs_JobSearchId",
                        column: x => x.JobSearchId,
                        principalSchema: "app",
                        principalTable: "JobSearchs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobSearchSkills_Skills_SkillId",
                        column: x => x.SkillId,
                        principalSchema: "app",
                        principalTable: "Skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobSearchProfiles_ProfileId",
                schema: "app",
                table: "JobSearchProfiles",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSearchs_ClientId",
                schema: "app",
                table: "JobSearchs",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSearchs_ReasonCauseId",
                schema: "app",
                table: "JobSearchs",
                column: "ReasonCauseId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSearchs_UserId",
                schema: "app",
                table: "JobSearchs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSearchSeniorities_SeniorityId",
                schema: "app",
                table: "JobSearchSeniorities",
                column: "SeniorityId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSearchSkills_SkillId",
                schema: "app",
                table: "JobSearchSkills",
                column: "SkillId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobSearchProfiles",
                schema: "app");

            migrationBuilder.DropTable(
                name: "JobSearchSeniorities",
                schema: "app");

            migrationBuilder.DropTable(
                name: "JobSearchSkills",
                schema: "app");

            migrationBuilder.DropTable(
                name: "JobSearchs",
                schema: "app");
        }
    }
}
