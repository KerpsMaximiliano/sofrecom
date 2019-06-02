using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class RefactorPrepaidImportedData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PrepaidImportedData_Prepaids_PrepaidId",
                schema: "app",
                table: "PrepaidImportedData");

            migrationBuilder.AlterColumn<int>(
                name: "PrepaidId",
                schema: "app",
                table: "PrepaidImportedData",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_PrepaidImportedData_Prepaids_PrepaidId",
                schema: "app",
                table: "PrepaidImportedData",
                column: "PrepaidId",
                principalSchema: "app",
                principalTable: "Prepaids",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PrepaidImportedData_Prepaids_PrepaidId",
                schema: "app",
                table: "PrepaidImportedData");

            migrationBuilder.AlterColumn<int>(
                name: "PrepaidId",
                schema: "app",
                table: "PrepaidImportedData",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

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
    }
}
