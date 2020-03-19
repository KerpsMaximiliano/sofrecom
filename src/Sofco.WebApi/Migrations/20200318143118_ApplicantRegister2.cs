using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class ApplicantRegister2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasPhoneInterview",
                schema: "app",
                table: "JobSearchApplicants",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "SeniorityId",
                schema: "app",
                table: "Applicants",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SkillId",
                schema: "app",
                table: "Applicants",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Applicants_SeniorityId",
                schema: "app",
                table: "Applicants",
                column: "SeniorityId");

            migrationBuilder.CreateIndex(
                name: "IX_Applicants_SkillId",
                schema: "app",
                table: "Applicants",
                column: "SkillId");

            migrationBuilder.AddForeignKey(
                name: "FK_Applicants_Seniorities_SeniorityId",
                schema: "app",
                table: "Applicants",
                column: "SeniorityId",
                principalSchema: "app",
                principalTable: "Seniorities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Applicants_Skills_SkillId",
                schema: "app",
                table: "Applicants",
                column: "SkillId",
                principalSchema: "app",
                principalTable: "Skills",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applicants_Seniorities_SeniorityId",
                schema: "app",
                table: "Applicants");

            migrationBuilder.DropForeignKey(
                name: "FK_Applicants_Skills_SkillId",
                schema: "app",
                table: "Applicants");

            migrationBuilder.DropIndex(
                name: "IX_Applicants_SeniorityId",
                schema: "app",
                table: "Applicants");

            migrationBuilder.DropIndex(
                name: "IX_Applicants_SkillId",
                schema: "app",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "HasPhoneInterview",
                schema: "app",
                table: "JobSearchApplicants");

            migrationBuilder.DropColumn(
                name: "SeniorityId",
                schema: "app",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "SkillId",
                schema: "app",
                table: "Applicants");
        }
    }
}
