using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class RemoveClientFromApplicant : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applicants_Customers_ClientId",
                schema: "app",
                table: "Applicants");

            migrationBuilder.RenameColumn(
                name: "ClientId",
                schema: "app",
                table: "Applicants",
                newName: "CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_Applicants_ClientId",
                schema: "app",
                table: "Applicants",
                newName: "IX_Applicants_CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Applicants_Customers_CustomerId",
                schema: "app",
                table: "Applicants",
                column: "CustomerId",
                principalSchema: "app",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applicants_Customers_CustomerId",
                schema: "app",
                table: "Applicants");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                schema: "app",
                table: "Applicants",
                newName: "ClientId");

            migrationBuilder.RenameIndex(
                name: "IX_Applicants_CustomerId",
                schema: "app",
                table: "Applicants",
                newName: "IX_Applicants_ClientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Applicants_Customers_ClientId",
                schema: "app",
                table: "Applicants",
                column: "ClientId",
                principalSchema: "app",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
