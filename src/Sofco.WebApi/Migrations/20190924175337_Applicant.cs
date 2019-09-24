using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class Applicant : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Applicants",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LastName = table.Column<string>(maxLength: 75, nullable: true),
                    FirstName = table.Column<string>(maxLength: 75, nullable: true),
                    Comments = table.Column<string>(maxLength: 3000, nullable: true),
                    Email = table.Column<string>(maxLength: 75, nullable: true),
                    ClientId = table.Column<int>(nullable: true),
                    ReasonCauseId = table.Column<int>(nullable: true),
                    RecommendedByUserId = table.Column<int>(nullable: true),
                    CountryCode1 = table.Column<string>(maxLength: 5, nullable: true),
                    AreaCode1 = table.Column<string>(maxLength: 5, nullable: true),
                    Telephone1 = table.Column<string>(maxLength: 15, nullable: true),
                    CountryCode2 = table.Column<string>(maxLength: 5, nullable: true),
                    AreaCode2 = table.Column<string>(maxLength: 5, nullable: true),
                    Telephone2 = table.Column<string>(maxLength: 15, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 25, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applicants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Applicants_Customers_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "app",
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Applicants_ReasonCauses_ReasonCauseId",
                        column: x => x.ReasonCauseId,
                        principalSchema: "app",
                        principalTable: "ReasonCauses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Applicants_Users_RecommendedByUserId",
                        column: x => x.RecommendedByUserId,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ApplicantProfiles",
                schema: "app",
                columns: table => new
                {
                    ApplicantId = table.Column<int>(nullable: false),
                    ProfileId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicantProfiles", x => new { x.ApplicantId, x.ProfileId });
                    table.ForeignKey(
                        name: "FK_ApplicantProfiles_Applicants_ApplicantId",
                        column: x => x.ApplicantId,
                        principalSchema: "app",
                        principalTable: "Applicants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicantProfiles_Profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalSchema: "app",
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApplicantSkillses",
                schema: "app",
                columns: table => new
                {
                    ApplicantId = table.Column<int>(nullable: false),
                    SkillId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicantSkillses", x => new { x.ApplicantId, x.SkillId });
                    table.ForeignKey(
                        name: "FK_ApplicantSkillses_Applicants_ApplicantId",
                        column: x => x.ApplicantId,
                        principalSchema: "app",
                        principalTable: "Applicants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicantSkillses_Skills_SkillId",
                        column: x => x.SkillId,
                        principalSchema: "app",
                        principalTable: "Skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicantProfiles_ProfileId",
                schema: "app",
                table: "ApplicantProfiles",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Applicants_ClientId",
                schema: "app",
                table: "Applicants",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Applicants_ReasonCauseId",
                schema: "app",
                table: "Applicants",
                column: "ReasonCauseId");

            migrationBuilder.CreateIndex(
                name: "IX_Applicants_RecommendedByUserId",
                schema: "app",
                table: "Applicants",
                column: "RecommendedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicantSkillses_SkillId",
                schema: "app",
                table: "ApplicantSkillses",
                column: "SkillId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicantProfiles",
                schema: "app");

            migrationBuilder.DropTable(
                name: "ApplicantSkillses",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Applicants",
                schema: "app");
        }
    }
}
