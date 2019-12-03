using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class JobSearchApplicantInterview : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ClientInterviewDate",
                schema: "app",
                table: "JobSearchApplicants",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientInterviewPlace",
                schema: "app",
                table: "JobSearchApplicants",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ClientInterviewerId",
                schema: "app",
                table: "JobSearchApplicants",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasClientInterview",
                schema: "app",
                table: "JobSearchApplicants",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasRrhhInterview",
                schema: "app",
                table: "JobSearchApplicants",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasTechnicalInterview",
                schema: "app",
                table: "JobSearchApplicants",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "RrhhInterviewDate",
                schema: "app",
                table: "JobSearchApplicants",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RrhhInterviewPlace",
                schema: "app",
                table: "JobSearchApplicants",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RrhhInterviewerId",
                schema: "app",
                table: "JobSearchApplicants",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TechnicalInterviewDate",
                schema: "app",
                table: "JobSearchApplicants",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TechnicalInterviewPlace",
                schema: "app",
                table: "JobSearchApplicants",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TechnicalInterviewerId",
                schema: "app",
                table: "JobSearchApplicants",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobSearchApplicants_ClientInterviewerId",
                schema: "app",
                table: "JobSearchApplicants",
                column: "ClientInterviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSearchApplicants_RrhhInterviewerId",
                schema: "app",
                table: "JobSearchApplicants",
                column: "RrhhInterviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSearchApplicants_TechnicalInterviewerId",
                schema: "app",
                table: "JobSearchApplicants",
                column: "TechnicalInterviewerId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobSearchApplicants_Users_ClientInterviewerId",
                schema: "app",
                table: "JobSearchApplicants",
                column: "ClientInterviewerId",
                principalSchema: "app",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JobSearchApplicants_Users_RrhhInterviewerId",
                schema: "app",
                table: "JobSearchApplicants",
                column: "RrhhInterviewerId",
                principalSchema: "app",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JobSearchApplicants_Users_TechnicalInterviewerId",
                schema: "app",
                table: "JobSearchApplicants",
                column: "TechnicalInterviewerId",
                principalSchema: "app",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobSearchApplicants_Users_ClientInterviewerId",
                schema: "app",
                table: "JobSearchApplicants");

            migrationBuilder.DropForeignKey(
                name: "FK_JobSearchApplicants_Users_RrhhInterviewerId",
                schema: "app",
                table: "JobSearchApplicants");

            migrationBuilder.DropForeignKey(
                name: "FK_JobSearchApplicants_Users_TechnicalInterviewerId",
                schema: "app",
                table: "JobSearchApplicants");

            migrationBuilder.DropIndex(
                name: "IX_JobSearchApplicants_ClientInterviewerId",
                schema: "app",
                table: "JobSearchApplicants");

            migrationBuilder.DropIndex(
                name: "IX_JobSearchApplicants_RrhhInterviewerId",
                schema: "app",
                table: "JobSearchApplicants");

            migrationBuilder.DropIndex(
                name: "IX_JobSearchApplicants_TechnicalInterviewerId",
                schema: "app",
                table: "JobSearchApplicants");

            migrationBuilder.DropColumn(
                name: "ClientInterviewDate",
                schema: "app",
                table: "JobSearchApplicants");

            migrationBuilder.DropColumn(
                name: "ClientInterviewPlace",
                schema: "app",
                table: "JobSearchApplicants");

            migrationBuilder.DropColumn(
                name: "ClientInterviewerId",
                schema: "app",
                table: "JobSearchApplicants");

            migrationBuilder.DropColumn(
                name: "HasClientInterview",
                schema: "app",
                table: "JobSearchApplicants");

            migrationBuilder.DropColumn(
                name: "HasRrhhInterview",
                schema: "app",
                table: "JobSearchApplicants");

            migrationBuilder.DropColumn(
                name: "HasTechnicalInterview",
                schema: "app",
                table: "JobSearchApplicants");

            migrationBuilder.DropColumn(
                name: "RrhhInterviewDate",
                schema: "app",
                table: "JobSearchApplicants");

            migrationBuilder.DropColumn(
                name: "RrhhInterviewPlace",
                schema: "app",
                table: "JobSearchApplicants");

            migrationBuilder.DropColumn(
                name: "RrhhInterviewerId",
                schema: "app",
                table: "JobSearchApplicants");

            migrationBuilder.DropColumn(
                name: "TechnicalInterviewDate",
                schema: "app",
                table: "JobSearchApplicants");

            migrationBuilder.DropColumn(
                name: "TechnicalInterviewPlace",
                schema: "app",
                table: "JobSearchApplicants");

            migrationBuilder.DropColumn(
                name: "TechnicalInterviewerId",
                schema: "app",
                table: "JobSearchApplicants");
        }
    }
}
