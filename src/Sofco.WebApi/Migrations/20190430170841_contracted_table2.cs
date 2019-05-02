using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class contracted_table2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContratedDetail_Analytics_AnalyticId",
                schema: "app",
                table: "ContratedDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ContratedDetail",
                schema: "app",
                table: "ContratedDetail");

            migrationBuilder.DropIndex(
                name: "IX_ContratedDetail_AnalyticId",
                schema: "app",
                table: "ContratedDetail");

            migrationBuilder.DropColumn(
                name: "AnalyticId",
                schema: "app",
                table: "ContratedDetail");

            migrationBuilder.RenameTable(
                name: "ContratedDetail",
                schema: "app",
                newName: "ContractedDetail",
                newSchema: "app");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "app",
                table: "ContractedDetail",
                maxLength: 250,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContractedDetail",
                schema: "app",
                table: "ContractedDetail",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ContractedDetail_IdAnalytic",
                schema: "app",
                table: "ContractedDetail",
                column: "IdAnalytic");

            migrationBuilder.AddForeignKey(
                name: "FK_ContractedDetail_Analytics_IdAnalytic",
                schema: "app",
                table: "ContractedDetail",
                column: "IdAnalytic",
                principalSchema: "app",
                principalTable: "Analytics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContractedDetail_Analytics_IdAnalytic",
                schema: "app",
                table: "ContractedDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ContractedDetail",
                schema: "app",
                table: "ContractedDetail");

            migrationBuilder.DropIndex(
                name: "IX_ContractedDetail_IdAnalytic",
                schema: "app",
                table: "ContractedDetail");

            migrationBuilder.RenameTable(
                name: "ContractedDetail",
                schema: "app",
                newName: "ContratedDetail",
                newSchema: "app");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "app",
                table: "ContratedDetail",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 250,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AnalyticId",
                schema: "app",
                table: "ContratedDetail",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContratedDetail",
                schema: "app",
                table: "ContratedDetail",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ContratedDetail_AnalyticId",
                schema: "app",
                table: "ContratedDetail",
                column: "AnalyticId");

            migrationBuilder.AddForeignKey(
                name: "FK_ContratedDetail_Analytics_AnalyticId",
                schema: "app",
                table: "ContratedDetail",
                column: "AnalyticId",
                principalSchema: "app",
                principalTable: "Analytics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
