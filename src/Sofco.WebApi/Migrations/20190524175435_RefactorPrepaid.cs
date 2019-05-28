using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class RefactorPrepaid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Prepaid",
                schema: "app",
                table: "PrepaidImportedData");

            migrationBuilder.AddColumn<int>(
                name: "PrepaidId",
                schema: "app",
                table: "PrepaidImportedData",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PrepaidImportedData_PrepaidId",
                schema: "app",
                table: "PrepaidImportedData",
                column: "PrepaidId");

            migrationBuilder.AddForeignKey(
                name: "FK_PrepaidImportedData_Prepaids_PrepaidId",
                schema: "app",
                table: "PrepaidImportedData",
                column: "PrepaidId",
                principalSchema: "app",
                principalTable: "Prepaids",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PrepaidImportedData_Prepaids_PrepaidId",
                schema: "app",
                table: "PrepaidImportedData");

            migrationBuilder.DropIndex(
                name: "IX_PrepaidImportedData_PrepaidId",
                schema: "app",
                table: "PrepaidImportedData");

            migrationBuilder.DropColumn(
                name: "PrepaidId",
                schema: "app",
                table: "PrepaidImportedData");

            migrationBuilder.AddColumn<string>(
                name: "Prepaid",
                schema: "app",
                table: "PrepaidImportedData",
                maxLength: 100,
                nullable: true);
        }
    }
}
