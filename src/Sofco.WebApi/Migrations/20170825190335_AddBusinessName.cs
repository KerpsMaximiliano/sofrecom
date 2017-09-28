using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class AddBusinessName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BusinessName",
                table: "Solfacs",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Solfacs_DocumentTypeId",
                table: "Solfacs",
                column: "DocumentTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Solfacs_DocumentTypes_DocumentTypeId",
                table: "Solfacs",
                column: "DocumentTypeId",
                principalTable: "DocumentTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Solfacs_DocumentTypes_DocumentTypeId",
                table: "Solfacs");

            migrationBuilder.DropIndex(
                name: "IX_Solfacs_DocumentTypeId",
                table: "Solfacs");

            migrationBuilder.DropColumn(
                name: "BusinessName",
                table: "Solfacs");
        }
    }
}
