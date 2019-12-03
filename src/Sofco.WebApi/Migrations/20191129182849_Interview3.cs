using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class Interview3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TechnicalInterviewComments",
                schema: "app",
                table: "JobSearchApplicants",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ClientInterviewComments",
                schema: "app",
                table: "JobSearchApplicants",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RrhhInterviewComments",
                schema: "app",
                table: "JobSearchApplicants",
                maxLength: 100,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RrhhInterviewComments",
                schema: "app",
                table: "JobSearchApplicants");

            migrationBuilder.AlterColumn<string>(
                name: "TechnicalInterviewComments",
                schema: "app",
                table: "JobSearchApplicants",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ClientInterviewComments",
                schema: "app",
                table: "JobSearchApplicants",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 100,
                oldNullable: true);
        }
    }
}
