using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class UpdateAnalytic : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CostCenterId",
                schema: "app",
                table: "Analytics",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TitleId",
                schema: "app",
                table: "Analytics",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Analytics_CostCenterId",
                schema: "app",
                table: "Analytics",
                column: "CostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_Analytics_DirectorId",
                schema: "app",
                table: "Analytics",
                column: "DirectorId");

            migrationBuilder.CreateIndex(
                name: "IX_Analytics_ManagerId",
                schema: "app",
                table: "Analytics",
                column: "ManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Analytics_CostCenters_CostCenterId",
                schema: "app",
                table: "Analytics",
                column: "CostCenterId",
                principalSchema: "app",
                principalTable: "CostCenters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Analytics_Users_DirectorId",
                schema: "app",
                table: "Analytics",
                column: "DirectorId",
                principalSchema: "app",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Analytics_Users_ManagerId",
                schema: "app",
                table: "Analytics",
                column: "ManagerId",
                principalSchema: "app",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Analytics_CostCenters_CostCenterId",
                schema: "app",
                table: "Analytics");

            migrationBuilder.DropForeignKey(
                name: "FK_Analytics_Users_DirectorId",
                schema: "app",
                table: "Analytics");

            migrationBuilder.DropForeignKey(
                name: "FK_Analytics_Users_ManagerId",
                schema: "app",
                table: "Analytics");

            migrationBuilder.DropIndex(
                name: "IX_Analytics_CostCenterId",
                schema: "app",
                table: "Analytics");

            migrationBuilder.DropIndex(
                name: "IX_Analytics_DirectorId",
                schema: "app",
                table: "Analytics");

            migrationBuilder.DropIndex(
                name: "IX_Analytics_ManagerId",
                schema: "app",
                table: "Analytics");

            migrationBuilder.DropColumn(
                name: "CostCenterId",
                schema: "app",
                table: "Analytics");

            migrationBuilder.DropColumn(
                name: "TitleId",
                schema: "app",
                table: "Analytics");
        }
    }
}
