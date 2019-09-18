using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class costsubtype2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CostDetailSubtype_CostDetailTypes_TypeId",
                schema: "app",
                table: "CostDetailSubtype");

            migrationBuilder.DropIndex(
                name: "IX_CostDetailSubtype_TypeId",
                schema: "app",
                table: "CostDetailSubtype");

            migrationBuilder.DropColumn(
                name: "TypeId",
                schema: "app",
                table: "CostDetailSubtype");

            migrationBuilder.AddColumn<int>(
                name: "CostDetailTypeId",
                schema: "app",
                table: "CostDetailSubtype",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CostDetailSubtype_CostDetailTypeId",
                schema: "app",
                table: "CostDetailSubtype",
                column: "CostDetailTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_CostDetailSubtype_CostDetailTypes_CostDetailTypeId",
                schema: "app",
                table: "CostDetailSubtype",
                column: "CostDetailTypeId",
                principalSchema: "app",
                principalTable: "CostDetailTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CostDetailSubtype_CostDetailTypes_CostDetailTypeId",
                schema: "app",
                table: "CostDetailSubtype");

            migrationBuilder.DropIndex(
                name: "IX_CostDetailSubtype_CostDetailTypeId",
                schema: "app",
                table: "CostDetailSubtype");

            migrationBuilder.DropColumn(
                name: "CostDetailTypeId",
                schema: "app",
                table: "CostDetailSubtype");

            migrationBuilder.AddColumn<int>(
                name: "TypeId",
                schema: "app",
                table: "CostDetailSubtype",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CostDetailSubtype_TypeId",
                schema: "app",
                table: "CostDetailSubtype",
                column: "TypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_CostDetailSubtype_CostDetailTypes_TypeId",
                schema: "app",
                table: "CostDetailSubtype",
                column: "TypeId",
                principalSchema: "app",
                principalTable: "CostDetailTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
