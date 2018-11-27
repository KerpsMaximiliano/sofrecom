using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Sofco.WebApi.Migrations
{
    public partial class AdvancementHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdvancementHistories",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AdvancementId = table.Column<int>(nullable: false),
                    Comment = table.Column<string>(maxLength: 400, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    StatusFrom = table.Column<string>(maxLength: 150, nullable: true),
                    StatusTo = table.Column<string>(maxLength: 150, nullable: true),
                    UserName = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdvancementHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdvancementHistories_Advancements_AdvancementId",
                        column: x => x.AdvancementId,
                        principalSchema: "app",
                        principalTable: "Advancements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdvancementHistories_AdvancementId",
                schema: "app",
                table: "AdvancementHistories",
                column: "AdvancementId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdvancementHistories",
                schema: "app");
        }
    }
}
