using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Sofco.WebApi.Migrations
{
    public partial class UserApprover : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_UserApprovers_Users_ApproverUserId",
                        column: x => x.ApproverUserId,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserApprovers",
                schema: "app");
        }
    }
}
