using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Sofco.WebApi.Migrations
{
    public partial class AddSoftwareLaw : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SoftwareLaw",
                schema: "app",
                table: "Analytics");

            migrationBuilder.AddColumn<int>(
                name: "SoftwareLawId",
                schema: "app",
                table: "Analytics",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SoftwareLaws",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoftwareLaws", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Analytics_SoftwareLawId",
                schema: "app",
                table: "Analytics",
                column: "SoftwareLawId");

            migrationBuilder.AddForeignKey(
                name: "FK_Analytics_SoftwareLaws_SoftwareLawId",
                schema: "app",
                table: "Analytics",
                column: "SoftwareLawId",
                principalSchema: "app",
                principalTable: "SoftwareLaws",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Analytics_SoftwareLaws_SoftwareLawId",
                schema: "app",
                table: "Analytics");

            migrationBuilder.DropTable(
                name: "SoftwareLaws",
                schema: "app");

            migrationBuilder.DropIndex(
                name: "IX_Analytics_SoftwareLawId",
                schema: "app",
                table: "Analytics");

            migrationBuilder.DropColumn(
                name: "SoftwareLawId",
                schema: "app",
                table: "Analytics");

            migrationBuilder.AddColumn<bool>(
                name: "SoftwareLaw",
                schema: "app",
                table: "Analytics",
                nullable: true);
        }
    }
}
