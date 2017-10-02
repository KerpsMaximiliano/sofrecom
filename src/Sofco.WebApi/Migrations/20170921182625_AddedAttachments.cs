using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Sofco.WebApi.Migrations
{
    public partial class AddedAttachments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SolfacHistories_Users_UserId",
                table: "SolfacHistories");

            migrationBuilder.DropColumn(
                name: "AttachedParts",
                table: "Solfacs");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "SolfacHistories",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "SolfacAttachments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    File = table.Column<byte[]>(nullable: true),
                    Name = table.Column<string>(maxLength: 200, nullable: true),
                    SolfacId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolfacAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolfacAttachments_Solfacs_SolfacId",
                        column: x => x.SolfacId,
                        principalTable: "Solfacs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SolfacAttachments_SolfacId",
                table: "SolfacAttachments",
                column: "SolfacId");

            migrationBuilder.AddForeignKey(
                name: "FK_SolfacHistories_Users_UserId",
                table: "SolfacHistories",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SolfacHistories_Users_UserId",
                table: "SolfacHistories");

            migrationBuilder.DropTable(
                name: "SolfacAttachments");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "SolfacHistories",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<string>(
                name: "AttachedParts",
                table: "Solfacs",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SolfacHistories_Users_UserId",
                table: "SolfacHistories",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
