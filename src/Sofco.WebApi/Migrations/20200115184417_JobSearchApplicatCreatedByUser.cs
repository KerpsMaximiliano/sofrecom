using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class JobSearchApplicatCreatedByUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                schema: "app",
                table: "JobSearchApplicants",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobSearchApplicants_CreatedByUserId",
                schema: "app",
                table: "JobSearchApplicants",
                column: "CreatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobSearchApplicants_Users_CreatedByUserId",
                schema: "app",
                table: "JobSearchApplicants",
                column: "CreatedByUserId",
                principalSchema: "app",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobSearchApplicants_Users_CreatedByUserId",
                schema: "app",
                table: "JobSearchApplicants");

            migrationBuilder.DropIndex(
                name: "IX_JobSearchApplicants_CreatedByUserId",
                schema: "app",
                table: "JobSearchApplicants");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                schema: "app",
                table: "JobSearchApplicants");
        }
    }
}
