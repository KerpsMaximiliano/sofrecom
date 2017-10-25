using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Sofco.WebApi.Migrations
{
    public partial class AddClientGroupToAnalytic : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ClientGroup",
                schema: "app",
                table: "Analytics",
                newName: "ClientGroupId");

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                schema: "app",
                table: "Technologies",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                schema: "app",
                table: "Solutions",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "ClientGroups",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientGroups", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Analytics_ClientGroupId",
                schema: "app",
                table: "Analytics",
                column: "ClientGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Analytics_ClientGroups_ClientGroupId",
                schema: "app",
                table: "Analytics",
                column: "ClientGroupId",
                principalSchema: "app",
                principalTable: "ClientGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Analytics_ClientGroups_ClientGroupId",
                schema: "app",
                table: "Analytics");

            migrationBuilder.DropTable(
                name: "ClientGroups",
                schema: "app");

            migrationBuilder.DropIndex(
                name: "IX_Analytics_ClientGroupId",
                schema: "app",
                table: "Analytics");

            migrationBuilder.RenameColumn(
                name: "ClientGroupId",
                schema: "app",
                table: "Analytics",
                newName: "ClientGroup");

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                schema: "app",
                table: "Technologies",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 30,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                schema: "app",
                table: "Solutions",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 30,
                oldNullable: true);
        }
    }
}
