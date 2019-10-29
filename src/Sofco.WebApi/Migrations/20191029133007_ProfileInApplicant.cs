using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class ProfileInApplicant : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Profile",
                schema: "app",
                table: "Applicants");

            migrationBuilder.AddColumn<int>(
                name: "ProfileId",
                schema: "app",
                table: "Applicants",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Applicants_ProfileId",
                schema: "app",
                table: "Applicants",
                column: "ProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Applicants_Profiles_ProfileId",
                schema: "app",
                table: "Applicants",
                column: "ProfileId",
                principalSchema: "app",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applicants_Profiles_ProfileId",
                schema: "app",
                table: "Applicants");

            migrationBuilder.DropIndex(
                name: "IX_Applicants_ProfileId",
                schema: "app",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "ProfileId",
                schema: "app",
                table: "Applicants");

            migrationBuilder.AddColumn<string>(
                name: "Profile",
                schema: "app",
                table: "Applicants",
                maxLength: 100,
                nullable: true);
        }
    }
}
