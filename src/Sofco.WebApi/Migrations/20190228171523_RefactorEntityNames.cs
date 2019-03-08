using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class RefactorEntityNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CustomerId",
                schema: "app",
                table: "Solfacs",
                newName: "AccountId");

            migrationBuilder.RenameColumn(
                name: "ClientName",
                schema: "app",
                table: "Solfacs",
                newName: "AccountName");

            migrationBuilder.RenameColumn(
                name: "ClientExternalName",
                schema: "app",
                table: "PurchaseOrders",
                newName: "AccountName");

            migrationBuilder.RenameColumn(
                name: "ClientExternalId",
                schema: "app",
                table: "PurchaseOrders",
                newName: "AccountId");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                schema: "app",
                table: "Invoices",
                newName: "AccountId");

            migrationBuilder.RenameColumn(
                name: "ExternalProjectId",
                schema: "app",
                table: "Hitos",
                newName: "ProjectId");

            migrationBuilder.RenameColumn(
                name: "ClientExternalName",
                schema: "app",
                table: "Certificates",
                newName: "AccountName");

            migrationBuilder.RenameColumn(
                name: "ClientExternalId",
                schema: "app",
                table: "Certificates",
                newName: "AccountId");

            migrationBuilder.RenameColumn(
                name: "Service",
                schema: "app",
                table: "Analytics",
                newName: "ServiceName");

            migrationBuilder.RenameColumn(
                name: "ClientExternalName",
                schema: "app",
                table: "Analytics",
                newName: "AccountName");

            migrationBuilder.RenameColumn(
                name: "ClientExternalId",
                schema: "app",
                table: "Analytics",
                newName: "AccountId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AccountName",
                schema: "app",
                table: "Solfacs",
                newName: "ClientName");

            migrationBuilder.RenameColumn(
                name: "AccountId",
                schema: "app",
                table: "Solfacs",
                newName: "CustomerId");

            migrationBuilder.RenameColumn(
                name: "AccountName",
                schema: "app",
                table: "PurchaseOrders",
                newName: "ClientExternalName");

            migrationBuilder.RenameColumn(
                name: "AccountId",
                schema: "app",
                table: "PurchaseOrders",
                newName: "ClientExternalId");

            migrationBuilder.RenameColumn(
                name: "AccountId",
                schema: "app",
                table: "Invoices",
                newName: "CustomerId");

            migrationBuilder.RenameColumn(
                name: "ProjectId",
                schema: "app",
                table: "Hitos",
                newName: "ExternalProjectId");

            migrationBuilder.RenameColumn(
                name: "AccountName",
                schema: "app",
                table: "Certificates",
                newName: "ClientExternalName");

            migrationBuilder.RenameColumn(
                name: "AccountId",
                schema: "app",
                table: "Certificates",
                newName: "ClientExternalId");

            migrationBuilder.RenameColumn(
                name: "ServiceName",
                schema: "app",
                table: "Analytics",
                newName: "Service");

            migrationBuilder.RenameColumn(
                name: "AccountName",
                schema: "app",
                table: "Analytics",
                newName: "ClientExternalName");

            migrationBuilder.RenameColumn(
                name: "AccountId",
                schema: "app",
                table: "Analytics",
                newName: "ClientExternalId");
        }
    }
}
