using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class RefactorJobSearch : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TimeHiring",
                schema: "app",
                table: "JobSearchs",
                newName: "Study");

            migrationBuilder.AddColumn<string>(
                name: "Benefits",
                schema: "app",
                table: "JobSearchs",
                maxLength: 3000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientContact",
                schema: "app",
                table: "JobSearchs",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                schema: "app",
                table: "JobSearchs",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ExtraHoursPaid",
                schema: "app",
                table: "JobSearchs",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "GuardsPaid",
                schema: "app",
                table: "JobSearchs",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasExtraHours",
                schema: "app",
                table: "JobSearchs",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasGuards",
                schema: "app",
                table: "JobSearchs",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "JobTime",
                schema: "app",
                table: "JobSearchs",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "JobType",
                schema: "app",
                table: "JobSearchs",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Language",
                schema: "app",
                table: "JobSearchs",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                schema: "app",
                table: "JobSearchs",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Observations",
                schema: "app",
                table: "JobSearchs",
                maxLength: 3000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ResourceAssignment",
                schema: "app",
                table: "JobSearchs",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TasksToDo",
                schema: "app",
                table: "JobSearchs",
                maxLength: 3000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Telephone",
                schema: "app",
                table: "JobSearchs",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TimeHiringId",
                schema: "app",
                table: "JobSearchs",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "YearsExperience",
                schema: "app",
                table: "JobSearchs",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "JobSearchSkillNotRequired",
                schema: "app",
                columns: table => new
                {
                    JobSearchId = table.Column<int>(nullable: false),
                    SkillId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobSearchSkillNotRequired", x => new { x.JobSearchId, x.SkillId });
                    table.ForeignKey(
                        name: "FK_JobSearchSkillNotRequired_JobSearchs_JobSearchId",
                        column: x => x.JobSearchId,
                        principalSchema: "app",
                        principalTable: "JobSearchs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobSearchSkillNotRequired_Skills_SkillId",
                        column: x => x.SkillId,
                        principalSchema: "app",
                        principalTable: "Skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TimeHirings",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(maxLength: 75, nullable: true),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeHirings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobSearchs_TimeHiringId",
                schema: "app",
                table: "JobSearchs",
                column: "TimeHiringId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSearchSkillNotRequired_SkillId",
                schema: "app",
                table: "JobSearchSkillNotRequired",
                column: "SkillId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobSearchs_TimeHirings_TimeHiringId",
                schema: "app",
                table: "JobSearchs",
                column: "TimeHiringId",
                principalSchema: "app",
                principalTable: "TimeHirings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobSearchs_TimeHirings_TimeHiringId",
                schema: "app",
                table: "JobSearchs");

            migrationBuilder.DropTable(
                name: "JobSearchSkillNotRequired",
                schema: "app");

            migrationBuilder.DropTable(
                name: "TimeHirings",
                schema: "app");

            migrationBuilder.DropIndex(
                name: "IX_JobSearchs_TimeHiringId",
                schema: "app",
                table: "JobSearchs");

            migrationBuilder.DropColumn(
                name: "Benefits",
                schema: "app",
                table: "JobSearchs");

            migrationBuilder.DropColumn(
                name: "ClientContact",
                schema: "app",
                table: "JobSearchs");

            migrationBuilder.DropColumn(
                name: "Email",
                schema: "app",
                table: "JobSearchs");

            migrationBuilder.DropColumn(
                name: "ExtraHoursPaid",
                schema: "app",
                table: "JobSearchs");

            migrationBuilder.DropColumn(
                name: "GuardsPaid",
                schema: "app",
                table: "JobSearchs");

            migrationBuilder.DropColumn(
                name: "HasExtraHours",
                schema: "app",
                table: "JobSearchs");

            migrationBuilder.DropColumn(
                name: "HasGuards",
                schema: "app",
                table: "JobSearchs");

            migrationBuilder.DropColumn(
                name: "JobTime",
                schema: "app",
                table: "JobSearchs");

            migrationBuilder.DropColumn(
                name: "JobType",
                schema: "app",
                table: "JobSearchs");

            migrationBuilder.DropColumn(
                name: "Language",
                schema: "app",
                table: "JobSearchs");

            migrationBuilder.DropColumn(
                name: "Location",
                schema: "app",
                table: "JobSearchs");

            migrationBuilder.DropColumn(
                name: "Observations",
                schema: "app",
                table: "JobSearchs");

            migrationBuilder.DropColumn(
                name: "ResourceAssignment",
                schema: "app",
                table: "JobSearchs");

            migrationBuilder.DropColumn(
                name: "TasksToDo",
                schema: "app",
                table: "JobSearchs");

            migrationBuilder.DropColumn(
                name: "Telephone",
                schema: "app",
                table: "JobSearchs");

            migrationBuilder.DropColumn(
                name: "TimeHiringId",
                schema: "app",
                table: "JobSearchs");

            migrationBuilder.DropColumn(
                name: "YearsExperience",
                schema: "app",
                table: "JobSearchs");

            migrationBuilder.RenameColumn(
                name: "Study",
                schema: "app",
                table: "JobSearchs",
                newName: "TimeHiring");
        }
    }
}
