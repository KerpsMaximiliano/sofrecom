using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class JobSearch2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobSearchs_Users_UserId",
                schema: "app",
                table: "JobSearchs");

            migrationBuilder.AddColumn<int>(
                name: "RecruiterId",
                schema: "app",
                table: "JobSearchs",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_JobSearchs_RecruiterId",
                schema: "app",
                table: "JobSearchs",
                column: "RecruiterId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobSearchs_Users_RecruiterId",
                schema: "app",
                table: "JobSearchs",
                column: "RecruiterId",
                principalSchema: "app",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JobSearchs_Users_UserId",
                schema: "app",
                table: "JobSearchs",
                column: "UserId",
                principalSchema: "app",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobSearchs_Users_RecruiterId",
                schema: "app",
                table: "JobSearchs");

            migrationBuilder.DropForeignKey(
                name: "FK_JobSearchs_Users_UserId",
                schema: "app",
                table: "JobSearchs");

            migrationBuilder.DropIndex(
                name: "IX_JobSearchs_RecruiterId",
                schema: "app",
                table: "JobSearchs");

            migrationBuilder.DropColumn(
                name: "RecruiterId",
                schema: "app",
                table: "JobSearchs");

            migrationBuilder.AddForeignKey(
                name: "FK_JobSearchs_Users_UserId",
                schema: "app",
                table: "JobSearchs",
                column: "UserId",
                principalSchema: "app",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
