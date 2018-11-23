using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Sofco.WebApi.Migrations
{
    public partial class RefactorAdvancement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdvancementDetails",
                schema: "app");

            migrationBuilder.AlterColumn<int>(
                name: "AnalyticId",
                schema: "app",
                table: "Advancements",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<decimal>(
                name: "Ammount",
                schema: "app",
                table: "Advancements",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "AuthorizerId",
                schema: "app",
                table: "Advancements",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "app",
                table: "Advancements",
                maxLength: 400,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Advancements_AuthorizerId",
                schema: "app",
                table: "Advancements",
                column: "AuthorizerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Advancements_Users_AuthorizerId",
                schema: "app",
                table: "Advancements",
                column: "AuthorizerId",
                principalSchema: "app",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Advancements_Users_AuthorizerId",
                schema: "app",
                table: "Advancements");

            migrationBuilder.DropIndex(
                name: "IX_Advancements_AuthorizerId",
                schema: "app",
                table: "Advancements");

            migrationBuilder.DropColumn(
                name: "Ammount",
                schema: "app",
                table: "Advancements");

            migrationBuilder.DropColumn(
                name: "AuthorizerId",
                schema: "app",
                table: "Advancements");

            migrationBuilder.DropColumn(
                name: "Description",
                schema: "app",
                table: "Advancements");

            migrationBuilder.AlterColumn<int>(
                name: "AnalyticId",
                schema: "app",
                table: "Advancements",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "AdvancementDetails",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AdvancementId = table.Column<int>(nullable: false),
                    Ammount = table.Column<decimal>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(maxLength: 400, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdvancementDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdvancementDetails_Advancements_AdvancementId",
                        column: x => x.AdvancementId,
                        principalSchema: "app",
                        principalTable: "Advancements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdvancementDetails_AdvancementId",
                schema: "app",
                table: "AdvancementDetails",
                column: "AdvancementId");
        }
    }
}
