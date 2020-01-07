﻿using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class RefactorInterview : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobSearchApplicants_Users_ClientInterviewerId",
                schema: "app",
                table: "JobSearchApplicants");

            migrationBuilder.DropForeignKey(
                name: "FK_JobSearchApplicants_Users_TechnicalInterviewerId",
                schema: "app",
                table: "JobSearchApplicants");

            migrationBuilder.DropTable(
                name: "UserApprovers",
                schema: "app");

            migrationBuilder.DropIndex(
                name: "IX_JobSearchApplicants_ClientInterviewerId",
                schema: "app",
                table: "JobSearchApplicants");

            migrationBuilder.DropIndex(
                name: "IX_JobSearchApplicants_TechnicalInterviewerId",
                schema: "app",
                table: "JobSearchApplicants");

            migrationBuilder.DropColumn(
                name: "ClientInterviewerId",
                schema: "app",
                table: "JobSearchApplicants");

            migrationBuilder.DropColumn(
                name: "IsClientExternal",
                schema: "app",
                table: "JobSearchApplicants");

            migrationBuilder.DropColumn(
                name: "IsTechnicalExternal",
                schema: "app",
                table: "JobSearchApplicants");

            migrationBuilder.DropColumn(
                name: "TechnicalInterviewerId",
                schema: "app",
                table: "JobSearchApplicants");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClientInterviewerId",
                schema: "app",
                table: "JobSearchApplicants",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsClientExternal",
                schema: "app",
                table: "JobSearchApplicants",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsTechnicalExternal",
                schema: "app",
                table: "JobSearchApplicants",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "TechnicalInterviewerId",
                schema: "app",
                table: "JobSearchApplicants",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserApprovers",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AnalyticId = table.Column<int>(nullable: false),
                    ApproverUserId = table.Column<int>(nullable: false),
                    Created = table.Column<DateTime>(nullable: true),
                    CreatedUser = table.Column<string>(nullable: true),
                    EmployeeId = table.Column<int>(nullable: false),
                    Modified = table.Column<DateTime>(nullable: true),
                    ModifiedUser = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserApprovers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserApprovers_Analytics_AnalyticId",
                        column: x => x.AnalyticId,
                        principalSchema: "app",
                        principalTable: "Analytics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserApprovers_Users_ApproverUserId",
                        column: x => x.ApproverUserId,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobSearchApplicants_ClientInterviewerId",
                schema: "app",
                table: "JobSearchApplicants",
                column: "ClientInterviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSearchApplicants_TechnicalInterviewerId",
                schema: "app",
                table: "JobSearchApplicants",
                column: "TechnicalInterviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_UserApprovers_AnalyticId",
                schema: "app",
                table: "UserApprovers",
                column: "AnalyticId");

            migrationBuilder.CreateIndex(
                name: "IX_UserApprovers_ApproverUserId",
                schema: "app",
                table: "UserApprovers",
                column: "ApproverUserId");

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
                name: "FK_JobSearchApplicants_Users_TechnicalInterviewerId",
                schema: "app",
                table: "JobSearchApplicants",
                column: "TechnicalInterviewerId",
                principalSchema: "app",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
