using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class RefactorResourceBilling : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResourceBillings_PurchaseOrders_PurchaseOrderId",
                schema: "app",
                table: "ResourceBillings");

            migrationBuilder.RenameColumn(
                name: "PurchaseOrderId",
                schema: "app",
                table: "ResourceBillings",
                newName: "EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_ResourceBillings_PurchaseOrderId",
                schema: "app",
                table: "ResourceBillings",
                newName: "IX_ResourceBillings_EmployeeId");

            migrationBuilder.AddColumn<string>(
                name: "HitoCrmId",
                schema: "app",
                table: "ResourceBillings",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ResourceBillings_Employees_EmployeeId",
                schema: "app",
                table: "ResourceBillings",
                column: "EmployeeId",
                principalSchema: "app",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResourceBillings_Employees_EmployeeId",
                schema: "app",
                table: "ResourceBillings");

            migrationBuilder.DropColumn(
                name: "HitoCrmId",
                schema: "app",
                table: "ResourceBillings");

            migrationBuilder.RenameColumn(
                name: "EmployeeId",
                schema: "app",
                table: "ResourceBillings",
                newName: "PurchaseOrderId");

            migrationBuilder.RenameIndex(
                name: "IX_ResourceBillings_EmployeeId",
                schema: "app",
                table: "ResourceBillings",
                newName: "IX_ResourceBillings_PurchaseOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_ResourceBillings_PurchaseOrders_PurchaseOrderId",
                schema: "app",
                table: "ResourceBillings",
                column: "PurchaseOrderId",
                principalSchema: "app",
                principalTable: "PurchaseOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
