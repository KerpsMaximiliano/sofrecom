using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class ChangePurchaseOrder2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrderFiles_Analytics_AnalyticId",
                schema: "app",
                table: "PurchaseOrderFiles");

            migrationBuilder.DropColumn(
                name: "AnalyticId",
                schema: "app",
                table: "PurchaseOrderFiles");

            migrationBuilder.DropColumn(
                name: "OpportunityDescription",
                schema: "app",
                table: "PurchaseOrderFiles");

            migrationBuilder.DropColumn(
                name: "OpportunityId",
                schema: "app",
                table: "PurchaseOrderFiles");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                schema: "app",
                table: "PurchaseOrderFiles");

            migrationBuilder.CreateTable(
                name: "PurchaseOrderAnalytics",
                schema: "app",
                columns: table => new
                {
                    PurchaseOrderId = table.Column<int>(nullable: false),
                    AnalyticId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrderAnalytics", x => new { x.PurchaseOrderId, x.AnalyticId });
                    table.ForeignKey(
                        name: "FK_PurchaseOrderAnalytics_Analytics_AnalyticId",
                        column: x => x.AnalyticId,
                        principalSchema: "app",
                        principalTable: "Analytics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderAnalytics_PurchaseOrderFiles_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalSchema: "app",
                        principalTable: "PurchaseOrderFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderAnalytics_AnalyticId",
                schema: "app",
                table: "PurchaseOrderAnalytics",
                column: "AnalyticId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PurchaseOrderAnalytics",
                schema: "app");

            migrationBuilder.AddColumn<int>(
                name: "AnalyticId",
                schema: "app",
                table: "PurchaseOrderFiles",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "OpportunityDescription",
                schema: "app",
                table: "PurchaseOrderFiles",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OpportunityId",
                schema: "app",
                table: "PurchaseOrderFiles",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProjectId",
                schema: "app",
                table: "PurchaseOrderFiles",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrderFiles_Analytics_AnalyticId",
                schema: "app",
                table: "PurchaseOrderFiles",
                column: "AnalyticId",
                principalSchema: "app",
                principalTable: "Analytics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
