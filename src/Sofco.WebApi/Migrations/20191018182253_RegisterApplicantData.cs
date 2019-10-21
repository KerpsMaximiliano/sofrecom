using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class RegisterApplicantData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                schema: "app",
                table: "Applicants",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Aggreements",
                schema: "app",
                table: "Applicants",
                maxLength: 3000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AnalyticId",
                schema: "app",
                table: "Applicants",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "BirthDate",
                schema: "app",
                table: "Applicants",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CivilStatus",
                schema: "app",
                table: "Applicants",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cuil",
                schema: "app",
                table: "Applicants",
                maxLength: 12,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ManagerId",
                schema: "app",
                table: "Applicants",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Nationality",
                schema: "app",
                table: "Applicants",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Office",
                schema: "app",
                table: "Applicants",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Prepaid",
                schema: "app",
                table: "Applicants",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Profile",
                schema: "app",
                table: "Applicants",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                schema: "app",
                table: "Applicants",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Salary",
                schema: "app",
                table: "Applicants",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                schema: "app",
                table: "Applicants",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Applicants_AnalyticId",
                schema: "app",
                table: "Applicants",
                column: "AnalyticId");

            migrationBuilder.CreateIndex(
                name: "IX_Applicants_ManagerId",
                schema: "app",
                table: "Applicants",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_Applicants_ProjectId",
                schema: "app",
                table: "Applicants",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Applicants_Analytics_AnalyticId",
                schema: "app",
                table: "Applicants",
                column: "AnalyticId",
                principalSchema: "app",
                principalTable: "Analytics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Applicants_Users_ManagerId",
                schema: "app",
                table: "Applicants",
                column: "ManagerId",
                principalSchema: "app",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Applicants_Projects_ProjectId",
                schema: "app",
                table: "Applicants",
                column: "ProjectId",
                principalSchema: "app",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applicants_Analytics_AnalyticId",
                schema: "app",
                table: "Applicants");

            migrationBuilder.DropForeignKey(
                name: "FK_Applicants_Users_ManagerId",
                schema: "app",
                table: "Applicants");

            migrationBuilder.DropForeignKey(
                name: "FK_Applicants_Projects_ProjectId",
                schema: "app",
                table: "Applicants");

            migrationBuilder.DropIndex(
                name: "IX_Applicants_AnalyticId",
                schema: "app",
                table: "Applicants");

            migrationBuilder.DropIndex(
                name: "IX_Applicants_ManagerId",
                schema: "app",
                table: "Applicants");

            migrationBuilder.DropIndex(
                name: "IX_Applicants_ProjectId",
                schema: "app",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "Address",
                schema: "app",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "Aggreements",
                schema: "app",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "AnalyticId",
                schema: "app",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "BirthDate",
                schema: "app",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "CivilStatus",
                schema: "app",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "Cuil",
                schema: "app",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "ManagerId",
                schema: "app",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "Nationality",
                schema: "app",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "Office",
                schema: "app",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "Prepaid",
                schema: "app",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "Profile",
                schema: "app",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                schema: "app",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "Salary",
                schema: "app",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "StartDate",
                schema: "app",
                table: "Applicants");
        }
    }
}
