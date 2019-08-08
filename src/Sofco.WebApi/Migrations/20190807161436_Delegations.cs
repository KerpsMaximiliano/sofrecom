using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class Delegations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Delegations",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    GrantedUserId = table.Column<int>(nullable: false),
                    AnalyticSourceId = table.Column<int>(nullable: true),
                    UserSourceId = table.Column<int>(nullable: true),
                    SourceType = table.Column<int>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Delegations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Delegations_Users_GrantedUserId",
                        column: x => x.GrantedUserId,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Delegations_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Delegations_GrantedUserId",
                schema: "app",
                table: "Delegations",
                column: "GrantedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Delegations_UserId",
                schema: "app",
                table: "Delegations",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Delegations",
                schema: "app");
        }
    }
}
