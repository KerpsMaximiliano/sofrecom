using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class AddedServiceType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PurchaseOrder",
                schema: "app",
                table: "Analytics");

            migrationBuilder.AddColumn<int>(
                name: "PurchaseOrderId",
                schema: "app",
                table: "Analytics",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ServiceTypeId",
                schema: "app",
                table: "Analytics",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PurchaseOrders",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(maxLength: 60, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceTypes",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(maxLength: 60, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Analytics_PurchaseOrderId",
                schema: "app",
                table: "Analytics",
                column: "PurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Analytics_ServiceTypeId",
                schema: "app",
                table: "Analytics",
                column: "ServiceTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Analytics_PurchaseOrders_PurchaseOrderId",
                schema: "app",
                table: "Analytics",
                column: "PurchaseOrderId",
                principalSchema: "app",
                principalTable: "PurchaseOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Analytics_ServiceTypes_ServiceTypeId",
                schema: "app",
                table: "Analytics",
                column: "ServiceTypeId",
                principalSchema: "app",
                principalTable: "ServiceTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Analytics_PurchaseOrders_PurchaseOrderId",
                schema: "app",
                table: "Analytics");

            migrationBuilder.DropForeignKey(
                name: "FK_Analytics_ServiceTypes_ServiceTypeId",
                schema: "app",
                table: "Analytics");

            migrationBuilder.DropTable(
                name: "PurchaseOrders",
                schema: "app");

            migrationBuilder.DropTable(
                name: "ServiceTypes",
                schema: "app");

            migrationBuilder.DropIndex(
                name: "IX_Analytics_PurchaseOrderId",
                schema: "app",
                table: "Analytics");

            migrationBuilder.DropIndex(
                name: "IX_Analytics_ServiceTypeId",
                schema: "app",
                table: "Analytics");

            migrationBuilder.DropColumn(
                name: "PurchaseOrderId",
                schema: "app",
                table: "Analytics");

            migrationBuilder.DropColumn(
                name: "ServiceTypeId",
                schema: "app",
                table: "Analytics");

            migrationBuilder.AddColumn<int>(
                name: "PurchaseOrder",
                schema: "app",
                table: "Analytics",
                nullable: false,
                defaultValue: 0);
        }
    }
}
