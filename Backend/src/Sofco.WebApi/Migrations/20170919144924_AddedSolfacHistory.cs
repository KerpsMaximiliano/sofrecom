using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Sofco.WebApi.Migrations
{
    public partial class AddedSolfacHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SolfacHistories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Comment = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    SolfacId = table.Column<int>(nullable: false),
                    SolfacStatusFrom = table.Column<int>(nullable: false),
                    SolfacStatusTo = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolfacHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolfacHistories_Solfacs_SolfacId",
                        column: x => x.SolfacId,
                        principalTable: "Solfacs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SolfacHistories_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SolfacHistories_SolfacId",
                table: "SolfacHistories",
                column: "SolfacId");

            migrationBuilder.CreateIndex(
                name: "IX_SolfacHistories_UserId",
                table: "SolfacHistories",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SolfacHistories");
        }
    }
}
